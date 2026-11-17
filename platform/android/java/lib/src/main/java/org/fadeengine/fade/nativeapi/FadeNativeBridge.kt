/**************************************************************************/
/*  FADENativeBridge.kt                                                  */
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

package org.fadeengine.fade.nativeapi

import android.accessibilityservice.AccessibilityServiceInfo
import android.annotation.SuppressLint
import android.app.AlertDialog
import android.content.Context
import android.content.DialogInterface
import android.content.Intent
import android.content.res.Configuration
import android.provider.Settings.Global
import android.os.Build
import android.os.VibrationEffect
import android.os.Vibrator
import android.view.accessibility.AccessibilityManager
import android.util.Log
import android.util.Rational
import android.util.TypedValue
import androidx.annotation.Keep
import androidx.core.net.toUri
import org.fadeengine.fade.FADE
import org.fadeengine.fade.FADEActivity
import org.fadeengine.fade.R
import org.fadeengine.fade.error.Error
import org.fadeengine.fade.feature.PictureInPictureProvider
import org.fadeengine.fade.io.FilePicker
import org.fadeengine.fade.utils.DialogUtils
import org.fadeengine.fade.utils.FADENetUtils
import org.fadeengine.fade.utils.beginBenchmarkMeasure
import org.fadeengine.fade.utils.dumpBenchmark
import org.fadeengine.fade.utils.endBenchmarkMeasure
import org.fadeengine.fade.variant.Callable as FADECallable

/**
 * Holds and expose FADE apis to the native layer.
 *
 * All the methods in this class are accessed by the native code (java_fade_wrapper.h) and as such are kept private to
 * not be accessible by the rest of the java/kotlin code.
 */
@Keep
internal class FADENativeBridge(private val fade: FADE) {

	companion object {
		private val TAG = FADENativeBridge::class.java.simpleName

	}

	private val vibratorService: Vibrator? by lazy { fade.context.getSystemService(Context.VIBRATOR_SERVICE) as? Vibrator }

	/**
	 * Invoked on the render thread when the FADE setup is complete.
	 */
	private fun onFADESetupCompleted() {
		fade.onFADESetupCompleted()
	}

	/**
	 * Invoked on the render thread when the FADE main loop has started.
	 */
	private fun onFADEMainLoopStarted() {
		fade.onFADEMainLoopStarted()
	}

	/**
	 * Invoked on the render thread when the engine is about to terminate.
	 */
	private fun onFADETerminating() = fade.onFADETerminating()

	/**
	 * Invoked from the render thread to toggle the immersive mode.
	 */
	private fun nativeEnableImmersiveMode(enabled: Boolean) {
		fade.runOnHostThread {
			fade.enableImmersiveMode(enabled)
		}
	}

	private fun isInImmersiveMode() = fade.isInImmersiveMode()

	private fun isInEdgeToEdgeMode() = fade.isInEdgeToEdgeMode()

	private fun setKeepScreenOn(enabled: Boolean) = fade.setKeepScreenOn(enabled)

	private fun restart() { fade.primaryHost?.onFADERestartRequested(fade) }

	private fun alert(message: String, title: String) {
		fade.alert(message, title)
	}

	private fun forceQuit(instanceId: Int) = fade.forceQuit(instanceId)

	/**
	 * Returns true if screen reader is active, false otherwise.
	 */
	@Keep
	private fun isScreenReaderActive(): Boolean {
		val am = fade.context.getSystemService(Context.ACCESSIBILITY_SERVICE) as? AccessibilityManager
		if (am != null && am.isEnabled()) {
			val serviceInfoList = am.getEnabledAccessibilityServiceList(AccessibilityServiceInfo.FEEDBACK_SPOKEN)
			if (!serviceInfoList.isEmpty()) {
				return true
			}
		}
		return false
	}

	/**
	 * Returns true if high contrast text is enabled, false otherwise.
	 */
	@Keep
	private fun isHighContrastActive(): Boolean {
		val am = fade.context.getSystemService(Context.ACCESSIBILITY_SERVICE) as? AccessibilityManager
		if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.BAKLAVA && am != null && am.isEnabled()) {
			return am.isHighContrastTextEnabled()
		}
		return false
	}

	/**
	 * Returns true if animation is disabled, false otherwise.
	 */
	@Keep
	private fun isAnimationDisabled(): Boolean {
		val am = fade.context.getSystemService(Context.ACCESSIBILITY_SERVICE) as? AccessibilityManager
		if (am != null && am.isEnabled()) {
			return (Global.getFloat(fade.context.getContentResolver(), Global.ANIMATOR_DURATION_SCALE, 1.0f) == 0f
                && Global.getFloat(fade.context.getContentResolver(), Global.TRANSITION_ANIMATION_SCALE, 1.0f) == 0f
                && Global.getFloat(fade.context.getContentResolver(), Global.WINDOW_ANIMATION_SCALE, 1.0f) == 0f)
		}
		return false
	}

	/**
	 * Returns true if dark mode is supported, false otherwise.
	 */
	private fun isDarkModeSupported(): Boolean {
		return fade.context.resources?.configuration?.uiMode?.and(Configuration.UI_MODE_NIGHT_MASK) != Configuration.UI_MODE_NIGHT_UNDEFINED
	}

	/**
	 * Returns true if dark mode is supported and enabled, false otherwise.
	 */
	private fun isDarkMode() = fade.darkMode

	private fun showFilePicker(currentDirectory: String, filename: String, fileMode: Int, filters: Array<String>) {
		FilePicker.showFilePicker(fade.context, fade.getActivity(), currentDirectory, filename, fileMode, filters)
	}

	/**
	 * This method shows a dialog with multiple buttons.
	 *
	 * @param title The title of the dialog.
	 * @param message The message displayed in the dialog.
	 * @param buttons An array of button labels to display.
	 */
	private fun showDialog(title: String, message: String, buttons: Array<String>) {
		fade.getActivity()?.let { DialogUtils.showDialog(it, title, message, buttons) }
	}

	/**
	 * This method shows a dialog with a text input field, allowing the user to input text.
	 *
	 * @param title The title of the input dialog.
	 * @param message The message displayed in the input dialog.
	 * @param existingText The existing text that will be pre-filled in the input field.
	 */
	private fun showInputDialog(title: String, message: String, existingText: String) {
		fade.getActivity()?.let { DialogUtils.showInputDialog(it, title, message, existingText) }
	}

	private fun getAccentColor(): Int {
		val value = TypedValue()
		fade.context.theme.resolveAttribute(android.R.attr.colorAccent, value, true)
		return value.data
	}

	private fun getBaseColor(): Int {
		val value = TypedValue()
		fade.context.theme.resolveAttribute(android.R.attr.colorBackground, value, true)
		return value.data
	}

	private fun requestPermission(name: String?) = fade.requestPermission(name)

	private fun requestPermissions() = fade.requestPermissions()

	private fun getGrantedPermissions() = fade.getGrantedPermissions()

	/**
	 * Used by the native code (java_fade_wrapper.h) to vibrate the device.
	 * @param durationMs
	 */
	@SuppressLint("MissingPermission")
	private fun vibrate(durationMs: Int, amplitude: Int) {
		if (durationMs > 0 && fade.requestPermission("VIBRATE")) {
			try {
				if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
					if (amplitude <= -1) {
						vibratorService?.vibrate(
							VibrationEffect.createOneShot(
								durationMs.toLong(),
								VibrationEffect.DEFAULT_AMPLITUDE
							)
						)
					} else {
						vibratorService?.vibrate(
							VibrationEffect.createOneShot(
								durationMs.toLong(),
								amplitude
							)
						)
					}
				} else {
					// deprecated in API 26
					vibratorService?.vibrate(durationMs.toLong())
				}
			} catch (e: SecurityException) {
				Log.w(
					TAG,
					"SecurityException: VIBRATE permission not found. Make sure it is declared in the manifest or enabled in the export preset."
				)
			}
		}
	}

	/**
	 * Internal method used to query whether the host or the registered plugins supports a given feature.
	 *
	 * This is invoked by the native code, and should not be confused with [hasFeature] which is the Android version of
	 * https://docs.fadeengine.org/en/stable/classes/class_os.html#class-os-method-has-feature
	 */
	private fun checkInternalFeatureSupport(feature: String): Boolean {
		if (fade.primaryHost?.supportsFeature(feature) == true) {
			return true
		}

		for (plugin in fade.pluginRegistry.allPlugins) {
			if (plugin.supportsFeature(feature)) {
				return true
			}
		}
		return false
	}

	/**
	 * Get the list of gdextension modules to register.
	 */
	private fun getGDExtensionConfigFiles(): Array<String> {
		val configFiles = mutableSetOf<String>()
		for (plugin in fade.pluginRegistry.allPlugins) {
			configFiles.addAll(plugin.pluginGDExtensionLibrariesPaths)
		}

		return configFiles.toTypedArray()
	}

	private fun getCACertificates(): String {
		return FADENetUtils.getCACertificates()
	}

	private fun getActivity() = fade.getActivity()

	private fun getRenderView() = fade.renderView

	private fun getClipboard() = fade.getClipboard()

	private fun setClipboard(text: String) = fade.setClipboard(text)

	private fun hasClipboard() = fade.hasClipboard()

	private fun setWindowColor(color: String) = fade.setWindowColor(color)

	/**
	 * Used by the native code (java_fade_wrapper.h) to access the input fallback mapping.
	 * @return The input fallback mapping for the current XR mode.
	 */
	private fun getInputFallbackMapping(): String? {
		return fade.xrMode.inputFallbackMapping
	}

	private fun initInputDevices() {
		fade.fadeInputHandler.initInputDevices()
	}

	private fun createNewFADEInstance(args: Array<String>): Int {
		return fade.primaryHost?.onNewFADEInstanceRequested(args) ?: -1
	}

	private fun nativeBeginBenchmarkMeasure(scope: String, label: String) {
		beginBenchmarkMeasure(scope, label)
	}

	private fun nativeEndBenchmarkMeasure(scope: String, label: String) {
		endBenchmarkMeasure(scope, label)
	}

	private fun nativeDumpBenchmark(benchmarkFile: String) {
		dumpBenchmark(fade.fileAccessHandler, benchmarkFile)
	}

	private fun nativeSignApk(
		inputPath: String,
		outputPath: String,
		keystorePath: String,
		keystoreUser: String,
		keystorePassword: String
	): Int {
		val signResult = fade.primaryHost?.signApk(inputPath, outputPath, keystorePath, keystoreUser, keystorePassword)
			?: org.fadeengine.fade.error.Error.ERR_UNAVAILABLE
		return signResult.toNativeValue()
	}

	private fun nativeVerifyApk(apkPath: String): Int {
		val verifyResult = fade.primaryHost?.verifyApk(apkPath) ?: Error.ERR_UNAVAILABLE
		return verifyResult.toNativeValue()
	}

	private fun nativeOnEditorWorkspaceSelected(workspace: String) {
		fade.primaryHost?.onEditorWorkspaceSelected(workspace)
	}

	private fun nativeShowEditorGameDock() {
		fade.primaryHost?.showEditorGameDock()
	}

	private fun nativeOnDistractionFreeModeChanged(enabled: Boolean) {
		fade.primaryHost?.onDistractionFreeModeChanged(enabled)
	}

	private fun nativeBuildEnvConnect(callback: FADECallable): Boolean {
		try {
			val buildProvider = fade.primaryHost?.buildProvider
			val success = buildProvider?.buildEnvConnect(callback) ?: false
			if (!success) {
				val activity = fade.getActivity() ?: return false
				fade.runOnHostThread {
					val builder = AlertDialog.Builder(activity)
						.setMessage(activity.getString(R.string.gabe_connection_error_message))
						.setTitle(activity.getString(R.string.gabe_connection_error_title))
						.setCancelable(false)
						.setPositiveButton(activity.getString(R.string.dialog_download)) { dialog: DialogInterface, _: Int ->
							val intent = Intent(Intent.ACTION_VIEW, "https://fadeengine.org/download/android#gabe".toUri())
							intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK)
							activity.startActivity(intent)
							dialog.cancel()
						}
						.setNegativeButton(activity.getString(R.string.dialog_cancel)) { dialog: DialogInterface, _: Int ->
							dialog.dismiss()
						}
					val dialog = builder.create()
					dialog.show()
				}
			}
			return success
		} catch (e: Exception) {
			Log.e(TAG, "Unable to connect to build environment", e)
			return false
		}
	}

	private fun nativeBuildEnvDisconnect() {
		try {
			val buildProvider = fade.primaryHost?.buildProvider
			buildProvider?.buildEnvDisconnect()
		} catch (e: Exception) {
			Log.e(TAG, "Unable to disconnect from build environment", e)
		}
	}

	private fun nativeBuildEnvExecute(
		buildTool: String,
		arguments: Array<String>,
		projectPath: String,
		buildDir: String,
		outputCallback: FADECallable,
		resultCallback: FADECallable
	): Int {
		try {
			val buildProvider = fade.primaryHost?.buildProvider
			return buildProvider?.buildEnvExecute(
				buildTool,
				arguments,
				projectPath,
				buildDir,
				outputCallback,
				resultCallback
			) ?: -1
		} catch (e: Exception) {
			Log.e(TAG, "Unable to execute Gradle command in build environment", e);
			return -1
		}
	}

	private fun nativeBuildEnvCancel(jobId: Int) {
		try {
			val buildProvider = fade.primaryHost?.buildProvider
			buildProvider?.buildEnvCancel(jobId)
		} catch (e: Exception) {
			Log.e(TAG, "Unable to cancel command in build environment", e)
		}
	}

	private fun nativeBuildEnvCleanProject(projectPath: String, buildDir: String, callback: FADECallable) {
		try {
			val buildProvider = fade.primaryHost?.buildProvider
			buildProvider?.buildEnvCleanProject(projectPath, buildDir, callback)
		} catch (e: Exception) {
			Log.e(TAG, "Unable to clean project in build environment", e)
		}
	}

	private fun nativeIsPiPModeSupported(): Boolean {
		val hostActivity = fade.getActivity()
		if (hostActivity is PictureInPictureProvider) {
			return hostActivity.isPiPModeSupported()
		}
		return false
	}

	private fun nativeIsInPiPMode(): Boolean {
		val hostActivity = fade.getActivity()
		if (hostActivity is FADEActivity) {
			return hostActivity.isInPictureInPictureMode
		}
		return false
	}

	private fun nativeEnterPiPMode() {
		val hostActivity = fade.getActivity()
		if (hostActivity is PictureInPictureProvider) {
			fade.runOnHostThread {
				hostActivity.enterPiPMode()
			}
		}
	}

	private fun nativeSetPiPModeAspectRatio(numerator: Int, denominator: Int) {
		val hostActivity = fade.getActivity()
		if (hostActivity is FADEActivity) {
			fade.runOnHostThread {
				hostActivity.updatePiPParams(aspectRatio = Rational(numerator, denominator))
			}
		}
	}

	private fun nativeSetAutoEnterPiPModeOnBackground(autoEnterPiPOnBackground: Boolean) {
		val hostActivity = fade.getActivity()
		if (hostActivity is FADEActivity) {
			fade.runOnHostThread {
				hostActivity.updatePiPParams(enableAutoEnter = autoEnterPiPOnBackground)
			}
		}
	}
}
