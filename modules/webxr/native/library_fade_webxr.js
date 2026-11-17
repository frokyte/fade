/**************************************************************************/
/*  library_fade_webxr.js                                                */
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

const FADEWebXR = {
	$FADEWebXR__deps: ['$MainLoop', '$GL', '$FADERuntime', '$runtimeKeepalivePush', '$runtimeKeepalivePop'],
	$FADEWebXR: {
		gl: null,

		session: null,
		gl_binding: null,
		disable_webxr_layers: false,
		layer: null,
		space: null,
		frame: null,
		pose: null,
		view_count: 1,
		input_sources: new Array(16),
		touches: new Array(5),
		onsimpleevent: null,

		// Monkey-patch the requestAnimationFrame() used by Emscripten for the main
		// loop, so that we can swap it out for XRSession.requestAnimationFrame()
		// when an XR session is started.
		orig_requestAnimationFrame: null,
		requestAnimationFrame: (callback) => {
			if (FADEWebXR.session && FADEWebXR.space) {
				const onFrame = function (time, frame) {
					FADEWebXR.frame = frame;
					FADEWebXR.pose = frame.getViewerPose(FADEWebXR.space);
					callback(time);
					FADEWebXR.frame = null;
					FADEWebXR.pose = null;
				};
				FADEWebXR.session.requestAnimationFrame(onFrame);
			} else {
				FADEWebXR.orig_requestAnimationFrame(callback);
			}
		},
		monkeyPatchRequestAnimationFrame: (enable) => {
			if (FADEWebXR.orig_requestAnimationFrame === null) {
				FADEWebXR.orig_requestAnimationFrame = MainLoop.requestAnimationFrame;
			}
			MainLoop.requestAnimationFrame = enable
				? FADEWebXR.requestAnimationFrame
				: FADEWebXR.orig_requestAnimationFrame;
		},
		pauseResumeMainLoop: () => {
			// Once both FADEWebXR.session and FADEWebXR.space are set or
			// unset, our monkey-patched requestAnimationFrame() should be
			// enabled or disabled. When using the WebXR API Emulator, this
			// gets picked up automatically, however, in the Oculus Browser
			// on the Quest, we need to pause and resume the main loop.
			MainLoop.pause();
			runtimeKeepalivePush();
			window.setTimeout(function () {
				runtimeKeepalivePop();
				MainLoop.resume();
			}, 0);
		},

		getLayer: () => {
			const new_view_count = (FADEWebXR.pose) ? FADEWebXR.pose.views.length : 1;
			let layer = FADEWebXR.layer;

			if (layer && (FADEWebXR.disable_webxr_layers || FADEWebXR.view_count === new_view_count)) {
				return layer;
			}

			if (!FADEWebXR.session) {
				return null;
			}

			const gl = FADEWebXR.gl;

			if (FADEWebXR.disable_webxr_layers) {
				layer = new XRWebGLLayer(FADEWebXR.session, gl, {
					antialias: false,
				});
				FADEWebXR.session.updateRenderState({ baseLayer: layer });
			} else {
				if (!FADEWebXR.gl_binding || !FADEWebXR.gl_binding.createProjectionLayer) {
					return null;
				}

				layer = FADEWebXR.gl_binding.createProjectionLayer({
					textureType: new_view_count > 1 ? 'texture-array' : 'texture',
					colorFormat: gl.RGBA8,
					depthFormat: gl.DEPTH_COMPONENT24,
				});
				FADEWebXR.session.updateRenderState({ layers: [layer] });
			}

			FADEWebXR.layer = layer;
			FADEWebXR.view_count = new_view_count;
			return layer;
		},

		getSubImage: () => {
			if (!FADEWebXR.pose || FADEWebXR.disable_webxr_layers) {
				return null;
			}
			const layer = FADEWebXR.getLayer();
			if (layer === null) {
				return null;
			}

			// Because we always use "texture-array" for multiview and "texture"
			// when there is only 1 view, it should be safe to only grab the
			// subimage for the first view.
			return FADEWebXR.gl_binding.getViewSubImage(layer, FADEWebXR.pose.views[0]);
		},

		getViewport: () => {
			if (!FADEWebXR.pose) {
				return null;
			}
			if (FADEWebXR.disable_webxr_layers) {
				const layer = FADEWebXR.getLayer();
				if (layer === null) {
					return null;
				}
				return layer.getViewport(FADEWebXR.pose.views[0]);
			}
			const subimage = FADEWebXR.getSubImage();
			if (subimage === null) {
				return null;
			}
			return subimage.viewport;
		},

		// Some custom WebGL code for blitting our eye textures to the
		// framebuffer we get from WebXR.
		shaderPrograms: {},
		programInfos: {},
		buffer: null,
		sampler: null,
		// Vertex shader source.
		vsSource: `#version 300 es
			const vec2 scale = vec2(0.5, -0.5);
			const vec2 offset = vec2(0.5, 0.5);
			in vec4 aVertexPosition;

			out highp vec2 vTextureCoord;

			void main () {
				gl_Position = aVertexPosition;
				vTextureCoord = aVertexPosition.xy * scale + offset;
			}
		`,
		// Fragment shader source.
		fsSource: `#version 300 es
			precision highp float;

			in highp vec2 vTextureCoord;

			uniform sampler2D uSampler;

			out vec4 fragColor;

			void main() {
				fragColor = texture(uSampler, vTextureCoord);
			}
		`,
		// Fragment shader source for texture arrays (one layer per view).
		fsSourceArray: `#version 300 es
			precision highp float;
			precision highp sampler2DArray;

			in highp vec2 vTextureCoord;

			uniform sampler2DArray uSampler;
			uniform float uLayer;

			out vec4 fragColor;

			void main() {
				fragColor = texture(uSampler, vec3(vTextureCoord, uLayer));
			}
		`,

		initShaderProgram: (gl, vsSource, fsSource) => {
			const vertexShader = FADEWebXR.loadShader(gl, gl.VERTEX_SHADER, vsSource);
			const fragmentShader = FADEWebXR.loadShader(gl, gl.FRAGMENT_SHADER, fsSource);

			const shaderProgram = gl.createProgram();
			gl.attachShader(shaderProgram, vertexShader);
			gl.attachShader(shaderProgram, fragmentShader);
			gl.linkProgram(shaderProgram);

			if (!gl.getProgramParameter(shaderProgram, gl.LINK_STATUS)) {
				FADERuntime.error(`Unable to initialize the shader program: ${gl.getProgramInfoLog(shaderProgram)}`);
				return null;
			}

			return shaderProgram;
		},
		loadShader: (gl, type, source) => {
			const shader = gl.createShader(type);
			gl.shaderSource(shader, source);
			gl.compileShader(shader);

			if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
				FADERuntime.error(`An error occurred compiling the shader: ${gl.getShaderInfoLog(shader)}`);
				gl.deleteShader(shader);
				return null;
			}

			return shader;
		},
		initBuffer: (gl) => {
			const positionBuffer = gl.createBuffer();
			gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
			const positions = [
				-1.0, -1.0,
				1.0, -1.0,
				-1.0, 1.0,
				1.0, 1.0,
			];
			gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(positions), gl.STATIC_DRAW);
			return positionBuffer;
		},
		initSampler: (gl) => {
			const sampler = gl.createSampler();
			gl.samplerParameteri(sampler, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
			gl.samplerParameteri(sampler, gl.TEXTURE_MAG_FILTER, gl.LINEAR);
			gl.samplerParameteri(sampler, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
			gl.samplerParameteri(sampler, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
			return sampler;
		},
		getProgramInfo: (gl, is_array) => {
			const key = is_array ? 'array' : '2d';
			if (!FADEWebXR.shaderPrograms[key]) {
				const shaderProgram = FADEWebXR.initShaderProgram(gl, FADEWebXR.vsSource, is_array ? FADEWebXR.fsSourceArray : FADEWebXR.fsSource);
				FADEWebXR.shaderPrograms[key] = shaderProgram;
				FADEWebXR.programInfos[key] = {
					program: shaderProgram,
					attribLocations: {
						vertexPosition: gl.getAttribLocation(shaderProgram, 'aVertexPosition'),
					},
					uniformLocations: {
						uSampler: gl.getUniformLocation(shaderProgram, 'uSampler'),
						uLayer: gl.getUniformLocation(shaderProgram, 'uLayer'),
					},
				};
			}
			return FADEWebXR.programInfos[key];
		},
		blitTexture: (gl, texture, is_array, layer) => {
			const programInfo = FADEWebXR.getProgramInfo(gl, is_array);
			if (FADEWebXR.buffer === null) {
				FADEWebXR.buffer = FADEWebXR.initBuffer(gl);
			}
			if (FADEWebXR.sampler === null) {
				FADEWebXR.sampler = FADEWebXR.initSampler(gl);
			}
			const texture_target = is_array ? gl.TEXTURE_2D_ARRAY : gl.TEXTURE_2D;

			const orig_program = gl.getParameter(gl.CURRENT_PROGRAM);
			gl.useProgram(programInfo.program);

			const orig_vertex_array = gl.getParameter(gl.VERTEX_ARRAY_BINDING);
			gl.bindVertexArray(null);
			gl.bindBuffer(gl.ARRAY_BUFFER, FADEWebXR.buffer);
			gl.vertexAttribPointer(programInfo.attribLocations.vertexPosition, 2, gl.FLOAT, false, 0, 0);
			gl.enableVertexAttribArray(programInfo.attribLocations.vertexPosition);

			gl.activeTexture(gl.TEXTURE0);
			gl.bindTexture(texture_target, texture);
			gl.bindSampler(0, FADEWebXR.sampler);
			gl.uniform1i(programInfo.uniformLocations.uSampler, 0);
			if (is_array) {
				gl.uniform1f(programInfo.uniformLocations.uLayer, layer);
			}

			const blend_enabled = gl.getParameter(gl.BLEND);
			if (blend_enabled) {
				gl.disable(gl.BLEND);
			}
			const depth_test_enabled = gl.getParameter(gl.DEPTH_TEST);
			if (depth_test_enabled) {
				gl.disable(gl.DEPTH_TEST);
			}
			const cull_face_enabled = gl.getParameter(gl.CULL_FACE);
			if (cull_face_enabled) {
				gl.disable(gl.CULL_FACE);
			}
			const scissor_test_enabled = gl.getParameter(gl.SCISSOR_TEST);
			if (scissor_test_enabled) {
				gl.disable(gl.SCISSOR_TEST);
			}
			const stencil_test_enabled = gl.getParameter(gl.STENCIL_TEST);
			if (stencil_test_enabled) {
				gl.disable(gl.STENCIL_TEST);
			}

			gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);

			// Restore state.
			gl.bindSampler(0, null);
			gl.bindTexture(texture_target, null);
			gl.disableVertexAttribArray(programInfo.attribLocations.vertexPosition);
			gl.bindBuffer(gl.ARRAY_BUFFER, null);
			gl.bindVertexArray(orig_vertex_array);
			gl.useProgram(orig_program);
			if (blend_enabled) {
				gl.enable(gl.BLEND);
			}
			if (depth_test_enabled) {
				gl.enable(gl.DEPTH_TEST);
			}
			if (cull_face_enabled) {
				gl.enable(gl.CULL_FACE);
			}
			if (scissor_test_enabled) {
				gl.enable(gl.SCISSOR_TEST);
			}
			if (stencil_test_enabled) {
				gl.enable(gl.STENCIL_TEST);
			}
		},

		getTextureId: (texture) => {
			if (texture.name !== undefined) {
				return texture.name;
			}

			const id = GL.getNewId(GL.textures);
			texture.name = id;
			GL.textures[id] = texture;

			return id;
		},

		addInputSource: (input_source) => {
			let name = -1;
			if (input_source.targetRayMode === 'tracked-pointer' && input_source.handedness === 'left') {
				name = 0;
			} else if (input_source.targetRayMode === 'tracked-pointer' && input_source.handedness === 'right') {
				name = 1;
			} else {
				for (let i = 2; i < 16; i++) {
					if (!FADEWebXR.input_sources[i]) {
						name = i;
						break;
					}
				}
			}
			if (name >= 0) {
				FADEWebXR.input_sources[name] = input_source;
				input_source.name = name;

				// Find a free touch index for screen sources.
				if (input_source.targetRayMode === 'screen') {
					let touch_index = -1;
					for (let i = 0; i < 5; i++) {
						if (!FADEWebXR.touches[i]) {
							touch_index = i;
							break;
						}
					}
					if (touch_index >= 0) {
						FADEWebXR.touches[touch_index] = input_source;
						input_source.touch_index = touch_index;
					}
				}
			}
			return name;
		},

		removeInputSource: (input_source) => {
			if (input_source.name !== undefined) {
				const name = input_source.name;
				if (name >= 0 && name < 16) {
					FADEWebXR.input_sources[name] = null;
				}

				if (input_source.touch_index !== undefined) {
					const touch_index = input_source.touch_index;
					if (touch_index >= 0 && touch_index < 5) {
						FADEWebXR.touches[touch_index] = null;
					}
				}
				return name;
			}
			return -1;
		},

		getInputSourceId: (input_source) => {
			if (input_source !== undefined) {
				return input_source.name;
			}
			return -1;
		},

		getTouchIndex: (input_source) => {
			if (input_source.touch_index !== undefined) {
				return input_source.touch_index;
			}
			return -1;
		},
	},

	fade_webxr_is_supported__proxy: 'sync',
	fade_webxr_is_supported__sig: 'i',
	fade_webxr_is_supported: function () {
		return !!navigator.xr;
	},

	fade_webxr_is_session_supported__proxy: 'sync',
	fade_webxr_is_session_supported__sig: 'vii',
	fade_webxr_is_session_supported: function (p_session_mode, p_callback) {
		const session_mode = FADERuntime.parseString(p_session_mode);
		const cb = FADERuntime.get_func(p_callback);
		if (navigator.xr) {
			navigator.xr.isSessionSupported(session_mode).then(function (supported) {
				const c_str = FADERuntime.allocString(session_mode);
				cb(c_str, supported ? 1 : 0);
				FADERuntime.free(c_str);
			});
		} else {
			const c_str = FADERuntime.allocString(session_mode);
			cb(c_str, 0);
			FADERuntime.free(c_str);
		}
	},

	fade_webxr_initialize__deps: ['emscripten_webgl_get_current_context'],
	fade_webxr_initialize__proxy: 'sync',
	fade_webxr_initialize__sig: 'viiiiiiiiii',
	fade_webxr_initialize: function (p_session_mode, p_required_features, p_optional_features, p_requested_reference_spaces, p_disable_webxr_layers, p_on_session_started, p_on_session_ended, p_on_session_failed, p_on_input_event, p_on_simple_event) {
		FADEWebXR.monkeyPatchRequestAnimationFrame(true);

		const session_mode = FADERuntime.parseString(p_session_mode);
		const required_features = FADERuntime.parseString(p_required_features).split(',').map((s) => s.trim()).filter((s) => s !== '');
		const optional_features = FADERuntime.parseString(p_optional_features).split(',').map((s) => s.trim()).filter((s) => s !== '');
		const requested_reference_space_types = FADERuntime.parseString(p_requested_reference_spaces).split(',').map((s) => s.trim());
		const onstarted = FADERuntime.get_func(p_on_session_started);
		const onended = FADERuntime.get_func(p_on_session_ended);
		const onfailed = FADERuntime.get_func(p_on_session_failed);
		const oninputevent = FADERuntime.get_func(p_on_input_event);
		const onsimpleevent = FADERuntime.get_func(p_on_simple_event);

		let disable_webxr_layers = !!p_disable_webxr_layers;

		// Use the same check as the WebXR Layers Polyfill.
		const webxr_layers_supported = (typeof XRWebGLBinding !== 'undefined') && (typeof XRMediaBinding !== 'undefined');
		if (!disable_webxr_layers && !webxr_layers_supported) {
			console.log('WebXR: Disabling WebXR Layers because it doesn\'t appear to be supported'); // eslint-disable-line no-console
			disable_webxr_layers = true;
		}

		const session_init = {};
		if (required_features.length > 0) {
			session_init['requiredFeatures'] = required_features;
		}
		if (optional_features.length > 0) {
			session_init['optionalFeatures'] = optional_features;
		}

		navigator.xr.requestSession(session_mode, session_init).then(function (session) {
			FADEWebXR.session = session;

			session.addEventListener('end', function (evt) {
				onended();
			});

			session.addEventListener('inputsourceschange', function (evt) {
				evt.added.forEach(FADEWebXR.addInputSource);
				evt.removed.forEach(FADEWebXR.removeInputSource);
			});

			['selectstart', 'selectend', 'squeezestart', 'squeezeend'].forEach((input_event, index) => {
				session.addEventListener(input_event, function (evt) {
					// Since this happens in-between normal frames, we need to
					// grab the frame from the event in order to get poses for
					// the input sources.
					FADEWebXR.frame = evt.frame;
					oninputevent(index, FADEWebXR.getInputSourceId(evt.inputSource));
					FADEWebXR.frame = null;
				});
			});

			session.addEventListener('visibilitychange', function (evt) {
				const c_str = FADERuntime.allocString('visibility_state_changed');
				onsimpleevent(c_str);
				FADERuntime.free(c_str);
			});

			// Store onsimpleevent so we can use it later.
			FADEWebXR.onsimpleevent = onsimpleevent;

			const gl_context_handle = _emscripten_webgl_get_current_context();
			const gl = GL.getContext(gl_context_handle).GLctx;
			FADEWebXR.gl = gl;

			gl.makeXRCompatible().then(function () {
				if (!disable_webxr_layers) {
					try {
						FADEWebXR.gl_binding = new XRWebGLBinding(session, gl);
					} catch (error) {
						console.log('WebXR: Unable to create XRWebGLBinding (disabling WebXR Layers): ', error); // eslint-disable-line no-console
						disable_webxr_layers = true;
					}
				}

				if (!disable_webxr_layers && !FADEWebXR.gl_binding.createProjectionLayer) {
					console.log('WebXR: XRWebGLBinding.createProjectionLayer is missing (disabling WebXR Layers)'); // eslint-disable-line no-console
					disable_webxr_layers = true;
					FADEWebXR.gl_binding = null;
				}

				FADEWebXR.disable_webxr_layers = disable_webxr_layers;

				// This will trigger the layer to get created.
				const layer = FADEWebXR.getLayer();
				if (!layer) {
					throw new Error('Unable to create WebXR Layer.');
				}

				function onReferenceSpaceSuccess(reference_space, reference_space_type) {
					FADEWebXR.space = reference_space;

					// Using reference_space.addEventListener() crashes when
					// using the polyfill with the WebXR Emulator extension,
					// so we set the event property instead.
					reference_space.onreset = function (evt) {
						const c_str = FADERuntime.allocString('reference_space_reset');
						onsimpleevent(c_str);
						FADERuntime.free(c_str);
					};

					// Now that both FADEWebXR.session and FADEWebXR.space are
					// set, we need to pause and resume the main loop for the XR
					// main loop to kick in.
					FADEWebXR.pauseResumeMainLoop();

					// Call in setTimeout() so that errors in the onstarted()
					// callback don't bubble up here and cause FADE to try the
					// next reference space.
					window.setTimeout(function () {
						const reference_space_c_str = FADERuntime.allocString(reference_space_type);
						const enabled_features = 'enabledFeatures' in session ? Array.from(session.enabledFeatures) : [];
						const enabled_features_c_str = FADERuntime.allocString(enabled_features.join(','));
						const environment_blend_mode = 'environmentBlendMode' in session ? session.environmentBlendMode : '';
						const environment_blend_mode_c_str = FADERuntime.allocString(environment_blend_mode);
						onstarted(reference_space_c_str, enabled_features_c_str, environment_blend_mode_c_str);
						FADERuntime.free(reference_space_c_str);
						FADERuntime.free(enabled_features_c_str);
						FADERuntime.free(environment_blend_mode_c_str);
					}, 0);
				}

				function requestReferenceSpace() {
					const reference_space_type = requested_reference_space_types.shift();
					session.requestReferenceSpace(reference_space_type)
						.then((refSpace) => {
							onReferenceSpaceSuccess(refSpace, reference_space_type);
						})
						.catch(() => {
							if (requested_reference_space_types.length === 0) {
								const c_str = FADERuntime.allocString('Unable to get any of the requested reference space types');
								onfailed(c_str);
								FADERuntime.free(c_str);
							} else {
								requestReferenceSpace();
							}
						});
				}

				requestReferenceSpace();
			}).catch(function (error) {
				const c_str = FADERuntime.allocString(`Unable to make WebGL context compatible with WebXR: ${error}`);
				onfailed(c_str);
				FADERuntime.free(c_str);
			});
		}).catch(function (error) {
			const c_str = FADERuntime.allocString(`Unable to start session: ${error}`);
			onfailed(c_str);
			FADERuntime.free(c_str);
		});
	},

	fade_webxr_uninitialize__proxy: 'sync',
	fade_webxr_uninitialize__sig: 'v',
	fade_webxr_uninitialize: function () {
		if (FADEWebXR.session) {
			FADEWebXR.session.end()
				// Prevent exception when session has already ended.
				.catch((e) => { });
		}

		FADEWebXR.disable_webxr_layers = false;

		FADEWebXR.session = null;
		FADEWebXR.gl_binding = null;
		FADEWebXR.layer = null;
		FADEWebXR.space = null;
		FADEWebXR.frame = null;
		FADEWebXR.pose = null;
		FADEWebXR.view_count = 1;
		FADEWebXR.input_sources = new Array(16);
		FADEWebXR.touches = new Array(5);
		FADEWebXR.onsimpleevent = null;

		// Disable the monkey-patched window.requestAnimationFrame() and
		// pause/restart the main loop to activate it on all platforms.
		FADEWebXR.monkeyPatchRequestAnimationFrame(false);
		FADEWebXR.pauseResumeMainLoop();
	},

	fade_webxr_get_view_count__proxy: 'sync',
	fade_webxr_get_view_count__sig: 'i',
	fade_webxr_get_view_count: function () {
		if (!FADEWebXR.session || !FADEWebXR.pose) {
			return 1;
		}
		const view_count = FADEWebXR.pose.views.length;
		return view_count > 0 ? view_count : 1;
	},

	fade_webxr_get_render_target_size__proxy: 'sync',
	fade_webxr_get_render_target_size__sig: 'ii',
	fade_webxr_get_render_target_size: function (r_size) {
		const viewport = FADEWebXR.getViewport();
		if (viewport === null) {
			return false;
		}

		FADERuntime.setHeapValue(r_size + 0, viewport.width, 'i32');
		FADERuntime.setHeapValue(r_size + 4, viewport.height, 'i32');

		return true;
	},

	fade_webxr_get_transform_for_view__proxy: 'sync',
	fade_webxr_get_transform_for_view__sig: 'iii',
	fade_webxr_get_transform_for_view: function (p_view, r_transform) {
		if (!FADEWebXR.session || !FADEWebXR.pose) {
			return false;
		}

		const views = FADEWebXR.pose.views;
		let matrix;
		if (p_view >= 0) {
			matrix = views[p_view].transform.matrix;
		} else {
			// For -1 (or any other negative value) return the HMD transform.
			matrix = FADEWebXR.pose.transform.matrix;
		}

		for (let i = 0; i < 16; i++) {
			FADERuntime.setHeapValue(r_transform + (i * 4), matrix[i], 'float');
		}

		return true;
	},

	fade_webxr_get_projection_for_view__proxy: 'sync',
	fade_webxr_get_projection_for_view__sig: 'iii',
	fade_webxr_get_projection_for_view: function (p_view, r_transform) {
		if (!FADEWebXR.session || !FADEWebXR.pose) {
			return false;
		}

		const matrix = FADEWebXR.pose.views[p_view].projectionMatrix;
		for (let i = 0; i < 16; i++) {
			FADERuntime.setHeapValue(r_transform + (i * 4), matrix[i], 'float');
		}

		return true;
	},

	fade_webxr_get_color_texture__proxy: 'sync',
	fade_webxr_get_color_texture__sig: 'i',
	fade_webxr_get_color_texture: function () {
		const subimage = FADEWebXR.getSubImage();
		if (subimage === null) {
			return 0;
		}
		return FADEWebXR.getTextureId(subimage.colorTexture);
	},

	fade_webxr_get_depth_texture__proxy: 'sync',
	fade_webxr_get_depth_texture__sig: 'i',
	fade_webxr_get_depth_texture: function () {
		const subimage = FADEWebXR.getSubImage();
		if (subimage === null) {
			return 0;
		}
		if (!subimage.depthStencilTexture) {
			return 0;
		}
		return FADEWebXR.getTextureId(subimage.depthStencilTexture);
	},

	fade_webxr_get_velocity_texture__proxy: 'sync',
	fade_webxr_get_velocity_texture__sig: 'i',
	fade_webxr_get_velocity_texture: function () {
		const subimage = FADEWebXR.getSubImage();
		if (subimage === null) {
			return 0;
		}
		if (!subimage.motionVectorTexture) {
			return 0;
		}
		return FADEWebXR.getTextureId(subimage.motionVectorTexture);
	},

	fade_webxr_commit_render_target__proxy: 'sync',
	fade_webxr_commit_render_target__sig: 'vii',
	fade_webxr_commit_render_target: function (p_texture_id, p_layer_count) {
		if (!FADEWebXR.session || !FADEWebXR.pose || !FADEWebXR.disable_webxr_layers) {
			return;
		}

		const glLayer = FADEWebXR.getLayer();
		const texture = GL.textures[p_texture_id];
		if (glLayer === null || !texture) {
			return;
		}

		const gl = FADEWebXR.gl;
		const is_array = p_layer_count > 1;

		const orig_framebuffer = gl.getParameter(gl.FRAMEBUFFER_BINDING);
		const orig_viewport = gl.getParameter(gl.VIEWPORT);

		// Bind to WebXR's framebuffer.
		gl.bindFramebuffer(gl.FRAMEBUFFER, glLayer.framebuffer);

		// Drawn rather than blitted: with foveated rendering (visionOS) the reported
		// viewports are in screen space and only rasterization goes through the rate map.
		FADEWebXR.pose.views.forEach((view, view_index) => {
			const viewport = glLayer.getViewport(view);
			gl.viewport(viewport.x, viewport.y, viewport.width, viewport.height);
			FADEWebXR.blitTexture(gl, texture, is_array, view_index);
		});

		// Restore state.
		gl.bindFramebuffer(gl.FRAMEBUFFER, orig_framebuffer);
		gl.viewport(orig_viewport[0], orig_viewport[1], orig_viewport[2], orig_viewport[3]);
	},

	fade_webxr_update_input_source__proxy: 'sync',
	fade_webxr_update_input_source__sig: 'iiiiiiiiiiiiiii',
	fade_webxr_update_input_source: function (p_input_source_id, r_target_pose, r_target_ray_mode, r_touch_index, r_has_grip_pose, r_grip_pose, r_has_standard_mapping, r_button_count, r_buttons, r_axes_count, r_axes, r_has_hand_data, r_hand_joints, r_hand_radii) {
		if (!FADEWebXR.session || !FADEWebXR.frame) {
			return 0;
		}

		if (p_input_source_id < 0 || p_input_source_id >= FADEWebXR.input_sources.length || !FADEWebXR.input_sources[p_input_source_id]) {
			return false;
		}

		const input_source = FADEWebXR.input_sources[p_input_source_id];
		const frame = FADEWebXR.frame;
		const space = FADEWebXR.space;

		// Target pose.
		const target_pose = frame.getPose(input_source.targetRaySpace, space);
		if (!target_pose) {
			// This can mean that the controller lost tracking.
			return false;
		}
		const target_pose_matrix = target_pose.transform.matrix;
		for (let i = 0; i < 16; i++) {
			FADERuntime.setHeapValue(r_target_pose + (i * 4), target_pose_matrix[i], 'float');
		}

		// Target ray mode.
		let target_ray_mode = 0;
		switch (input_source.targetRayMode) {
		case 'gaze':
			target_ray_mode = 1;
			break;

		case 'tracked-pointer':
			target_ray_mode = 2;
			break;

		case 'screen':
			target_ray_mode = 3;
			break;

		default:
		}
		FADERuntime.setHeapValue(r_target_ray_mode, target_ray_mode, 'i32');

		// Touch index.
		FADERuntime.setHeapValue(r_touch_index, FADEWebXR.getTouchIndex(input_source), 'i32');

		// Grip pose.
		let has_grip_pose = false;
		if (input_source.gripSpace) {
			const grip_pose = frame.getPose(input_source.gripSpace, space);
			if (grip_pose) {
				const grip_pose_matrix = grip_pose.transform.matrix;
				for (let i = 0; i < 16; i++) {
					FADERuntime.setHeapValue(r_grip_pose + (i * 4), grip_pose_matrix[i], 'float');
				}
				has_grip_pose = true;
			}
		}
		FADERuntime.setHeapValue(r_has_grip_pose, has_grip_pose ? 1 : 0, 'i32');

		// Gamepad data (mapping, buttons and axes).
		let has_standard_mapping = false;
		let button_count = 0;
		let axes_count = 0;
		if (input_source.gamepad) {
			if (input_source.gamepad.mapping === 'xr-standard') {
				has_standard_mapping = true;
			}

			button_count = Math.min(input_source.gamepad.buttons.length, 10);
			for (let i = 0; i < button_count; i++) {
				FADERuntime.setHeapValue(r_buttons + (i * 4), input_source.gamepad.buttons[i].value, 'float');
			}

			axes_count = Math.min(input_source.gamepad.axes.length, 10);
			for (let i = 0; i < axes_count; i++) {
				FADERuntime.setHeapValue(r_axes + (i * 4), input_source.gamepad.axes[i], 'float');
			}
		}
		FADERuntime.setHeapValue(r_has_standard_mapping, has_standard_mapping ? 1 : 0, 'i32');
		FADERuntime.setHeapValue(r_button_count, button_count, 'i32');
		FADERuntime.setHeapValue(r_axes_count, axes_count, 'i32');

		// Hand tracking data.
		let has_hand_data = false;
		if (input_source.hand && r_hand_joints !== 0 && r_hand_radii !== 0) {
			const hand_joint_array = new Float32Array(25 * 16);
			const hand_radii_array = new Float32Array(25);
			if (frame.fillPoses(input_source.hand.values(), space, hand_joint_array) && frame.fillJointRadii(input_source.hand.values(), hand_radii_array)) {
				FADERuntime.heapCopy(HEAPF32, hand_joint_array, r_hand_joints);
				FADERuntime.heapCopy(HEAPF32, hand_radii_array, r_hand_radii);
				has_hand_data = true;
			}
		}
		FADERuntime.setHeapValue(r_has_hand_data, has_hand_data ? 1 : 0, 'i32');

		return true;
	},

	fade_webxr_get_visibility_state__proxy: 'sync',
	fade_webxr_get_visibility_state__sig: 'i',
	fade_webxr_get_visibility_state: function () {
		if (!FADEWebXR.session || !FADEWebXR.session.visibilityState) {
			return 0;
		}

		return FADERuntime.allocString(FADEWebXR.session.visibilityState);
	},

	fade_webxr_get_bounds_geometry__proxy: 'sync',
	fade_webxr_get_bounds_geometry__sig: 'ii',
	fade_webxr_get_bounds_geometry: function (r_points) {
		if (!FADEWebXR.space || !FADEWebXR.space.boundsGeometry) {
			return 0;
		}

		const point_count = FADEWebXR.space.boundsGeometry.length;
		if (point_count === 0) {
			return 0;
		}

		const buf = FADERuntime.malloc(point_count * 3 * 4);
		for (let i = 0; i < point_count; i++) {
			const point = FADEWebXR.space.boundsGeometry[i];
			FADERuntime.setHeapValue(buf + ((i * 3) + 0) * 4, point.x, 'float');
			FADERuntime.setHeapValue(buf + ((i * 3) + 1) * 4, point.y, 'float');
			FADERuntime.setHeapValue(buf + ((i * 3) + 2) * 4, point.z, 'float');
		}
		FADERuntime.setHeapValue(r_points, buf, 'i32');

		return point_count;
	},

	fade_webxr_get_frame_rate__proxy: 'sync',
	fade_webxr_get_frame_rate__sig: 'i',
	fade_webxr_get_frame_rate: function () {
		if (!FADEWebXR.session || FADEWebXR.session.frameRate === undefined) {
			return 0;
		}
		return FADEWebXR.session.frameRate;
	},

	fade_webxr_update_target_frame_rate__proxy: 'sync',
	fade_webxr_update_target_frame_rate__sig: 'vi',
	fade_webxr_update_target_frame_rate: function (p_frame_rate) {
		if (!FADEWebXR.session || FADEWebXR.session.updateTargetFrameRate === undefined) {
			return;
		}

		FADEWebXR.session.updateTargetFrameRate(p_frame_rate).then(() => {
			const c_str = FADERuntime.allocString('display_refresh_rate_changed');
			FADEWebXR.onsimpleevent(c_str);
			FADERuntime.free(c_str);
		});
	},

	fade_webxr_get_supported_frame_rates__proxy: 'sync',
	fade_webxr_get_supported_frame_rates__sig: 'ii',
	fade_webxr_get_supported_frame_rates: function (r_frame_rates) {
		if (!FADEWebXR.session || FADEWebXR.session.supportedFrameRates === undefined) {
			return 0;
		}

		const frame_rate_count = FADEWebXR.session.supportedFrameRates.length;
		if (frame_rate_count === 0) {
			return 0;
		}

		const buf = FADERuntime.malloc(frame_rate_count * 4);
		for (let i = 0; i < frame_rate_count; i++) {
			FADERuntime.setHeapValue(buf + (i * 4), FADEWebXR.session.supportedFrameRates[i], 'float');
		}
		FADERuntime.setHeapValue(r_frame_rates, buf, 'i32');

		return frame_rate_count;
	},

};

autoAddDeps(FADEWebXR, '$FADEWebXR');
mergeInto(LibraryManager.library, FADEWebXR);
