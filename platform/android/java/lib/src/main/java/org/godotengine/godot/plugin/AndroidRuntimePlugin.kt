/**************************************************************************/
/*  AndroidRuntimePlugin.kt                                               */
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

package org.fadeengine.fade.plugin

import android.content.Intent
import android.util.Log
import androidx.annotation.Keep
import androidx.core.net.toUri

import org.fadeengine.fade.FADE
import org.fadeengine.fade.variant.Callable
import java.lang.reflect.InvocationHandler
import java.lang.reflect.Proxy

/**
 * Built-in FADE Android plugin used to provide access to the Android runtime capabilities.
 *
 * @see <a href="https://docs.fadeengine.org/en/latest/tutorials/platform/android/javaclasswrapper_and_androidruntimeplugin.html">Integrating with Android APIs</a>
 */
class AndroidRuntimePlugin(fade: FADE) : FADEPlugin(fade) {

	companion object {
		private val TAG = AndroidRuntimePlugin::class.java.simpleName

		/**
		 * Helper method used to generate FADE Proxy instances.
		 */
		@JvmStatic
		@Keep
		private fun generateProxyInstance(interfaces: Array<String>, invocationHandler: InvocationHandler): Any? {
			try {
				val interfaceClasses = interfaces.map { Class.forName(it) }.toTypedArray()

				val proxy = Proxy.newProxyInstance(invocationHandler.javaClass.classLoader, interfaceClasses, invocationHandler)
				return proxy
			} catch (e: Exception) {
				Log.w(TAG, "Error generating FADE proxy for interfaces ${interfaces.joinToString(",")}", e)
			}
			return null
		}

		/**
		 * Utility method used to create [java.lang.reflect.Proxy] instance wrapping a given FADE [Callable].
		 *
		 * The [Proxy] instance is used to implement one SAM interface with the [Callable] serving as the delegate
		 * implementation for the SAM interface overridden methods.
		 */
		@JvmStatic
		@Keep
		private fun createProxyFromFADECallable(interfaceName: String, fadeCallable: Callable): Any? {
			return generateProxyInstance(arrayOf(interfaceName)) { proxy, method, args ->
				when (method.name) {
					// We automatically handle 'toString', 'equals' and 'hashCode' to simplify the task of the caller
					// and provide consistency.
					"toString" -> "FADE Callable Proxy for $interfaceName"
					"equals" -> proxy === args[0]
					"hashCode" -> fadeCallable.hashCode()

					// Invocation for the interface single abstract method falls here and is dispatched to the
					// FADE [Callable].
					else -> fadeCallable.call(*(args ?: emptyArray()))
				}
			}
		}

		/**
		 * Utility method used to create [java.lang.reflect.Proxy] instance wrapping a given FADE Object represented by
		 * its ObjectID.
		 *
		 * The [Proxy] instance is used to implement one or multiple interfaces with the Object represented by
		 * [fadeObjectID] serving as the delegate implementation for the interface(s) overridden methods.
		 */
		@JvmStatic
		@Keep
		private fun createProxyFromFADEObjectID(fadeObjectID: Long, interfaces: Array<String>): Any? {
			return generateProxyInstance(interfaces) { proxy, method, args ->
				when (val methodName = method.name) {
					// We automatically handle 'toString', 'equals' and 'hashCode' to simplify the task of the caller
					// and provide consistency.
					"toString" -> "FADE Object Proxy for ${interfaces.joinToString(",")}"
					"equals" -> proxy === args[0]
					"hashCode" -> fadeObjectID.hashCode()

					// Invocation for the remaining interface(s) methods falls here and is dispatched to the
					// FADE Object.
					else -> Callable.call(fadeObjectID, methodName, *(args ?: emptyArray()))
				}
			}
		}
	}

	override fun getPluginName() = "AndroidRuntime"

	/**
	 * Provides access to the application [android.content.Context] to GDScript
	 */
	@UsedByFADE
	fun getApplicationContext() = activity?.applicationContext

	/**
	 * Provides access to the host [android.app.Activity] to GDScript
	 */
	@UsedByFADE
	public override fun getActivity() = super.getActivity()

	/**
	 * Utility method used to create [Runnable] from FADE [Callable].
	 */
	@UsedByFADE
	fun createRunnableFromFADECallable(fadeCallable: Callable): Runnable {
		return Runnable { fadeCallable.call() }
	}

	/**
	 * Utility method used to create [java.util.concurrent.Callable] from FADE [Callable].
	 */
	@UsedByFADE
	fun createCallableFromFADECallable(fadeCallable: Callable): java.util.concurrent.Callable<Any> {
		return java.util.concurrent.Callable { fadeCallable.call() }
	}

	/**
	 * Helper method to take/release persistable URI permission.
	 */
	@UsedByFADE
	fun updatePersistableUriPermission(uriString: String, persist: Boolean): Boolean {
		try {
			val uri = uriString.toUri()
			val contentResolver = context.contentResolver
			if (persist) {
				contentResolver.takePersistableUriPermission(uri, Intent.FLAG_GRANT_READ_URI_PERMISSION or Intent.FLAG_GRANT_WRITE_URI_PERMISSION)
			} else {
				contentResolver.releasePersistableUriPermission(uri, Intent.FLAG_GRANT_READ_URI_PERMISSION or Intent.FLAG_GRANT_WRITE_URI_PERMISSION)
			}
		} catch (e: RuntimeException) {
			Log.d(TAG, "Error updating persistable permission: ", e)
			return false
		}
		return true
	}
}
