/**************************************************************************/
/*  library_fade_websocket.js                                            */
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

const FADEWebSocket = {
	// Our socket implementation that forwards events to C++.
	$FADEWebSocket__deps: ['$IDHandler', '$FADERuntime'],
	$FADEWebSocket: {
		// Connection opened, report selected protocol
		_onopen: function (p_id, callback, event) {
			const ref = IDHandler.get(p_id);
			if (!ref) {
				return; // FADE object is gone.
			}
			const c_str = FADERuntime.allocString(ref.protocol);
			callback(c_str);
			FADERuntime.free(c_str);
		},

		// Message received, report content and type (UTF8 vs binary)
		_onmessage: function (p_id, callback, event) {
			const ref = IDHandler.get(p_id);
			if (!ref) {
				return; // FADE object is gone.
			}
			let buffer;
			let is_string = 0;
			if (event.data instanceof ArrayBuffer) {
				buffer = new Uint8Array(event.data);
			} else if (event.data instanceof Blob) {
				FADERuntime.error('Blob type not supported');
				return;
			} else if (typeof event.data === 'string') {
				is_string = 1;
				buffer = new TextEncoder('utf-8').encode(event.data);
			} else {
				FADERuntime.error('Unknown message type');
				return;
			}
			const len = buffer.length * buffer.BYTES_PER_ELEMENT;
			const out = FADERuntime.malloc(len);
			HEAPU8.set(buffer, out);
			callback(out, len, is_string);
			FADERuntime.free(out);
		},

		// An error happened, 'onclose' will be called after this.
		_onerror: function (p_id, callback, event) {
			const ref = IDHandler.get(p_id);
			if (!ref) {
				return; // FADE object is gone.
			}
			callback();
		},

		// Connection is closed, this is always fired. Report close code, reason, and clean status.
		_onclose: function (p_id, callback, event) {
			const ref = IDHandler.get(p_id);
			if (!ref) {
				return; // FADE object is gone.
			}
			const c_str = FADERuntime.allocString(event.reason);
			callback(event.code, c_str, event.wasClean ? 1 : 0);
			FADERuntime.free(c_str);
		},

		// Send a message
		send: function (p_id, p_data) {
			const ref = IDHandler.get(p_id);
			if (!ref || ref.readyState !== ref.OPEN) {
				return 1; // FADE object is gone or socket is not in a ready state.
			}
			ref.send(p_data);
			return 0;
		},

		// Get current bufferedAmount
		bufferedAmount: function (p_id) {
			const ref = IDHandler.get(p_id);
			if (!ref) {
				return 0; // FADE object is gone.
			}
			return ref.bufferedAmount;
		},

		create: function (socket, p_on_open, p_on_message, p_on_error, p_on_close) {
			const id = IDHandler.add(socket);
			socket.onopen = FADEWebSocket._onopen.bind(null, id, p_on_open);
			socket.onmessage = FADEWebSocket._onmessage.bind(null, id, p_on_message);
			socket.onerror = FADEWebSocket._onerror.bind(null, id, p_on_error);
			socket.onclose = FADEWebSocket._onclose.bind(null, id, p_on_close);
			return id;
		},

		// Closes the JavaScript WebSocket (if not already closing) associated to a given C++ object.
		close: function (p_id, p_code, p_reason) {
			const ref = IDHandler.get(p_id);
			if (ref && ref.readyState < ref.CLOSING) {
				const code = p_code;
				const reason = p_reason;
				ref.close(code, reason);
			}
		},

		// Deletes the reference to a C++ object (closing any connected socket if necessary).
		destroy: function (p_id) {
			const ref = IDHandler.get(p_id);
			if (!ref) {
				return;
			}
			FADEWebSocket.close(p_id, 3001, 'destroyed');
			IDHandler.remove(p_id);
			ref.onopen = null;
			ref.onmessage = null;
			ref.onerror = null;
			ref.onclose = null;
		},
	},

	fade_js_websocket_create__proxy: 'sync',
	fade_js_websocket_create__sig: 'iiiiiiii',
	fade_js_websocket_create: function (p_ref, p_url, p_proto, p_on_open, p_on_message, p_on_error, p_on_close) {
		const on_open = FADERuntime.get_func(p_on_open).bind(null, p_ref);
		const on_message = FADERuntime.get_func(p_on_message).bind(null, p_ref);
		const on_error = FADERuntime.get_func(p_on_error).bind(null, p_ref);
		const on_close = FADERuntime.get_func(p_on_close).bind(null, p_ref);
		const url = FADERuntime.parseString(p_url);
		const protos = FADERuntime.parseString(p_proto);
		let socket = null;
		try {
			if (protos) {
				socket = new WebSocket(url, protos.split(','));
			} else {
				socket = new WebSocket(url);
			}
		} catch (e) {
			return 0;
		}
		socket.binaryType = 'arraybuffer';
		return FADEWebSocket.create(socket, on_open, on_message, on_error, on_close);
	},

	fade_js_websocket_send__proxy: 'sync',
	fade_js_websocket_send__sig: 'iiiii',
	fade_js_websocket_send: function (p_id, p_buf, p_buf_len, p_raw) {
		let out = FADERuntime.heapSlice(HEAPU8, p_buf, p_buf_len);
		if (!p_raw) {
			out = new TextDecoder('utf-8').decode(out);
		}
		return FADEWebSocket.send(p_id, out);
	},

	fade_js_websocket_buffered_amount__proxy: 'sync',
	fade_js_websocket_buffered_amount__sig: 'ii',
	fade_js_websocket_buffered_amount: function (p_id) {
		return FADEWebSocket.bufferedAmount(p_id);
	},

	fade_js_websocket_close__proxy: 'sync',
	fade_js_websocket_close__sig: 'viii',
	fade_js_websocket_close: function (p_id, p_code, p_reason) {
		const code = p_code;
		const reason = FADERuntime.parseString(p_reason);
		FADEWebSocket.close(p_id, code, reason);
	},

	fade_js_websocket_destroy__proxy: 'sync',
	fade_js_websocket_destroy__sig: 'vi',
	fade_js_websocket_destroy: function (p_id) {
		FADEWebSocket.destroy(p_id);
	},
};

autoAddDeps(FADEWebSocket, '$FADEWebSocket');
mergeInto(LibraryManager.library, FADEWebSocket);
