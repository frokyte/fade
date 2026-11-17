/**************************************************************************/
/*  library_fade_input.js                                                */
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

/*
 * IME API helper.
 */

const FADEIME = {
	$FADEIME__deps: ['$FADERuntime', '$FADEEventListeners'],
	$FADEIME__postset: 'FADEOS.atexit(function(resolve, reject) { FADEIME.clear(); resolve(); });',
	$FADEIME: {
		ime: null,
		active: false,
		focusTimerIntervalId: -1,

		getModifiers: function (evt) {
			return (evt.shiftKey + 0) + ((evt.altKey + 0) << 1) + ((evt.ctrlKey + 0) << 2) + ((evt.metaKey + 0) << 3);
		},

		ime_active: function (active) {
			function clearFocusTimerInterval() {
				clearInterval(FADEIME.focusTimerIntervalId);
				FADEIME.focusTimerIntervalId = -1;
			}

			function focusTimer() {
				if (FADEIME.ime == null) {
					clearFocusTimerInterval();
					return;
				}
				FADEIME.ime.focus();
			}

			if (FADEIME.focusTimerIntervalId > -1) {
				clearFocusTimerInterval();
			}

			if (FADEIME.ime == null) {
				return;
			}

			FADEIME.active = active;
			if (active) {
				FADEIME.ime.style.display = 'block';
				FADEIME.focusTimerIntervalId = setInterval(focusTimer, 100);
			} else {
				FADEIME.ime.style.display = 'none';
				FADEConfig.canvas.focus();
			}
		},

		ime_position: function (x, y) {
			if (FADEIME.ime == null) {
				return;
			}
			const canvas = FADEConfig.canvas;
			const rect = canvas.getBoundingClientRect();
			const rw = canvas.width / rect.width;
			const rh = canvas.height / rect.height;
			const clx = (x / rw) + rect.x;
			const cly = (y / rh) + rect.y;

			FADEIME.ime.style.left = `${clx}px`;
			FADEIME.ime.style.top = `${cly}px`;
		},

		init: function (ime_cb, key_cb, code, key) {
			function key_event_cb(pressed, evt) {
				const modifiers = FADEIME.getModifiers(evt);
				FADERuntime.stringToHeap(evt.code, code, 32);
				FADERuntime.stringToHeap(evt.key, key, 32);
				key_cb(pressed, evt.repeat, modifiers);
				evt.preventDefault();
			}
			function ime_event_cb(event) {
				if (FADEIME.ime == null) {
					return;
				}
				switch (event.type) {
				case 'compositionstart':
					ime_cb(0, null);
					FADEIME.ime.innerHTML = '';
					break;
				case 'compositionupdate': {
					const ptr = FADERuntime.allocString(event.data);
					ime_cb(1, ptr);
					FADERuntime.free(ptr);
				} break;
				case 'compositionend': {
					const ptr = FADERuntime.allocString(event.data);
					ime_cb(2, ptr);
					FADERuntime.free(ptr);
					FADEIME.ime.innerHTML = '';
				} break;
				default:
					// Do nothing.
				}
			}

			const ime = document.createElement('div');
			ime.className = 'ime';
			ime.style.background = 'none';
			ime.style.opacity = 0.0;
			ime.style.position = 'fixed';
			ime.style.textAlign = 'left';
			ime.style.fontSize = '1px';
			ime.style.left = '0px';
			ime.style.top = '0px';
			ime.style.width = '100%';
			ime.style.height = '40px';
			ime.style.pointerEvents = 'none';
			ime.style.display = 'none';
			ime.contentEditable = 'true';

			FADEEventListeners.add(ime, 'compositionstart', ime_event_cb, false);
			FADEEventListeners.add(ime, 'compositionupdate', ime_event_cb, false);
			FADEEventListeners.add(ime, 'compositionend', ime_event_cb, false);
			FADEEventListeners.add(ime, 'keydown', key_event_cb.bind(null, 1), false);
			FADEEventListeners.add(ime, 'keyup', key_event_cb.bind(null, 0), false);

			ime.onblur = function () {
				this.style.display = 'none';
				FADEConfig.canvas.focus();
				FADEIME.active = false;
			};

			FADEConfig.canvas.parentElement.appendChild(ime);
			FADEIME.ime = ime;
		},

		clear: function () {
			if (FADEIME.ime == null) {
				return;
			}
			if (FADEIME.focusTimerIntervalId > -1) {
				clearInterval(FADEIME.focusTimerIntervalId);
				FADEIME.focusTimerIntervalId = -1;
			}
			FADEIME.ime.remove();
			FADEIME.ime = null;
		},
	},
};
mergeInto(LibraryManager.library, FADEIME);

/*
 * Gamepad API helper.
 */
const FADEInputGamepads = {
	$FADEInputGamepads__deps: ['$FADERuntime', '$FADEEventListeners'],
	$FADEInputGamepads: {
		samples: [],

		get_pads: function () {
			try {
				// Will throw in iframe when permission is denied.
				// Will throw/warn in the future for insecure contexts.
				// See https://github.com/w3c/gamepad/pull/120
				const pads = navigator.getGamepads();
				if (pads) {
					return pads;
				}
				return [];
			} catch (e) {
				return [];
			}
		},

		get_samples: function () {
			return FADEInputGamepads.samples;
		},

		get_sample: function (index) {
			const samples = FADEInputGamepads.samples;
			return index < samples.length ? samples[index] : null;
		},

		sample: function () {
			const pads = FADEInputGamepads.get_pads();
			const samples = [];
			for (let i = 0; i < pads.length; i++) {
				const pad = pads[i];
				if (!pad) {
					samples.push(null);
					continue;
				}
				const s = {
					standard: pad.mapping === 'standard',
					buttons: [],
					axes: [],
					connected: pad.connected,
				};
				for (let b = 0; b < pad.buttons.length; b++) {
					s.buttons.push(pad.buttons[b].value);
				}
				for (let a = 0; a < pad.axes.length; a++) {
					s.axes.push(pad.axes[a]);
				}
				samples.push(s);
			}
			FADEInputGamepads.samples = samples;
		},

		init: function (onchange) {
			FADEInputGamepads.samples = [];
			function add(pad) {
				const guid = FADEInputGamepads.get_guid(pad);
				const c_id = FADERuntime.allocString(pad.id);
				const c_guid = FADERuntime.allocString(guid);
				onchange(pad.index, 1, c_id, c_guid);
				FADERuntime.free(c_id);
				FADERuntime.free(c_guid);
			}
			const pads = FADEInputGamepads.get_pads();
			for (let i = 0; i < pads.length; i++) {
				// Might be reserved space.
				if (pads[i]) {
					add(pads[i]);
				}
			}
			FADEEventListeners.add(window, 'gamepadconnected', function (evt) {
				if (evt.gamepad) {
					add(evt.gamepad);
				}
			}, false);
			FADEEventListeners.add(window, 'gamepaddisconnected', function (evt) {
				if (evt.gamepad) {
					onchange(evt.gamepad.index, 0);
				}
			}, false);
		},

		get_guid: function (pad) {
			if (pad.mapping) {
				return pad.mapping;
			}
			const ua = navigator.userAgent;
			let os = 'Unknown';
			if (ua.indexOf('Android') >= 0) {
				os = 'Android';
			} else if (ua.indexOf('Linux') >= 0) {
				os = 'Linux';
			} else if (ua.indexOf('iPhone') >= 0) {
				os = 'iOS';
			} else if (ua.indexOf('Macintosh') >= 0) {
				// Updated iPads will fall into this category.
				os = 'MacOSX';
			} else if (ua.indexOf('Windows') >= 0) {
				os = 'Windows';
			}

			const id = pad.id;
			// Chrom* style: NAME (Vendor: xxxx Product: xxxx).
			const exp1 = /vendor: ([0-9a-f]{4}) product: ([0-9a-f]{4})/i;
			// Firefox/Safari style (Safari may remove leading zeroes).
			const exp2 = /^([0-9a-f]+)-([0-9a-f]+)-/i;
			let vendor = '';
			let product = '';
			if (exp1.test(id)) {
				const match = exp1.exec(id);
				vendor = match[1].padStart(4, '0');
				product = match[2].padStart(4, '0');
			} else if (exp2.test(id)) {
				const match = exp2.exec(id);
				vendor = match[1].padStart(4, '0');
				product = match[2].padStart(4, '0');
			}
			if (!vendor || !product) {
				return `${os}Unknown`;
			}
			return os + vendor + product;
		},
	},
};
mergeInto(LibraryManager.library, FADEInputGamepads);

/*
 * Drag and drop helper.
 * This is pretty big, but basically detect dropped files on FADEConfig.canvas,
 * process them one by one (recursively for directories), and copies them to
 * the temporary FS path '/tmp/drop-[random]/' so it can be emitted as a fade
 * event (that requires a string array of paths).
 *
 * NOTE: The temporary files are removed after the callback. This means that
 * deferred callbacks won't be able to access the files.
 */
const FADEInputDragDrop = {
	$FADEInputDragDrop__deps: ['$FS', '$FADEFS'],
	$FADEInputDragDrop: {
		MAX_DROP_SIZE: 200 * 1024 * 1024,
		create_drop: function () {
			const drop = {};
			drop.pending = [];
			drop.dirs = [];
			drop.files = [];
			drop.size = 0;
			drop.add_entry = function (entry, path) {
				if (entry.isDirectory) {
					drop.add_dir(entry, path);
				} else if (entry.isFile) {
					drop.add_file(entry, path);
				} else {
					FADERuntime.error('Unrecognized entry...', entry);
				}
			};
			drop.add_dir = function (entry, path) {
				const next = `${path}/${entry.name}`;
				drop.pending.push(() => new Promise(function (resolve, reject) {
					const reader = entry.createReader();
					reader.readEntries(function (entries) {
						drop.dirs.push(next);
						for (let i = 0; i < entries.length; i++) {
							drop.add_entry(entries[i], next);
						}
						resolve();
					});
				}));
			};
			drop.add_file = function (entry, path) {
				drop.pending.push(() => new Promise(function (resolve, reject) {
					entry.file(function (file) {
						drop.size += file.size ?? 0;
						if (drop.size > FADEInputDragDrop.MAX_DROP_SIZE) {
							reject(new Error('Drop size limit reached.'));
							return;
						}
						const reader = new FileReader();
						reader.onload = function () {
							const f = {
								'path': `${path}/${file.name}`,
								'name': file.name,
								'type': file.type,
								'size': file.size,
								'data': reader.result,
							};
							drop.files.push(f);
							resolve();
						};
						reader.onerror = function (err) {
							reject(err);
						};
						reader.readAsArrayBuffer(file);
					}, function (err) {
						reject(err);
					});
				}));
			};
			return drop;
		},

		process: function (drop) {
			if (drop.pending.length === 0) {
				return Promise.resolve();
			}
			// Execute pending operations.
			return drop.pending.splice(0).reduce(function (a, b) {
				return a.then(b);
			}, Promise.resolve()).then(function (result) {
				// Will check if new operations have been queued or resolve.
				return FADEInputDragDrop.process(drop);
			});
		},

		_process_event: function (ev, callback) {
			ev.preventDefault();
			const ROOT = `/tmp/drop-${parseInt(Math.random() * (1 << 30), 10)}`;
			const drop = FADEInputDragDrop.create_drop();
			if (ev.dataTransfer.items) {
				// Use DataTransferItemList interface to access the file(s)
				for (let i = 0; i < ev.dataTransfer.items.length; i++) {
					const item = ev.dataTransfer.items[i];
					let entry = null;
					if ('getAsEntry' in item) {
						entry = item.getAsEntry();
					} else if ('webkitGetAsEntry' in item) {
						entry = item.webkitGetAsEntry();
					}
					if (entry) {
						drop.add_entry(entry, ROOT);
					}
				}
			} else {
				FADERuntime.error('File upload not supported');
			}
			FADEInputDragDrop.process(drop).then(function () {
				const out = [ROOT];
				drop.dirs.forEach((path) => {
					FS.mkdirTree(path);
					// Add dirs to the string array if in the drop root.
					const idx = path.indexOf('/', ROOT.length + 1);
					if (idx === -1) {
						out.push(path);
					}
				});
				drop.files.forEach((elem) => {
					const path = elem['path'];
					FADEFS.copy_to_fs(path, elem['data']);
					// Add file to the string array if in the drop root.
					const idx = path.indexOf('/', ROOT.length + 1);
					if (idx === -1) {
						out.push(path);
					}
				});
				callback(out);
			}).catch(function (err) {
				FADERuntime.error(err);
			});
		},

		handler: function (callback) {
			return function (ev) {
				FADEInputDragDrop._process_event(ev, callback);
			};
		},
	},
};
mergeInto(LibraryManager.library, FADEInputDragDrop);

/*
 * FADE exposed input functions.
 */
const FADEInput = {
	$FADEInput__deps: ['$FADERuntime', '$FADEConfig', '$FADEEventListeners', '$FADEInputGamepads', '$FADEInputDragDrop', '$FADEIME'],
	$FADEInput: {
		inputKeyCallback: null,
		setInputKeyData: null,

		getModifiers: function (evt) {
			return (evt.shiftKey + 0) + ((evt.altKey + 0) << 1) + ((evt.ctrlKey + 0) << 2) + ((evt.metaKey + 0) << 3);
		},

		computePosition: function (evt, rect) {
			const canvas = FADEConfig.canvas;
			const rw = canvas.width / rect.width;
			const rh = canvas.height / rect.height;
			const x = (evt.clientX - rect.x) * rw;
			const y = (evt.clientY - rect.y) * rh;
			return [x, y];
		},

		onKeyEvent: function (pIsPressed, pEvent) {
			if (FADEInput.inputKeyCallback == null) {
				throw new TypeError('FADEInput.onKeyEvent(): FADEInput.inputKeyCallback is null, cannot process key event.');
			}
			if (FADEInput.setInputKeyData == null) {
				throw new TypeError('FADEInput.onKeyEvent(): FADEInput.setInputKeyData is null, cannot process key event.');
			}

			const modifiers = FADEInput.getModifiers(pEvent);
			FADEInput.setInputKeyData(pEvent.code, pEvent.key);
			FADEInput.inputKeyCallback(pIsPressed ? 1 : 0, pEvent.repeat, modifiers);
			pEvent.preventDefault();
		},
	},

	/*
	 * Mouse API
	 */
	fade_js_input_mouse_move_cb__proxy: 'sync',
	fade_js_input_mouse_move_cb__sig: 'vi',
	fade_js_input_mouse_move_cb: function (callback) {
		const func = FADERuntime.get_func(callback);
		const canvas = FADEConfig.canvas;
		function move_cb(evt) {
			const rect = canvas.getBoundingClientRect();
			const pos = FADEInput.computePosition(evt, rect);
			// Scale movement
			const rw = canvas.width / rect.width;
			const rh = canvas.height / rect.height;
			const rel_pos_x = evt.movementX * rw;
			const rel_pos_y = evt.movementY * rh;
			const modifiers = FADEInput.getModifiers(evt);
			func(pos[0], pos[1], rel_pos_x, rel_pos_y, modifiers, evt.pressure);
		}
		FADEEventListeners.add(window, 'pointermove', move_cb, false);
	},

	fade_js_input_mouse_wheel_cb__proxy: 'sync',
	fade_js_input_mouse_wheel_cb__sig: 'vi',
	fade_js_input_mouse_wheel_cb: function (callback) {
		const func = FADERuntime.get_func(callback);
		function wheel_cb(evt) {
			if (func(evt.deltaMode, evt.deltaX ?? 0, evt.deltaY ?? 0)) {
				evt.preventDefault();
			}
		}
		FADEEventListeners.add(FADEConfig.canvas, 'wheel', wheel_cb, false);
	},

	fade_js_input_mouse_button_cb__proxy: 'sync',
	fade_js_input_mouse_button_cb__sig: 'vi',
	fade_js_input_mouse_button_cb: function (callback) {
		const func = FADERuntime.get_func(callback);
		const canvas = FADEConfig.canvas;
		function button_cb(p_pressed, evt) {
			const rect = canvas.getBoundingClientRect();
			const pos = FADEInput.computePosition(evt, rect);
			const modifiers = FADEInput.getModifiers(evt);
			// Since the event is consumed, focus manually.
			// NOTE: The iframe container may not have focus yet, so focus even when already active.
			if (p_pressed) {
				FADEConfig.canvas.focus();
			}
			if (func(p_pressed, evt.button, pos[0], pos[1], modifiers)) {
				evt.preventDefault();
			}
		}
		FADEEventListeners.add(canvas, 'mousedown', button_cb.bind(null, 1), false);
		FADEEventListeners.add(window, 'mouseup', button_cb.bind(null, 0), false);
	},

	/*
	 * Touch API
	 */
	fade_js_input_touch_cb__proxy: 'sync',
	fade_js_input_touch_cb__sig: 'viii',
	fade_js_input_touch_cb: function (callback, ids, coords) {
		const func = FADERuntime.get_func(callback);
		const canvas = FADEConfig.canvas;
		function touch_cb(type, evt) {
			// Since the event is consumed, focus manually.
			// NOTE: The iframe container may not have focus yet, so focus even when already active.
			if (type === 0) {
				FADEConfig.canvas.focus();
			}
			const rect = canvas.getBoundingClientRect();
			const touches = evt.changedTouches;
			for (let i = 0; i < touches.length; i++) {
				const touch = touches[i];
				const pos = FADEInput.computePosition(touch, rect);
				FADERuntime.setHeapValue(coords + (i * 2) * 8, pos[0], 'double');
				FADERuntime.setHeapValue(coords + (i * 2 + 1) * 8, pos[1], 'double');
				FADERuntime.setHeapValue(ids + i * 4, touch.identifier, 'i32');
			}
			func(type, touches.length);
			if (evt.cancelable) {
				evt.preventDefault();
			}
		}
		FADEEventListeners.add(canvas, 'touchstart', touch_cb.bind(null, 0), false);
		FADEEventListeners.add(canvas, 'touchend', touch_cb.bind(null, 1), false);
		FADEEventListeners.add(canvas, 'touchcancel', touch_cb.bind(null, 1), false);
		FADEEventListeners.add(canvas, 'touchmove', touch_cb.bind(null, 2), false);
	},

	/*
	 * Key API
	 */
	fade_js_input_key_cb__proxy: 'sync',
	fade_js_input_key_cb__sig: 'viii',
	fade_js_input_key_cb: function (pCallback, pCodePtr, pKeyPtr) {
		FADEInput.inputKeyCallback = FADERuntime.get_func(pCallback);
		FADEInput.setInputKeyData = (pCode, pKey) => {
			FADERuntime.stringToHeap(pCode, pCodePtr, 32);
			FADERuntime.stringToHeap(pKey, pKeyPtr, 32);
		};
		FADEEventListeners.add(FADEConfig.canvas, 'keydown', FADEInput.onKeyEvent.bind(null, true), false);
		FADEEventListeners.add(FADEConfig.canvas, 'keyup', FADEInput.onKeyEvent.bind(null, false), false);
	},

	/*
	 * IME API
	 */
	fade_js_set_ime_active__proxy: 'sync',
	fade_js_set_ime_active__sig: 'vi',
	fade_js_set_ime_active: function (p_active) {
		FADEIME.ime_active(p_active);
	},

	fade_js_set_ime_position__proxy: 'sync',
	fade_js_set_ime_position__sig: 'vii',
	fade_js_set_ime_position: function (p_x, p_y) {
		FADEIME.ime_position(p_x, p_y);
	},

	fade_js_set_ime_cb__proxy: 'sync',
	fade_js_set_ime_cb__sig: 'viiii',
	fade_js_set_ime_cb: function (p_ime_cb, p_key_cb, code, key) {
		const ime_cb = FADERuntime.get_func(p_ime_cb);
		const key_cb = FADERuntime.get_func(p_key_cb);
		FADEIME.init(ime_cb, key_cb, code, key);
	},

	fade_js_is_ime_focused__proxy: 'sync',
	fade_js_is_ime_focused__sig: 'i',
	fade_js_is_ime_focused: function () {
		return FADEIME.active;
	},

	/*
	 * Gamepad API
	 */
	fade_js_input_gamepad_cb__proxy: 'sync',
	fade_js_input_gamepad_cb__sig: 'vi',
	fade_js_input_gamepad_cb: function (change_cb) {
		const onchange = FADERuntime.get_func(change_cb);
		FADEInputGamepads.init(onchange);
	},

	fade_js_input_gamepad_sample_count__proxy: 'sync',
	fade_js_input_gamepad_sample_count__sig: 'i',
	fade_js_input_gamepad_sample_count: function () {
		return FADEInputGamepads.get_samples().length;
	},

	fade_js_input_gamepad_sample__proxy: 'sync',
	fade_js_input_gamepad_sample__sig: 'i',
	fade_js_input_gamepad_sample: function () {
		FADEInputGamepads.sample();
		return 0;
	},

	fade_js_input_gamepad_sample_get__proxy: 'sync',
	fade_js_input_gamepad_sample_get__sig: 'iiiiiii',
	fade_js_input_gamepad_sample_get: function (p_index, r_btns, r_btns_num, r_axes, r_axes_num, r_standard) {
		const sample = FADEInputGamepads.get_sample(p_index);
		if (!sample || !sample.connected) {
			return 1;
		}
		const btns = sample.buttons;
		const btns_len = btns.length < 16 ? btns.length : 16;
		for (let i = 0; i < btns_len; i++) {
			FADERuntime.setHeapValue(r_btns + (i << 2), btns[i], 'float');
		}
		FADERuntime.setHeapValue(r_btns_num, btns_len, 'i32');
		const axes = sample.axes;
		const axes_len = axes.length < 10 ? axes.length : 10;
		for (let i = 0; i < axes_len; i++) {
			FADERuntime.setHeapValue(r_axes + (i << 2), axes[i], 'float');
		}
		FADERuntime.setHeapValue(r_axes_num, axes_len, 'i32');
		const is_standard = sample.standard ? 1 : 0;
		FADERuntime.setHeapValue(r_standard, is_standard, 'i32');
		return 0;
	},

	/*
	 * Drag/Drop API
	 */
	fade_js_input_drop_files_cb__proxy: 'sync',
	fade_js_input_drop_files_cb__sig: 'vi',
	fade_js_input_drop_files_cb: function (callback) {
		const func = FADERuntime.get_func(callback);
		const dropFiles = function (files) {
			const args = files || [];
			if (!args.length) {
				return;
			}
			const argc = args.length;
			const argv = FADERuntime.allocStringArray(args);
			func(argv, argc);
			FADERuntime.freeStringArray(argv, argc);
		};
		const canvas = FADEConfig.canvas;
		FADEEventListeners.add(canvas, 'dragover', function (ev) {
			// Prevent default behavior (which would try to open the file(s))
			ev.preventDefault();
		}, false);
		FADEEventListeners.add(canvas, 'drop', FADEInputDragDrop.handler(dropFiles));
	},

	/* Paste API */
	fade_js_input_paste_cb__proxy: 'sync',
	fade_js_input_paste_cb__sig: 'vi',
	fade_js_input_paste_cb: function (callback) {
		const func = FADERuntime.get_func(callback);
		FADEEventListeners.add(window, 'paste', function (evt) {
			const text = evt.clipboardData.getData('text');
			const ptr = FADERuntime.allocString(text);
			func(ptr);
			FADERuntime.free(ptr);
		}, false);
	},

	fade_js_input_vibrate_handheld__proxy: 'sync',
	fade_js_input_vibrate_handheld__sig: 'vi',
	fade_js_input_vibrate_handheld: function (p_duration_ms) {
		if (typeof navigator.vibrate !== 'function') {
			FADERuntime.print('This browser does not support vibration.');
		} else {
			navigator.vibrate(p_duration_ms);
		}
	},
};

autoAddDeps(FADEInput, '$FADEInput');
mergeInto(LibraryManager.library, FADEInput);
