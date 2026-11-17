/**************************************************************************/
/*  library_fade_os.js                                                   */
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

const IDHandler = {
	$IDHandler: {
		_last_id: 0,
		_references: {},

		get: function (p_id) {
			return IDHandler._references[p_id];
		},

		add: function (p_data) {
			const id = ++IDHandler._last_id;
			IDHandler._references[id] = p_data;
			return id;
		},

		remove: function (p_id) {
			delete IDHandler._references[p_id];
		},
	},
};

autoAddDeps(IDHandler, '$IDHandler');
mergeInto(LibraryManager.library, IDHandler);

const FADEConfig = {
	$FADEConfig__postset: 'Module["initConfig"] = FADEConfig.init_config;',
	$FADEConfig__deps: ['$FADERuntime'],
	$FADEConfig: {
		canvas: null,
		locale: 'en',
		canvas_resize_policy: 2, // Adaptive
		virtual_keyboard: false,
		fade_pool_size: 4,
		debug_port: null,
		on_execute: null,
		on_terminate_pid: null,
		on_exit: null,
		pid: 0,

		init_config: function (p_opts) {
			FADEConfig.canvas_resize_policy = p_opts['canvasResizePolicy'];
			FADEConfig.canvas = p_opts['canvas'];
			FADEConfig.locale = p_opts['locale'] || FADEConfig.locale;
			FADEConfig.virtual_keyboard = p_opts['virtualKeyboard'];
			FADEConfig.fade_pool_size = p_opts['fadePoolSize'];
			FADEConfig.debug_port = p_opts['debugPort'];
			FADEConfig.pid = p_opts['pid'];
			FADEConfig.on_execute = p_opts['onExecute'];
			FADEConfig.on_terminate_pid = p_opts['onTerminatePID'];
			FADEConfig.on_exit = p_opts['onExit'];
			if (p_opts['focusCanvas']) {
				FADEConfig.canvas.focus();
			}
		},

		locate_file: function (file) {
			return Module['locateFile'](file);
		},

		clear: function () {
			FADEConfig.canvas = null;
			FADEConfig.locale = 'en';
			FADEConfig.canvas_resize_policy = 2;
			FADEConfig.virtual_keyboard = false;
			FADEConfig.debug_port = null;
			FADEConfig.on_execute = null;
			FADEConfig.on_exit = null;
		},
	},

	fade_js_config_pid_get__proxy: 'sync',
	fade_js_config_pid_get__sig: 'i',
	fade_js_config_pid_get: function () {
		return FADEConfig.pid;
	},

	fade_js_config_canvas_id_get__proxy: 'sync',
	fade_js_config_canvas_id_get__sig: 'vii',
	fade_js_config_canvas_id_get: function (p_ptr, p_ptr_max) {
		FADERuntime.stringToHeap(`#${FADEConfig.canvas.id}`, p_ptr, p_ptr_max);
	},

	fade_js_config_locale_get__proxy: 'sync',
	fade_js_config_locale_get__sig: 'vii',
	fade_js_config_locale_get: function (p_ptr, p_ptr_max) {
		FADERuntime.stringToHeap(FADEConfig.locale, p_ptr, p_ptr_max);
	},
};

autoAddDeps(FADEConfig, '$FADEConfig');
mergeInto(LibraryManager.library, FADEConfig);

const FADEFS = {
	$FADEFS__deps: ['$FS', '$IDBFS', '$FADERuntime'],
	$FADEFS__postset: [
		'Module["initFS"] = FADEFS.init;',
		'Module["copyToFS"] = FADEFS.copy_to_fs;',
	].join(''),
	$FADEFS: {
		// ERRNO_CODES works every odd version of emscripten, but this will break too eventually.
		ENOENT: 44,
		_idbfs: false,
		_syncing: false,
		_mount_points: [],

		is_persistent: function () {
			return FADEFS._idbfs ? 1 : 0;
		},

		// Initialize fade file system, setting up persistent paths.
		// Returns a promise that resolves when the FS is ready.
		// We keep track of mount_points, so that we can properly close the IDBFS
		// since emscripten is not doing it by itself. (emscripten GH#12516).
		init: function (persistentPaths) {
			FADEFS._idbfs = false;
			if (!Array.isArray(persistentPaths)) {
				return Promise.reject(new Error('Persistent paths must be an array'));
			}
			if (!persistentPaths.length) {
				return Promise.resolve();
			}
			FADEFS._mount_points = persistentPaths.slice();

			function createRecursive(dir) {
				try {
					FS.stat(dir);
				} catch (e) {
					if (e.errno !== FADEFS.ENOENT) {
						// Let mkdirTree throw in case, we cannot trust the above check.
						FADERuntime.error(e);
					}
					FS.mkdirTree(dir);
				}
			}

			FADEFS._mount_points.forEach(function (path) {
				createRecursive(path);
				FS.mount(IDBFS, {}, path);
			});
			return new Promise(function (resolve, reject) {
				FS.syncfs(true, function (err) {
					if (err) {
						FADEFS._mount_points = [];
						FADEFS._idbfs = false;
						FADERuntime.print(`IndexedDB not available: ${err.message}`);
					} else {
						FADEFS._idbfs = true;
					}
					resolve(err);
				});
			});
		},

		// Deinit fade file system, making sure to unmount file systems, and close IDBFS(s).
		deinit: function () {
			FADEFS._mount_points.forEach(function (path) {
				try {
					FS.unmount(path);
				} catch (e) {
					FADERuntime.print('Already unmounted', e);
				}
				if (FADEFS._idbfs && IDBFS.dbs[path]) {
					IDBFS.dbs[path].close();
					delete IDBFS.dbs[path];
				}
			});
			FADEFS._mount_points = [];
			FADEFS._idbfs = false;
			FADEFS._syncing = false;
		},

		sync: function () {
			if (FADEFS._syncing) {
				FADERuntime.error('Already syncing!');
				return Promise.resolve();
			}
			FADEFS._syncing = true;
			return new Promise(function (resolve, reject) {
				FS.syncfs(false, function (error) {
					if (error) {
						FADERuntime.error(`Failed to save IDB file system: ${error.message}`);
					}
					FADEFS._syncing = false;
					resolve(error);
				});
			});
		},

		// Copies a buffer to the internal file system. Creating directories recursively.
		copy_to_fs: function (path, buffer) {
			const idx = path.lastIndexOf('/');
			let dir = '/';
			if (idx > 0) {
				dir = path.slice(0, idx);
			}
			try {
				FS.stat(dir);
			} catch (e) {
				if (e.errno !== FADEFS.ENOENT) {
					// Let mkdirTree throw in case, we cannot trust the above check.
					FADERuntime.error(e);
				}
				FS.mkdirTree(dir);
			}
			FS.writeFile(path, new Uint8Array(buffer));
		},
	},
};
mergeInto(LibraryManager.library, FADEFS);

const FADEOS = {
	$FADEOS__deps: ['$FADERuntime', '$FADEConfig', '$FADEFS'],
	$FADEOS__postset: [
		'Module["request_quit"] = function() { FADEOS.request_quit() };',
		'Module["force_quit"] = function(code) { _emscripten_force_exit(code ?? 1) };',
		'Module["onExit"] = FADEOS.cleanup;',
		'FADEOS._fs_sync_promise = Promise.resolve();',
	].join(''),
	$FADEOS: {
		request_quit: function () {},
		_async_cbs: [],
		_fs_sync_promise: null,

		atexit: function (p_promise_cb) {
			FADEOS._async_cbs.push(p_promise_cb);
		},

		cleanup: function (exit_code) {
			const cb = FADEConfig.on_exit;
			FADEFS.deinit();
			FADEConfig.clear();
			if (cb) {
				cb(exit_code);
			}
		},

		finish_async: function (callback) {
			FADEOS._fs_sync_promise.then(function (err) {
				const promises = [];
				FADEOS._async_cbs.forEach(function (cb) {
					promises.push(new Promise(cb));
				});
				return Promise.all(promises);
			}).then(function () {
				return FADEFS.sync(); // Final FS sync.
			}).then(function (err) {
				// Always deferred.
				setTimeout(function () {
					callback();
				}, 0);
			});
		},
	},

	fade_js_os_finish_async__proxy: 'sync',
	fade_js_os_finish_async__sig: 'vi',
	fade_js_os_finish_async: function (p_callback) {
		const func = FADERuntime.get_func(p_callback);
		FADEOS.finish_async(func);
	},

	fade_js_os_request_quit_cb__proxy: 'sync',
	fade_js_os_request_quit_cb__sig: 'vi',
	fade_js_os_request_quit_cb: function (p_callback) {
		FADEOS.request_quit = FADERuntime.get_func(p_callback);
	},

	fade_js_os_fs_is_persistent__proxy: 'sync',
	fade_js_os_fs_is_persistent__sig: 'i',
	fade_js_os_fs_is_persistent: function () {
		return FADEFS.is_persistent();
	},

	fade_js_os_fs_sync__proxy: 'sync',
	fade_js_os_fs_sync__sig: 'vi',
	fade_js_os_fs_sync: function (callback) {
		const func = FADERuntime.get_func(callback);
		FADEOS._fs_sync_promise = FADEFS.sync();
		FADEOS._fs_sync_promise.then(function (err) {
			func();
		});
	},

	fade_js_os_has_feature__proxy: 'sync',
	fade_js_os_has_feature__sig: 'ii',
	fade_js_os_has_feature: function (p_ftr) {
		const ftr = FADERuntime.parseString(p_ftr);
		const ua = navigator.userAgent;
		if (ftr === 'web_macos') {
			return (ua.indexOf('Mac') !== -1) ? 1 : 0;
		}
		if (ftr === 'web_windows') {
			return (ua.indexOf('Windows') !== -1) ? 1 : 0;
		}
		if (ftr === 'web_android') {
			return (ua.indexOf('Android') !== -1) ? 1 : 0;
		}
		if (ftr === 'web_ios') {
			return ((ua.indexOf('iPhone') !== -1) || (ua.indexOf('iPad') !== -1) || (ua.indexOf('iPod') !== -1)) ? 1 : 0;
		}
		if (ftr === 'web_linuxbsd') {
			return ((ua.indexOf('CrOS') !== -1) || (ua.indexOf('BSD') !== -1) || (ua.indexOf('Linux') !== -1) || (ua.indexOf('X11') !== -1)) ? 1 : 0;
		}
		return 0;
	},

	fade_js_os_execute__proxy: 'sync',
	fade_js_os_execute__sig: 'ipp',
	fade_js_os_execute: function (p_path, p_args) {
		const path = FADERuntime.parseString(p_path);
		const json_args = FADERuntime.parseString(p_args);
		const args = JSON.parse(json_args);
		if (FADEConfig.on_execute) {
			return FADEConfig.on_execute(path, args) ?? 0;
		}
		return -1;
	},

	fade_js_os_kill__proxy: 'sync',
	fade_js_os_kill__sig: 'ii',
	fade_js_os_kill: function (p_pid) {
		if (!FADEConfig.on_terminate_pid) {
			return -1;
		}
		FADEConfig.on_terminate_pid(p_pid);
		return 0;
	},

	fade_js_os_shell_open__proxy: 'sync',
	fade_js_os_shell_open__sig: 'vi',
	fade_js_os_shell_open: function (p_uri) {
		window.open(FADERuntime.parseString(p_uri), '_blank');
	},

	fade_js_os_hw_concurrency_get__proxy: 'sync',
	fade_js_os_hw_concurrency_get__sig: 'i',
	fade_js_os_hw_concurrency_get: function () {
		// TODO FADE core needs fixing to avoid spawning too many threads (> 24).
		const concurrency = navigator.hardwareConcurrency || 1;
		return concurrency < 2 ? concurrency : 2;
	},

	fade_js_os_thread_pool_size_get__proxy: 'sync',
	fade_js_os_thread_pool_size_get__sig: 'i',
	fade_js_os_thread_pool_size_get: function () {
		if (typeof PThread === 'undefined') {
			// Threads aren't supported, so default to `1`.
			return 1;
		}

		return FADEConfig.fade_pool_size;
	},

	fade_js_os_download_buffer__proxy: 'sync',
	fade_js_os_download_buffer__sig: 'viiii',
	fade_js_os_download_buffer: function (p_ptr, p_size, p_name, p_mime) {
		const buf = FADERuntime.heapSlice(HEAP8, p_ptr, p_size);
		const name = FADERuntime.parseString(p_name);
		const mime = FADERuntime.parseString(p_mime);
		const blob = new Blob([buf], { type: mime });
		const url = window.URL.createObjectURL(blob);
		const a = document.createElement('a');
		a.href = url;
		a.download = name;
		a.style.display = 'none';
		document.body.appendChild(a);
		a.click();
		a.remove();
		window.URL.revokeObjectURL(url);
	},
};

autoAddDeps(FADEOS, '$FADEOS');
mergeInto(LibraryManager.library, FADEOS);

/*
 * FADE event listeners.
 * Keeps track of registered event listeners so it can remove them on shutdown.
 */
const FADEEventListeners = {
	$FADEEventListeners__deps: ['$FADEOS'],
	$FADEEventListeners__postset: 'FADEOS.atexit(function(resolve, reject) { FADEEventListeners.clear(); resolve(); });',
	$FADEEventListeners: {
		handlers: [],

		has: function (target, event, method, capture) {
			return FADEEventListeners.handlers.findIndex(function (e) {
				return e.target === target && e.event === event && e.method === method && e.capture === capture;
			}) !== -1;
		},

		add: function (target, event, method, capture) {
			if (FADEEventListeners.has(target, event, method, capture)) {
				return;
			}
			function Handler(p_target, p_event, p_method, p_capture) {
				this.target = p_target;
				this.event = p_event;
				this.method = p_method;
				this.capture = p_capture;
			}
			FADEEventListeners.handlers.push(new Handler(target, event, method, capture));
			target.addEventListener(event, method, capture);
		},

		clear: function () {
			FADEEventListeners.handlers.forEach(function (h) {
				h.target.removeEventListener(h.event, h.method, h.capture);
			});
			FADEEventListeners.handlers.length = 0;
		},
	},
};
mergeInto(LibraryManager.library, FADEEventListeners);

const FADEPWA = {

	$FADEPWA__deps: ['$FADERuntime', '$FADEEventListeners'],
	$FADEPWA: {
		hasUpdate: false,

		updateState: function (cb, reg) {
			if (!reg) {
				return;
			}
			if (!reg.active) {
				return;
			}
			if (reg.waiting) {
				FADEPWA.hasUpdate = true;
				cb();
			}
			FADEEventListeners.add(reg, 'updatefound', function () {
				const installing = reg.installing;
				FADEEventListeners.add(installing, 'statechange', function () {
					if (installing.state === 'installed') {
						FADEPWA.hasUpdate = true;
						cb();
					}
				});
			});
		},
	},

	fade_js_pwa_cb__proxy: 'sync',
	fade_js_pwa_cb__sig: 'vi',
	fade_js_pwa_cb: function (p_update_cb) {
		if ('serviceWorker' in navigator) {
			try {
				const cb = FADERuntime.get_func(p_update_cb);
				navigator.serviceWorker.getRegistration().then(FADEPWA.updateState.bind(null, cb));
			} catch (e) {
				FADERuntime.error('Failed to assign PWA callback', e);
			}
		}
	},

	fade_js_pwa_update__proxy: 'sync',
	fade_js_pwa_update__sig: 'i',
	fade_js_pwa_update: function () {
		if ('serviceWorker' in navigator && FADEPWA.hasUpdate) {
			try {
				navigator.serviceWorker.getRegistration().then(function (reg) {
					if (!reg || !reg.waiting) {
						return;
					}
					reg.waiting.postMessage('update');
				});
			} catch (e) {
				FADERuntime.error(e);
				return 1;
			}
			return 0;
		}
		return 1;
	},
};

autoAddDeps(FADEPWA, '$FADEPWA');
mergeInto(LibraryManager.library, FADEPWA);
