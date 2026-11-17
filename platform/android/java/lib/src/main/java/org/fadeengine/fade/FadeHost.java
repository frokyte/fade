/**************************************************************************/
/*  FADEHost.java                                                        */
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

package org.fadeengine.fade;

import org.fadeengine.fade.error.Error;
import org.fadeengine.fade.plugin.FADEPlugin;

import android.app.Activity;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;

import java.util.Collections;
import java.util.List;
import java.util.Set;

/**
 * Denotate a component (e.g: Activity, Fragment) that hosts the {@link FADE} engine.
 */
public interface FADEHost {
	/**
	 * Provides a set of command line parameters to setup the {@link FADE} engine.
	 */
	default List<String> getCommandLine() {
		return Collections.emptyList();
	}

	/**
	 * Invoked on the render thread when setup of the {@link FADE} engine is complete.
	 */
	default void onFADESetupCompleted() {}

	/**
	 * Invoked on the render thread when the {@link FADE} engine main loop has started.
	 */
	default void onFADEMainLoopStarted() {}

	/**
	 * Invoked on the render thread to terminate the given {@link FADE} engine instance.
	 */
	default void onFADEForceQuit(FADE instance) {}

	/**
	 * Invoked on the render thread to terminate the {@link FADE} engine instance with the given id.
	 * @param fadeInstanceId id of the FADE instance to terminate. See {@code onNewFADEInstanceRequested}
	 *
	 * @return true if successful, false otherwise.
	 */
	default boolean onFADEForceQuit(int fadeInstanceId) {
		return false;
	}

	/**
	 * Invoked on the render thread when the FADE instance wants to be restarted. It's up to the host
	 * to perform the appropriate action(s).
	 */
	default void onFADERestartRequested(FADE instance) {}

	/**
	 * Invoked on the render thread when a new FADE instance is requested. It's up to the host to
	 * perform the appropriate action(s).
	 *
	 * @param args Arguments used to initialize the new instance.
	 *
	 * @return the id of the new instance. See {@code onFADEForceQuit}
	 */
	default int onNewFADEInstanceRequested(String[] args) {
		return -1;
	}

	/**
	 * Provide access to the Activity hosting the {@link FADE} engine if any.
	 */
	@Nullable
	Activity getActivity();

	/**
	 * Provide access to the hosted {@link FADE} engine.
	 */
	FADE getFADE();

	/**
	 * Returns a set of {@link FADEPlugin} to be registered with the hosted {@link FADE} engine.
	 */
	default Set<FADEPlugin> getHostPlugins(FADE engine) {
		return Collections.emptySet();
	}

	/**
	 * Signs the given Android apk
	 *
	 * @param inputPath Path to the apk that should be signed
	 * @param outputPath Path for the signed output apk; can be the same as inputPath
	 * @param keystorePath Path to the keystore to use for signing the apk
	 * @param keystoreUser Keystore user credential
	 * @param keystorePassword Keystore password credential
	 *
	 * @return {@link Error#OK} if signing is successful
	 */
	default Error signApk(@NonNull String inputPath, @NonNull String outputPath, @NonNull String keystorePath, @NonNull String keystoreUser, @NonNull String keystorePassword) {
		return Error.ERR_UNAVAILABLE;
	}

	/**
	 * Verifies the given Android apk is signed
	 *
	 * @param apkPath Path to the apk that should be verified
	 * @return {@link Error#OK} if verification was successful
	 */
	default Error verifyApk(@NonNull String apkPath) {
		return Error.ERR_UNAVAILABLE;
	}

	/**
	 * Returns whether the given feature tag is supported.
	 *
	 * @see <a href="https://docs.fadeengine.org/en/stable/tutorials/export/feature_tags.html">Feature tags</a>
	 */
	default boolean supportsFeature(String featureTag) {
		return false;
	}

	/**
	 * Invoked on the render thread when an editor workspace has been selected.
	 */
	default void onEditorWorkspaceSelected(String workspace) {}

	/**
	 * Invoked on the render thread to request showing the editor game dock.
	 */
	default void showEditorGameDock() {}

	/**
	 * Triggered when the editor's distraction-free mode changes.
	 */
	default void onDistractionFreeModeChanged(Boolean enabled) {}

	/**
	 * Runs the specified action on a host provided thread.
	 */
	default void runOnHostThread(Runnable action) {
		if (action == null) {
			return;
		}

		Activity activity = getActivity();
		if (activity != null) {
			activity.runOnUiThread(action);
		}
	}

	/**
	 * Gets the build provider, if available.
	 *
	 * @return the build provider, if available; otherwise, null.
	 */
	default @Nullable BuildProvider getBuildProvider() {
		return null;
	}
}
