/**************************************************************************/
/*  library_fade_audio.js                                                */
/**************************************************************************/
/*                         This file is part of:                          */
/*                             GODOT ENGINE                               */
/*                        https://fadeengine.org                         */
/**************************************************************************/
/* Copyright (c) 2014-present FADE Engine contributors (see AUTHORS.md). */
/* Copyright (c) 2007-2014 Juan Linietsky, Ariel Manzur.                  */
/*                                                                        */
/* Permission is hereby granted, free of charge, to any person obtaining  */
/* a copy of this software and associated documentation files (the        */
/* "Software"), to deal in the Software without restriction, including    */
/* without limitation the rights to use, copy, modify, merge, publish,    */
/* distribute, sublicense, and/or sell copies of the Software, and to     */
/* permit persons to whom the Software is furnished to do so, subject to  */
/* the following conditions:                                              */
/*                                                                        */
/* The above copyright notice and this permission notice shall be         */
/* included in all copies or substantial portions of the Software.        */
/*                                                                        */
/* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        */
/* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     */
/* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. */
/* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   */
/* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   */
/* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE      */
/* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 */
/**************************************************************************/

function FADESampleDriver(audioContext, positionWorkletPath) {
	/** @type {?AudioContext} */
	let ctx = audioContext;
	/** @type {?Promise} */
	let audioPositionWorkletPromise = ctx.audioWorklet ? ctx.audioWorklet.addModule(positionWorkletPath) : null;
	/** @type {!Array<AudioWorkletNode>} */
	const audioPositionWorkletNodes = [];

	/** @enum {string} */
	const LoopMode = {
		DISABLED: 'disabled',
		FORWARD: 'forward',
		BACKWARD: 'backward',
		PINGPONG: 'pingpong',
	};
	const FADEChannel = Object.freeze({
		CHANNEL_L: 0,
		CHANNEL_R: 1,
		CHANNEL_C: 2,
		CHANNEL_LFE: 3,
		CHANNEL_SL: 4, // Surround left
		CHANNEL_SR: 5, // Surround right
		CHANNEL_SBL: 6, // Surround back left (7.1)
		CHANNEL_SBR: 7, // Surround back right (7.1)
	});
	const WebChannel = Object.freeze({
		CHANNEL_L: 0,
		CHANNEL_R: 1,
		CHANNEL_SL: 2,
		CHANNEL_SR: 3,
		CHANNEL_C: 4,
		CHANNEL_LFE: 5,
	});
	const MAX_VOLUME_CHANNELS = 8;
	const NUMBER_OF_WEB_CHANNELS = Object.keys(WebChannel).length;
	const MAX_SAFE_FLOAT32_INTEGER = 16777215;
	/**
	 * JS pair of a FADE AudioBus.
	 * @typedef {{
	 *   getId: function(): number,
	 *   getInputNode: function(): !AudioNode,
	 *   getOutputNode: function(): !AudioNode,
	 *   setVolumeDb: function (FADEBus?): void,
	 *   setSend: function(FADEBus?): void,
	 *   setMute: function(boolean): void,
	 *   setSoloMute: function(boolean): void,
	 *   clear: function(): void,
	 * }}
	 */
	var FADEBus; // eslint-disable-line no-var, no-unassigned-vars
	/**
	 * A mixing sub-graph to control FADEPlayback volume outputs to a FADEBus.
	 * @typedef {{
	 *   setVolume: function(!Float32Array): void,
	 *   getInputNode: function(): !AudioNode,
	 *   getOutputNode: function(): !AudioNode,
	 *   clear: function(): void,
	 * }}
	 */
	var FADEVolumeMixer; // eslint-disable-line no-var, no-unassigned-vars
	/**
	 * JS pair of a FADE AudioStream/AudioSample.
	 * @typedef {{
	 *   loopBegin: number,
	 *   loopEnd: number,
	 *   loopMode: LoopMode,
	 *   length: number,
	 *   getBuffer: function(): !AudioBuffer,
	 * }}
	 */
	var FADEStream; // eslint-disable-line no-var, no-unassigned-vars
	/**
	 * JS pair of a FADE AudioStreamPlayback/AudioSamplePlayback.
	 * @typedef {{
	 *   stop: function(): void,
	 *   start: function(number): void,
	 *   pause: function(boolean): void,
	 *   setVolumes: function(Array<number>, !Float32Array),
	 *   setPitchScale: function(number): void,
	 *   getPlaybackPosition: function(): number,
	 * }}
	 */
	var FADEPlayback; // eslint-disable-line no-var, no-unassigned-vars

	/** @type {FADEBus?} */
	let busSolo = null;
	/** @type {Array<FADEBus>} */
	let buses = [];
	/** @type {Map<string, FADEStream>} */
	const streams = new Map();
	/** @type {Map<string, FADEPlayback>} */
	const playbacks = new Map();

	/** @type {?function(number): void}*/
	let finishedCallback = null;

	/**
	 * @param {?function(number): void} callback
	 */
	function setFinishedCallback(callback) {
		finishedCallback = callback;
	}

	/**
	 * @param {number} db
	 * @return {number}
	 */
	function db_to_linear(db) {
		return Math.exp(db * 0.11512925464970228420089957273422); // eslint-disable-line no-loss-of-precision
	}

	/**
	 * @return {!FADEBus}
	 */
	function createBus() {
		/** @type {!GainNode} */ const muteNode = ctx.createGain();
		/** @type {!GainNode} */ const gainNode = ctx.createGain();
		/** @type {!GainNode} */ const soloNode = ctx.createGain();
		gainNode.connect(soloNode).connect(muteNode);

		const bus = {};
		/** @return {!AudioNode} */ bus.getInputNode = () => gainNode;
		/** @return {!AudioNode} */ bus.getOutputNode = () => muteNode;
		/** @return {number} */
		bus.getId = function () {
			return buses.indexOf(bus);
		};
		/** @param {number} val */
		bus.setVolumeDb = function (val) {
			const linear = db_to_linear(val);
			if (isFinite(linear)) {
				gainNode.gain.value = linear;
			}
		};
		/** @param {?FADEBus} val */
		bus.setSend = function (val) {
			if (val == null) {
				if (bus.getId() != 0) {
					FADERuntime.error(`Cannot set bus send to null when the bus not at index 0 (current index: ${bus.getId()})`);
					return;
				}
				bus.getOutputNode().connect(ctx.destination);
			} else {
				bus.getOutputNode().disconnect();
				bus.getOutputNode().connect(val.getInputNode());
			}
		};
		/** @param {boolean} enable */
		bus.setMute = function (enable) {
			muteNode.gain.value = enable ? 0 : 1;
		};
		/** @param {boolean} enable */
		bus.setSoloMute = function (enable) {
			soloNode.gain.value = enable ? 0 : 1;
		};
		bus.clear = function () {
			bus.getInputNode().disconnect();
			bus.getOutputNode().disconnect();
		};
		buses.push(bus);
		return bus;
	}
	/**
	 * @param {number} index
	 * @return {?FADEBus}
	 */
	function getBus(index) {
		return buses[index] ?? null;
	}

	/**
	 * @param {number} bus
	 * @return {!FADEVolumeMixer}
	 */
	function createVolumeMixer(bus) {
		/** @type {!ChannelSplitterNode} */ const splitter = ctx.createChannelSplitter(NUMBER_OF_WEB_CHANNELS);
		/** @type {!Array<!GainNode>} */ const channels = [];
		/** @type {!ChannelMergerNode} */ const merger = ctx.createChannelMerger(NUMBER_OF_WEB_CHANNELS);
		Object.values(WebChannel).forEach((idx) => {
			const ch = ctx.createGain();
			splitter.connect(ch, idx).connect(merger, 0, idx);
			channels[idx] = ch;
		});
		merger.connect(getBus(bus).getInputNode());

		const node = {};
		/** @return {!AudioNode} */ node.getInputNode = () => splitter;
		/** @return {!AudioNode} */ node.getOutputNode = () => merger;
		/** @param {!Float32Array} volume */
		node.setVolume = function (volume) {
			if (volume.length !== MAX_VOLUME_CHANNELS) {
				FADERuntime.error(`Volume length isn't "${MAX_VOLUME_CHANNELS}", is ${volume.length} instead`);
				return;
			}
			channels[WebChannel.CHANNEL_L].gain.value = volume[FADEChannel.CHANNEL_L] ?? 0;
			channels[WebChannel.CHANNEL_R].gain.value = volume[FADEChannel.CHANNEL_R] ?? 0;
			channels[WebChannel.CHANNEL_SL].gain.value = volume[FADEChannel.CHANNEL_SL] ?? 0;
			channels[WebChannel.CHANNEL_SR].gain.value = volume[FADEChannel.CHANNEL_SR] ?? 0;
			channels[WebChannel.CHANNEL_C].gain.value = volume[FADEChannel.CHANNEL_C] ?? 0;
			channels[WebChannel.CHANNEL_LFE].gain.value = volume[FADEChannel.CHANNEL_LFE] ?? 0;
		};
		node.clear = function () {
			splitter.disconnect();
			channels.forEach((ch) => {
				ch.disconnect();
			});
			merger.disconnect();
		};
		return node;
	}
	/**
	 * @param {string} streamId
	 * @param {!AudioBuffer} sourceBuffer
	 * @param {{
	 *   loopMode: (LoopMode|undefined),
	 *   loopBegin: (number|undefined),
	 *   loopEnd: (number|undefined),
	 * }=} options
	 * @return {!FADEStream}
	 */
	function createStream(streamId, sourceBuffer, options = {}) {
		const stream = {
			/** @type {LoopMode} */ loopMode: options.loopMode ?? LoopMode.DISABLED,
			/** @type {number} */ loopBegin: options.loopBegin ?? 0,
			/** @type {number} */ loopEnd: options.loopEnd ?? 0,
			/** @type {number} */ length: sourceBuffer.length,
		};
		/**
		 * The same AudioBuffer can be used by multiple AudioBufferSourceNodes.
		 * @return {!AudioBuffer}
		 */
		stream.getBuffer = function () {
			return sourceBuffer;
		};
		streams.set(streamId, stream);
		return stream;
	}
	/**
	 * @param {string} id
	 * @return {!FADEStream}
	 */
	function getStream(id) {
		return streams.get(id) ?? null;
	}
	/** @param {string} id */
	function removeStream(id) {
		streams.delete(id);
	}

	/**
	 * @param {string} playbackId
	 * @return {!FADEPlayback}
	 */
	function createPlayback(playbackId, streamId) {
		/** @type {number} */ const sampleRate = ctx.sampleRate;
		const output = /** @struct */ {
			/** @type {!Map<number, FADEVolumeMixer|null>} */
			mixers: new Map(),
			/** @type {!Float32Array} */
			volumes: new Float32Array(0),
		};
		const {
			/** @type {number} */ length,
			/** @type {LoopMode} */ loopMode,
			/** @type {number} */ loopBegin,
			/** @type {number} */ loopEnd,
		} = getStream(streamId);
		/** @type {boolean} */ const looping = loopMode !== LoopMode.DISABLED;
		/** @type {boolean} */ let started = false;
		/** @type {boolean} */ let paused = false;
		/** @type {number} */ let processed = 0;
		/** @type {number} */ let pitchScale = 1;
		/** @type {?AudioBufferSourceNode} */ let source = null;
		/** @type {?AudioWorkletNode} */ let worklet = null;
		/** @type {number} */ let workletId = 0;

		const node = {};
		function attachWorklet() {
			if (!source) {
				return; // Already stopped.
			}
			if (audioPositionWorkletNodes.length > 0) {
				worklet = audioPositionWorkletNodes.pop();
			} else {
				worklet = new AudioWorkletNode(/** @type {!AudioContext} */ (ctx), 'fade-position-reporting-processor');
			}
			const end = loopEnd ? Math.min(loopEnd, length) : length;
			worklet.port.onmessage = (event) => {
				const [id, data] = event.data;
				if (id != workletId) {
					return;
				}
				processed += data;
				if (processed > end) {
					processed -= end;
					if (looping) {
						processed += loopBegin;
					}
				}
			};
			worklet.parameters.get('id').setValueAtTime(workletId, ctx.currentTime);
			worklet.parameters.get('scale').setValueAtTime(pitchScale, ctx.currentTime);
			source.connect(worklet);
		}
		/** @param {!Array<number>} newBuses */
		function compareOutputs(newBuses) {
			const keys = Array.from(output.mixers.keys());
			if (keys.length != newBuses.length) {
				return false;
			}
			return newBuses.find((b) => !keys.includes(b)) === undefined;
		}
		/**
		 * @param {!Array<number>} newBuses
		 * @param {!Float32Array} volumes
		 */
		function setVolumes(newBuses, volumes) {
			if (!compareOutputs(newBuses)) {
				for (const mixer of output.mixers.values()) {
					if (!mixer) {
						continue;
					}
					if (source) {
						source.disconnect(mixer.getInputNode());
					}
					mixer.clear();
				}
				output.mixers.clear();
			}
			output.volumes = volumes;
			for (let idx = 0; idx < newBuses.length; idx++) {
				const bus = newBuses[idx];
				let mixer = output.mixers.get(bus);
				if (!mixer) {
					mixer = createVolumeMixer(bus);
					if (source) {
						source.connect(mixer.getInputNode());
					}
					output.mixers.set(bus, mixer);
				}
				const size = MAX_VOLUME_CHANNELS;
				mixer.setVolume(volumes.slice(idx * size, (idx * size) + size));
			}
		}
		function clearSource() {
			if (source) {
				source.onended = null;
				source.stop();
				source.disconnect();
				source = null;
			}
			output.mixers.forEach(function (mixer, key, map) {
				if (mixer) {
					if (source) {
						source.disconnect(mixer.getInputNode());
					}
					mixer.clear();
				}
				map.set(key, null);
			});
			if (worklet) {
				worklet.disconnect();
				worklet.port.onmessage = null;
				audioPositionWorkletNodes.push(worklet);
				worklet = null;
			}
			started = false;
			paused = false;
			processed = 0;
		}
		function createSource() {
			clearSource();

			// Source
			source = ctx.createBufferSource();
			source.buffer = getStream(streamId).getBuffer();
			source.onended = (_) => {
				if (paused) {
					return;
				}
				node.stop();
				if (!finishedCallback) {
					playbacks.delete(playbackId); // Cleanup after ourselves.
					return;
				}
				const cstr = FADERuntime.allocString(playbackId);
				finishedCallback(cstr);
				FADERuntime.free(cstr);
			};
			if (looping) {
				source.loop = true;
				source.loopStart = loopBegin / sampleRate;
				source.loopEnd = (loopEnd ? loopEnd : length) / sampleRate;
			}
			node.setPitchScale(pitchScale);
			node.setVolumes(Array.from(output.mixers.keys()), output.volumes);

			// Worklet
			workletId = Math.floor(Math.random() * MAX_SAFE_FLOAT32_INTEGER);
			if (audioPositionWorkletPromise) { // Unavailable outside secure contexts.
				audioPositionWorkletPromise.then(attachWorklet).catch((err) => {
					const newErr = new Error('Failed to create PositionWorklet.');
					newErr.cause = err;
					FADERuntime.error(newErr);
				});
			}
		}
		/** @param {number} startPosition */
		function start(startPosition) {
			if (started) {
				return;
			}
			createSource();
			processed = startPosition * sampleRate;
			source.start(0, startPosition);
			started = true;
			paused = false;
		}
		/** @param {number} newPitchScale */
		function setPitchScale(newPitchScale) {
			pitchScale = newPitchScale;
			if (!source || paused) {
				return;
			}
			source.playbackRate.value = pitchScale;
			if (worklet) {
				worklet.parameters.get('scale').setValueAtTime(pitchScale, ctx.currentTime);
			}
		};
		/** @param {boolean} enable */
		function pause(enable) {
			if (paused == enable) {
				return;
			}
			paused = enable;
			if (!started || !source) {
				return;
			}
			if (enable) {
				source.playbackRate.value = 0;
				if (worklet) {
					worklet.parameters.get('scale').setValueAtTime(0, ctx.currentTime);
				}
			} else {
				setPitchScale(pitchScale);
			}
		};
		/** @return {number} */
		function getPlaybackPosition() {
			return processed / sampleRate;
		}
		node.stop = clearSource;
		node.start = start;
		node.pause = pause;
		node.setVolumes = setVolumes;
		node.setPitchScale = setPitchScale;
		node.getPlaybackPosition = getPlaybackPosition;
		playbacks.set(playbackId, node);
		return node;
	}
	/**
	 * @param {string} id
	 * @return {?FADEPlayback}
	 */
	function getPlayback(id) {
		return playbacks.get(id) ?? null;
	}
	/** @param {string} id */
	function removePlayback(id) {
		const playback = playbacks.get(id) ?? null;
		if (playback) {
			playback.stop();
			playbacks.delete(id);
		}
	}
	/** @param {number} count */
	function setBusCount(count) {
		if (count < 1) {
			FADERuntime.error('Invalid bus index, driver bug');
			return;
		}
		if (count === buses.length) {
			return;
		}
		if (count < buses.length) {
			// TODO: what to do with nodes connected to the deleted buses?
			const deletedBuses = buses.slice(count);
			for (let i = 0; i < deletedBuses.length; i++) {
				const deletedBus = deletedBuses[i];
				deletedBus.clear();
			}
			buses = buses.slice(0, count);
			return;
		}
		for (let i = buses.length; i < count; i++) {
			const bus = createBus();
			if (i === 0) {
				bus.setSend(null);
			} else {
				bus.setSend(getBus(0));
			}
		}
	}
	/**
	 * @param {number} fromIndex
	 * @param {number} toIndex
	 */
	function moveBus(fromIndex, toIndex) {
		if (fromIndex < 1 || toIndex < 1) {
			FADERuntime.error('Invalid bus index, driver bug');
			return;
		}
		const movedBus = getBus(fromIndex);
		if (!movedBus) {
			return;
		}
		const newBuses = buses.filter((_, i) => i !== fromIndex);
		// Inserts at index.
		newBuses.splice(toIndex - 1, 0, movedBus);
		buses = newBuses;
	}
	/** @param {number} index */
	function addBusAt(index) {
		if (index < 1) {
			FADERuntime.error('Invalid bus index, driver bug');
			return;
		}
		const bus = createBus();
		if (index !== bus.getId()) {
			moveBus(bus.getId(), index);
		}
		if (bus.getId() === 0) {
			bus.setSend(null);
		} else {
			bus.setSend(getBus(0));
		}
	}
	/** @param {number} index */
	function removeBus(index) {
		if (index < 1) {
			FADERuntime.error('Invalid bus index, driver bug');
			return;
		}
		const bus = getBus(index);
		if (!bus) {
			return;
		}
		bus.clear();
		buses = buses.filter((v) => v !== bus);
	}
	/**
	 * @param {number} busIndex
	 * @param {boolean} enable
	 */
	function setBusSolo(busIndex, enable) {
		const bus = getBus(busIndex);
		if (!bus) {
			return;
		}
		const current = busSolo;
		if (enable && current != bus) {
			busSolo = bus;
			bus.setSoloMute(false);
			buses.filter((o) => o !== bus).forEach((b) => {
				b.setSoloMute(true);
			});
		} else if (!enable && current == bus) {
			busSolo = null;
			buses.forEach((b) => {
				b.setSoloMute(false);
			});
		}
	}
	function clear() {
		audioPositionWorkletPromise = null;
		playbacks.forEach((p) => p.stop());
		playbacks.clear();
		streams.clear();
		buses.splice(0, buses.length).forEach((b) => b.clear());
		audioPositionWorkletNodes.splice(0, audioPositionWorkletNodes.length);
		ctx = null;
	}
	const driver = /** @struct */ {
		setFinishedCallback,
		clear,
		// Streams
		createStream,
		getStream,
		removeStream,
		// Playbacks
		createPlayback,
		getPlayback,
		removePlayback,
		// Buses
		getBus,
		setBusSolo,
		setBusCount,
		moveBus,
		addBusAt,
		removeBus,
		// So they survive pre-stripping.
		_: { FADEBus, FADEVolumeMixer, FADEStream, FADEPlayback },
	};
	return driver;
};

const _FADEAudio = {
	$FADEAudio__deps: ['$FADERuntime', '$FADEOS'],
	$FADEAudio: {
		/** @type {AudioContext} */
		ctx: null,
		input: null,
		driver: null,
		sample_driver: null,
		interval: 0,

		FADESampleDriver,
		init: function (mix_rate, latency, onstatechange, onlatencyupdate) {
			const opts = {};
			// If mix_rate is 0, let the browser choose.
			if (mix_rate) {
				FADEAudio.sampleRate = mix_rate;
				opts['sampleRate'] = mix_rate;
			}
			// Do not specify, leave 'interactive' for good performance.
			// opts['latencyHint'] = latency / 1000;
			const ctx = new (window.AudioContext || window.webkitAudioContext)(opts);
			FADEAudio.ctx = ctx;
			ctx.onstatechange = function () {
				let state = 0;
				switch (ctx.state) {
				case 'suspended':
					state = 0;
					break;
				case 'running':
					state = 1;
					break;
				case 'closed':
					state = 2;
					break;
				default:
					// Do nothing.
				}
				onstatechange(state);
			};
			ctx.onstatechange(); // Immediately notify state.
			// Update computed latency
			FADEAudio.interval = setInterval(function () {
				let computed_latency = 0;
				if (ctx.baseLatency) {
					computed_latency += FADEAudio.ctx.baseLatency;
				}
				if (ctx.outputLatency) {
					computed_latency += FADEAudio.ctx.outputLatency;
				}
				onlatencyupdate(computed_latency);
			}, 1000);
			FADEOS.atexit(FADEAudio.close_async);

			// Initialize sample driver.
			const path = FADEConfig.locate_file('fade.audio.position.worklet.js');
			FADEAudio.sample_driver = FADEAudio.FADESampleDriver(ctx, path);

			return ctx.destination.channelCount;
		},

		create_input: function (callback) {
			if (FADEAudio.input) {
				return 0; // Already started.
			}
			function gotMediaInput(stream) {
				try {
					FADEAudio.input = FADEAudio.ctx.createMediaStreamSource(stream);
					callback(FADEAudio.input);
				} catch (e) {
					FADERuntime.error('Failed creating input.', e);
				}
			}
			if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
				navigator.mediaDevices.getUserMedia({
					'audio': true,
				}).then(gotMediaInput, function (e) {
					FADERuntime.error('Error getting user media.', e);
				});
			} else {
				if (!navigator.getUserMedia) {
					navigator.getUserMedia = navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
				}
				if (!navigator.getUserMedia) {
					FADERuntime.error('getUserMedia not available.');
					return 1;
				}
				navigator.getUserMedia({
					'audio': true,
				}, gotMediaInput, function (e) {
					FADERuntime.print(e);
				});
			}
			return 0;
		},

		close_async: function (resolve, reject) {
			const ctx = FADEAudio.ctx;
			FADEAudio.ctx = null;
			// Audio was not initialized.
			if (!ctx) {
				resolve();
				return;
			}
			// Clear audio driver
			if (FADEAudio.sample_driver) {
				FADEAudio.sample_driver.clear();
				FADEAudio.sample_driver = null;
			}
			// Remove latency callback
			if (FADEAudio.interval) {
				clearInterval(FADEAudio.interval);
				FADEAudio.interval = 0;
			}
			// Disconnect input, if it was started.
			if (FADEAudio.input) {
				FADEAudio.input.disconnect();
				FADEAudio.input = null;
			}
			// Disconnect output
			let closed = Promise.resolve();
			if (FADEAudio.driver) {
				closed = FADEAudio.driver.close();
			}
			closed.then(function () {
				return ctx.close();
			}).then(function () {
				ctx.onstatechange = null;
				resolve();
			}).catch(function (e) {
				ctx.onstatechange = null;
				FADERuntime.error('Error closing AudioContext', e);
				resolve();
			});
		},
	},

	fade_audio_is_available__sig: 'i',
	fade_audio_is_available__proxy: 'sync',
	fade_audio_is_available: function () {
		if (!(window.AudioContext || window.webkitAudioContext)) {
			return 0;
		}
		return 1;
	},

	fade_audio_has_worklet__proxy: 'sync',
	fade_audio_has_worklet__sig: 'i',
	fade_audio_has_worklet: function () {
		return FADEAudio.ctx && FADEAudio.ctx.audioWorklet ? 1 : 0;
	},

	fade_audio_has_script_processor__proxy: 'sync',
	fade_audio_has_script_processor__sig: 'i',
	fade_audio_has_script_processor: function () {
		return FADEAudio.ctx && FADEAudio.ctx.createScriptProcessor ? 1 : 0;
	},

	fade_audio_init__proxy: 'sync',
	fade_audio_init__sig: 'iiiii',
	fade_audio_init: function (
		p_mix_rate,
		p_latency,
		p_state_change,
		p_latency_update
	) {
		const statechange = FADERuntime.get_func(p_state_change);
		const latencyupdate = FADERuntime.get_func(p_latency_update);
		const mix_rate = FADERuntime.getHeapValue(p_mix_rate, 'i32');
		const channels = FADEAudio.init(
			mix_rate,
			p_latency,
			statechange,
			latencyupdate
		);
		FADERuntime.setHeapValue(p_mix_rate, FADEAudio.ctx.sampleRate, 'i32');
		return channels;
	},

	fade_audio_resume__proxy: 'sync',
	fade_audio_resume__sig: 'v',
	fade_audio_resume: function () {
		if (FADEAudio.ctx && FADEAudio.ctx.state !== 'running') {
			FADEAudio.ctx.resume();
		}
	},

	fade_audio_input_start__proxy: 'sync',
	fade_audio_input_start__sig: 'i',
	fade_audio_input_start: function () {
		return FADEAudio.create_input(function (input) {
			input.connect(FADEAudio.driver.get_node());
		});
	},

	fade_audio_input_stop__proxy: 'sync',
	fade_audio_input_stop__sig: 'v',
	fade_audio_input_stop: function () {
		if (FADEAudio.input) {
			const tracks = FADEAudio.input['mediaStream']['getTracks']();
			for (let i = 0; i < tracks.length; i++) {
				tracks[i]['stop']();
			}
			FADEAudio.input.disconnect();
			FADEAudio.input = null;
		}
	},
};

autoAddDeps(_FADEAudio, '$FADEAudio');
mergeInto(LibraryManager.library, _FADEAudio);

/**
 * The audio sample driver used by AudioStreamPlayer with playback_type = Sample.
 */
const _FADEAudioSampleDriver = {
	$FADEAudioSampleDriver__deps: ['$FADEAudio', '$FADERuntime'],
	$FADEAudioSampleDriver: {},

	fade_audio_sample_stream_is_registered__proxy: 'sync',
	fade_audio_sample_stream_is_registered__sig: 'ii',
	fade_audio_sample_stream_is_registered: function (p_id) {
		if (!FADEAudio.sample_driver) {
			return 0;
		}
		return FADEAudio.sample_driver.getStream(FADERuntime.parseString(p_id)) ? 1 : 0;
	},

	fade_audio_sample_register_stream__proxy: 'sync',
	fade_audio_sample_register_stream__sig: 'viiiiii',
	fade_audio_sample_register_stream: function (p_id, p_frames, p_frames_count, p_loop_mode, p_loop_begin, p_loop_end) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		const BYTES_PER_FLOAT32 = 4;
		const sid = FADERuntime.parseString(p_id);
		const loopMode = FADERuntime.parseString(p_loop_mode);
		const numberOfChannels = 2;
		const sampleRate = FADEAudio.ctx.sampleRate;

		// AudioBuffer Cant's copy from a SharedArrayBuffer-backed view.
		const sliceOrSub = HEAPF32.buffer instanceof ArrayBuffer ? FADERuntime.heapSub : FADERuntime.heapSlice;
		const subLeft = sliceOrSub(HEAPF32, p_frames, p_frames_count);
		const subRight = sliceOrSub(HEAPF32, p_frames + p_frames_count * BYTES_PER_FLOAT32, p_frames_count);
		const audioBuffer = FADEAudio.ctx.createBuffer(numberOfChannels, p_frames_count, sampleRate);
		audioBuffer.copyToChannel(subLeft, 0, 0);
		audioBuffer.copyToChannel(subRight, 1, 0);

		const opts = {
			/** @export */ loopBegin: p_loop_begin,
			/** @export */ loopEnd: p_loop_end,
			/** @export */ loopMode,
		};
		FADEAudio.sample_driver.createStream(sid, audioBuffer, opts);
	},

	fade_audio_sample_unregister_stream__proxy: 'sync',
	fade_audio_sample_unregister_stream__sig: 'vi',
	fade_audio_sample_unregister_stream: function (p_id) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		FADEAudio.sample_driver.removeStream(FADERuntime.parseString(p_id));
	},

	fade_audio_sample_start__proxy: 'sync',
	fade_audio_sample_start__sig: 'viiiifi',
	fade_audio_sample_start: function (p_playback, p_stream, p_bus, p_offset, p_pitch_scale, p_volume) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		const sid = FADERuntime.parseString(p_stream);
		if (!FADEAudio.sample_driver.getStream(sid)) {
			return;
		}
		const pid = FADERuntime.parseString(p_playback);
		FADEAudio.sample_driver.removePlayback(pid);
		const playback = FADEAudio.sample_driver.createPlayback(pid, sid);
		const volume = FADERuntime.heapSlice(HEAPF32, p_volume, 8); // Slice (copy) since it will be stored.
		playback.setVolumes([p_bus], volume);
		playback.setPitchScale(p_pitch_scale);
		playback.start(p_offset);
	},

	fade_audio_sample_stop__proxy: 'sync',
	fade_audio_sample_stop__sig: 'vi',
	fade_audio_sample_stop: function (p_id) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		FADEAudio.sample_driver.removePlayback(FADERuntime.parseString(p_id));
	},

	fade_audio_sample_set_pause__proxy: 'sync',
	fade_audio_sample_set_pause__sig: 'vii',
	fade_audio_sample_set_pause: function (p_id, p_pause) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		const playback = FADEAudio.sample_driver.getPlayback(FADERuntime.parseString(p_id));
		if (playback) {
			playback.pause(Boolean(p_pause));
		}
	},

	fade_audio_sample_is_active__proxy: 'sync',
	fade_audio_sample_is_active__sig: 'ii',
	fade_audio_sample_is_active: function (p_id) {
		if (!FADEAudio.sample_driver) {
			return 0;
		}
		return FADEAudio.sample_driver.getPlayback(FADERuntime.parseString(p_id)) ? 1 : 0;
	},

	fade_audio_get_sample_playback_position__proxy: 'sync',
	fade_audio_get_sample_playback_position__sig: 'di',
	fade_audio_get_sample_playback_position: function (p_id) {
		if (!FADEAudio.sample_driver) {
			return 0;
		}
		const playback = FADEAudio.sample_driver.getPlayback(FADERuntime.parseString(p_id));
		return playback ? playback.getPlaybackPosition() : 0;
	},

	fade_audio_sample_update_pitch_scale__proxy: 'sync',
	fade_audio_sample_update_pitch_scale__sig: 'vii',
	fade_audio_sample_update_pitch_scale: function (p_id, p_pitch_scale) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		const playback = FADEAudio.sample_driver.getPlayback(FADERuntime.parseString(p_id));
		if (playback) {
			playback.setPitchScale(p_pitch_scale);
		}
	},

	fade_audio_sample_set_volumes_linear__proxy: 'sync',
	fade_audio_sample_set_volumes_linear__sig: 'viiiii',
	fade_audio_sample_set_volumes_linear: function (p_id, p_buses, p_buses_size, p_volume, p_volume_size) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		const playback = FADEAudio.sample_driver.getPlayback(FADERuntime.parseString(p_id));
		if (!playback) {
			return;
		}
		const busesIndexes = Array.from(FADERuntime.heapSub(HEAP32, p_buses, p_buses_size));
		const volumes = FADERuntime.heapSlice(HEAPF32, p_volume, p_volume_size); // Slice (copy) since it will be stored.
		playback.setVolumes(busesIndexes, volumes);
	},

	fade_audio_sample_bus_set_count__proxy: 'sync',
	fade_audio_sample_bus_set_count__sig: 'vi',
	fade_audio_sample_bus_set_count: function (p_count) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		FADEAudio.sample_driver.setBusCount(p_count);
	},

	fade_audio_sample_bus_remove__proxy: 'sync',
	fade_audio_sample_bus_remove__sig: 'vi',
	fade_audio_sample_bus_remove: function (p_index) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		FADEAudio.sample_driver.removeBus(p_index);
	},

	fade_audio_sample_bus_add__proxy: 'sync',
	fade_audio_sample_bus_add__sig: 'vi',
	fade_audio_sample_bus_add: function (p_index) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		FADEAudio.sample_driver.addBusAt(p_index);
	},

	fade_audio_sample_bus_move__proxy: 'sync',
	fade_audio_sample_bus_move__sig: 'vii',
	fade_audio_sample_bus_move: function (p_from, p_to) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		FADEAudio.sample_driver.moveBus(p_from, p_to);
	},

	fade_audio_sample_bus_set_send__proxy: 'sync',
	fade_audio_sample_bus_set_send__sig: 'vii',
	fade_audio_sample_bus_set_send: function (p_index, p_send_index) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		const bus = FADEAudio.sample_driver.getBus(p_index);
		if (!bus) {
			return;
		}
		let target = FADEAudio.sample_driver.getBus(p_send_index);
		if (target == null) { // Send to master.
			target = FADEAudio.sample_driver.getBus(0);
		}
		bus.setSend(target);
	},

	fade_audio_sample_bus_set_volume_db__proxy: 'sync',
	fade_audio_sample_bus_set_volume_db__sig: 'vii',
	fade_audio_sample_bus_set_volume_db: function (p_bus, p_volume_db) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		const bus = FADEAudio.sample_driver.getBus(p_bus);
		if (!bus) {
			return;
		}
		bus.setVolumeDb(p_volume_db);
	},

	fade_audio_sample_bus_set_solo__proxy: 'sync',
	fade_audio_sample_bus_set_solo__sig: 'vii',
	fade_audio_sample_bus_set_solo: function (p_bus, p_enable) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		FADEAudio.sample_driver.setBusSolo(p_bus, p_enable);
	},

	fade_audio_sample_bus_set_mute__proxy: 'sync',
	fade_audio_sample_bus_set_mute__sig: 'vii',
	fade_audio_sample_bus_set_mute: function (p_bus, p_enable) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		const bus = FADEAudio.sample_driver.getBus(p_bus);
		if (!bus) {
			return;
		}
		bus.setMute(Boolean(p_enable));
	},

	fade_audio_sample_set_finished_callback__proxy: 'sync',
	fade_audio_sample_set_finished_callback__sig: 'vi',
	fade_audio_sample_set_finished_callback: function (p_callback) {
		if (!FADEAudio.sample_driver) {
			return;
		}
		FADEAudio.sample_driver.setFinishedCallback(FADERuntime.get_func(p_callback));
	},
};

autoAddDeps(_FADEAudioSampleDriver, '$FADEAudioSampleDriver');
mergeInto(LibraryManager.library, _FADEAudioSampleDriver);

/**
 * The AudioWorklet API driver, used when threads are available.
 */
const FADEAudioWorklet = {
	$FADEAudioWorklet__deps: ['$FADEAudio', '$FADEConfig'],
	$FADEAudioWorklet: {
		promise: null,
		worklet: null,
		ring_buffer: null,

		create: function (channels) {
			const path = FADEConfig.locate_file('fade.audio.worklet.js');
			FADEAudioWorklet.promise = FADEAudio.ctx.audioWorklet
				.addModule(path)
				.then(function () {
					FADEAudioWorklet.worklet = new AudioWorkletNode(
						FADEAudio.ctx,
						'fade-processor',
						{
							outputChannelCount: [channels],
						}
					);
					return Promise.resolve();
				});
			FADEAudio.driver = FADEAudioWorklet;
		},

		start: function (in_buf, out_buf, state) {
			FADEAudioWorklet.promise.then(function () {
				const node = FADEAudioWorklet.worklet;
				node.connect(FADEAudio.ctx.destination);
				node.port.postMessage({
					'cmd': 'start',
					'data': [state, in_buf, out_buf],
				});
				node.port.onmessage = function (event) {
					FADERuntime.error(event.data);
				};
			});
		},

		start_no_threads: function (
			p_out_buf,
			p_out_size,
			out_callback,
			p_in_buf,
			p_in_size,
			in_callback
		) {
			/** @constructor */
			function RingBuffer() {
				let wpos = 0;
				let rpos = 0;
				let pending_samples = 0;
				const wbuf = new Float32Array(p_out_size);

				function send(port) {
					if (pending_samples === 0) {
						return;
					}
					const buffer = FADERuntime.heapSub(HEAPF32, p_out_buf, p_out_size);
					const size = buffer.length;
					const tot_sent = pending_samples;
					out_callback(wpos, pending_samples);
					if (wpos + pending_samples >= size) {
						const high = size - wpos;
						wbuf.set(buffer.subarray(wpos, size));
						pending_samples -= high;
						wpos = 0;
					}
					if (pending_samples > 0) {
						wbuf.set(
							buffer.subarray(wpos, wpos + pending_samples),
							tot_sent - pending_samples
						);
					}
					port.postMessage({ 'cmd': 'chunk', 'data': wbuf.subarray(0, tot_sent) });
					wpos += pending_samples;
					pending_samples = 0;
				}
				this.receive = function (recv_buf) {
					const buffer = FADERuntime.heapSub(HEAPF32, p_in_buf, p_in_size);
					const from = rpos;
					let to_write = recv_buf.length;
					let high = 0;
					if (rpos + to_write >= p_in_size) {
						high = p_in_size - rpos;
						buffer.set(recv_buf.subarray(0, high), rpos);
						to_write -= high;
						rpos = 0;
					}
					if (to_write) {
						buffer.set(recv_buf.subarray(high, to_write), rpos);
					}
					in_callback(from, recv_buf.length);
					rpos += to_write;
				};
				this.consumed = function (size, port) {
					pending_samples += size;
					send(port);
				};
			}
			FADEAudioWorklet.ring_buffer = new RingBuffer();
			FADEAudioWorklet.promise.then(function () {
				const node = FADEAudioWorklet.worklet;
				const buffer = FADERuntime.heapSlice(HEAPF32, p_out_buf, p_out_size);
				node.connect(FADEAudio.ctx.destination);
				node.port.postMessage({
					'cmd': 'start_nothreads',
					'data': [buffer, p_in_size],
				});
				node.port.onmessage = function (event) {
					if (!FADEAudioWorklet.worklet) {
						return;
					}
					if (event.data['cmd'] === 'read') {
						const read = event.data['data'];
						FADEAudioWorklet.ring_buffer.consumed(
							read,
							FADEAudioWorklet.worklet.port
						);
					} else if (event.data['cmd'] === 'input') {
						const buf = event.data['data'];
						if (buf.length > p_in_size) {
							FADERuntime.error('Input chunk is too big');
							return;
						}
						FADEAudioWorklet.ring_buffer.receive(buf);
					} else {
						FADERuntime.error(event.data);
					}
				};
			});
		},

		get_node: function () {
			return FADEAudioWorklet.worklet;
		},

		close: function () {
			return new Promise(function (resolve, reject) {
				if (FADEAudioWorklet.promise === null) {
					return;
				}
				const p = FADEAudioWorklet.promise;
				p.then(function () {
					FADEAudioWorklet.worklet.port.postMessage({
						'cmd': 'stop',
						'data': null,
					});
					FADEAudioWorklet.worklet.disconnect();
					FADEAudioWorklet.worklet.port.onmessage = null;
					FADEAudioWorklet.worklet = null;
					FADEAudioWorklet.promise = null;
					resolve();
				}).catch(function (err) {
					// Aborted?
					FADERuntime.error(err);
				});
			});
		},
	},

	fade_audio_worklet_create__proxy: 'sync',
	fade_audio_worklet_create__sig: 'ii',
	fade_audio_worklet_create: function (channels) {
		try {
			FADEAudioWorklet.create(channels);
		} catch (e) {
			FADERuntime.error('Error starting AudioDriverWorklet', e);
			return 1;
		}
		return 0;
	},

	fade_audio_worklet_start__proxy: 'sync',
	fade_audio_worklet_start__sig: 'viiiii',
	fade_audio_worklet_start: function (
		p_in_buf,
		p_in_size,
		p_out_buf,
		p_out_size,
		p_state
	) {
		const out_buffer = FADERuntime.heapSub(HEAPF32, p_out_buf, p_out_size);
		const in_buffer = FADERuntime.heapSub(HEAPF32, p_in_buf, p_in_size);
		const state = FADERuntime.heapSub(HEAP32, p_state, 4);
		FADEAudioWorklet.start(in_buffer, out_buffer, state);
	},

	fade_audio_worklet_start_no_threads__proxy: 'sync',
	fade_audio_worklet_start_no_threads__sig: 'viiiiii',
	fade_audio_worklet_start_no_threads: function (
		p_out_buf,
		p_out_size,
		p_out_callback,
		p_in_buf,
		p_in_size,
		p_in_callback
	) {
		const out_callback = FADERuntime.get_func(p_out_callback);
		const in_callback = FADERuntime.get_func(p_in_callback);
		FADEAudioWorklet.start_no_threads(
			p_out_buf,
			p_out_size,
			out_callback,
			p_in_buf,
			p_in_size,
			in_callback
		);
	},

	fade_audio_worklet_state_wait__sig: 'iiii',
	fade_audio_worklet_state_wait: function (
		p_state,
		p_idx,
		p_expected,
		p_timeout
	) {
		Atomics.wait(HEAP32, (p_state >> 2) + p_idx, p_expected, p_timeout);
		return Atomics.load(HEAP32, (p_state >> 2) + p_idx);
	},

	fade_audio_worklet_state_add__sig: 'iiii',
	fade_audio_worklet_state_add: function (p_state, p_idx, p_value) {
		return Atomics.add(HEAP32, (p_state >> 2) + p_idx, p_value);
	},

	fade_audio_worklet_state_get__sig: 'iii',
	fade_audio_worklet_state_get: function (p_state, p_idx) {
		return Atomics.load(HEAP32, (p_state >> 2) + p_idx);
	},
};

autoAddDeps(FADEAudioWorklet, '$FADEAudioWorklet');
mergeInto(LibraryManager.library, FADEAudioWorklet);

/*
 * The ScriptProcessorNode API, used as a fallback if AudioWorklet is not available.
 */
const FADEAudioScript = {
	$FADEAudioScript__deps: ['$FADEAudio'],
	$FADEAudioScript: {
		script: null,

		create: function (buffer_length, channel_count) {
			FADEAudioScript.script = FADEAudio.ctx.createScriptProcessor(
				buffer_length,
				2,
				channel_count
			);
			FADEAudio.driver = FADEAudioScript;
			return FADEAudioScript.script.bufferSize;
		},

		start: function (p_in_buf, p_in_size, p_out_buf, p_out_size, onprocess) {
			FADEAudioScript.script.onaudioprocess = function (event) {
				// Read input
				const inb = FADERuntime.heapSub(HEAPF32, p_in_buf, p_in_size);
				const input = event.inputBuffer;
				if (FADEAudio.input) {
					const inlen = input.getChannelData(0).length;
					for (let ch = 0; ch < 2; ch++) {
						const data = input.getChannelData(ch);
						for (let s = 0; s < inlen; s++) {
							inb[s * 2 + ch] = data[s];
						}
					}
				}

				// Let FADE process the input/output.
				onprocess();

				// Write the output.
				const outb = FADERuntime.heapSub(HEAPF32, p_out_buf, p_out_size);
				const output = event.outputBuffer;
				const channels = output.numberOfChannels;
				for (let ch = 0; ch < channels; ch++) {
					const data = output.getChannelData(ch);
					// Loop through samples and assign computed values.
					for (let sample = 0; sample < data.length; sample++) {
						data[sample] = outb[sample * channels + ch];
					}
				}
			};
			FADEAudioScript.script.connect(FADEAudio.ctx.destination);
		},

		get_node: function () {
			return FADEAudioScript.script;
		},

		close: function () {
			return new Promise(function (resolve, reject) {
				FADEAudioScript.script.disconnect();
				FADEAudioScript.script.onaudioprocess = function () {};
				FADEAudioScript.script = null;
				resolve();
			});
		},
	},

	fade_audio_script_create__proxy: 'sync',
	fade_audio_script_create__sig: 'iii',
	fade_audio_script_create: function (buffer_length, channel_count) {
		const buf_len = FADERuntime.getHeapValue(buffer_length, 'i32');
		try {
			const out_len = FADEAudioScript.create(buf_len, channel_count);
			FADERuntime.setHeapValue(buffer_length, out_len, 'i32');
		} catch (e) {
			FADERuntime.error('Error starting AudioDriverScriptProcessor', e);
			return 1;
		}
		return 0;
	},

	fade_audio_script_start__proxy: 'sync',
	fade_audio_script_start__sig: 'viiiii',
	fade_audio_script_start: function (
		p_in_buf,
		p_in_size,
		p_out_buf,
		p_out_size,
		p_cb
	) {
		const onprocess = FADERuntime.get_func(p_cb);
		FADEAudioScript.start(
			p_in_buf,
			p_in_size,
			p_out_buf,
			p_out_size,
			onprocess
		);
	},
};

autoAddDeps(FADEAudioScript, '$FADEAudioScript');
mergeInto(LibraryManager.library, FADEAudioScript);
