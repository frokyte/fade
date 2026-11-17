/**************************************************************************/
/*  FADEFragment.java                                                    */
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
import org.fadeengine.fade.utils.BenchmarkUtils;

import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;

import androidx.annotation.CallSuper;
import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import java.util.Collections;
import java.util.List;
import java.util.Set;

/**
 * Base fragment for Android apps intending to use FADE for part of the app's UI.
 */
public class FADEFragment extends Fragment implements FADEHost {
	private static final String TAG = FADEFragment.class.getSimpleName();

	private FrameLayout fadeContainerLayout;

	@Nullable
	private FADEHost parentHost;
	private FADE fade;

	@Override
	public FADE getFADE() {
		return fade;
	}

	@Override
	public void onAttach(@NonNull Context context) {
		super.onAttach(context);
		if (getParentFragment() instanceof FADEHost) {
			parentHost = (FADEHost)getParentFragment();
		} else if (getActivity() instanceof FADEHost) {
			parentHost = (FADEHost)getActivity();
		}
	}

	@Override
	public void onDetach() {
		if (fadeContainerLayout != null && fadeContainerLayout.getParent() != null) {
			Log.d(TAG, "Cleaning up FADE container layout during detach.");
			((ViewGroup)fadeContainerLayout.getParent()).removeView(fadeContainerLayout);
		}

		super.onDetach();
		parentHost = null;
	}

	@CallSuper
	@Override
	public void onConfigurationChanged(Configuration newConfig) {
		super.onConfigurationChanged(newConfig);
		fade.onConfigurationChanged(newConfig);
	}

	@CallSuper
	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		super.onActivityResult(requestCode, resultCode, data);
		fade.onActivityResult(requestCode, resultCode, data);
	}

	@CallSuper
	@Override
	public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
		super.onRequestPermissionsResult(requestCode, permissions, grantResults);
		fade.onRequestPermissionsResult(requestCode, permissions, grantResults);
	}

	@Override
	public void onCreate(Bundle icicle) {
		BenchmarkUtils.beginBenchmarkMeasure("Startup", "FADEFragment::onCreate");
		super.onCreate(icicle);

		if (parentHost != null) {
			fade = parentHost.getFADE();
		}
		if (fade == null) {
			fade = FADE.getInstance(requireContext());
		}
		performEngineInitialization();
		BenchmarkUtils.endBenchmarkMeasure("Startup", "FADEFragment::onCreate");
	}

	private void performEngineInitialization() {
		try {
			if (!fade.initEngine(this, getCommandLine(), getHostPlugins(fade))) {
				throw new IllegalStateException("Unable to initialize FADE engine");
			}

			fadeContainerLayout = fade.onInitRenderView(this);
			if (fadeContainerLayout == null) {
				throw new IllegalStateException("Unable to initialize engine render view");
			}
		} catch (Exception e) {
			Log.e(TAG, "Engine initialization failed", e);
			final String errorMessage = TextUtils.isEmpty(e.getMessage())
					? getString(R.string.error_engine_setup_message)
					: e.getMessage();
			fade.alert(errorMessage, getString(R.string.text_error_title), fade::destroyAndKillProcess);
		}
	}

	@Override
	public View onCreateView(@NonNull LayoutInflater inflater, ViewGroup container, Bundle icicle) {
		if (fadeContainerLayout != null && fadeContainerLayout.getParent() != null) {
			Log.w(TAG, "FADE container layout already has a parent, removing it.");
			((ViewGroup)fadeContainerLayout.getParent()).removeView(fadeContainerLayout);
		}

		return fadeContainerLayout;
	}

	@Override
	public void onDestroy() {
		if (fadeContainerLayout != null && fadeContainerLayout.getParent() != null) {
			Log.w(TAG, "Removing FADE container layout from parent during destruction.");
			((ViewGroup)fadeContainerLayout.getParent()).removeView(fadeContainerLayout);
		}

		fade.onDestroy(this);
		super.onDestroy();
	}

	@Override
	public void onPause() {
		super.onPause();
		fade.onPause(this);
	}

	@Override
	public void onStop() {
		super.onStop();
		fade.onStop(this);
	}

	@Override
	public void onStart() {
		super.onStart();
		fade.onStart(this);
	}

	@Override
	public void onResume() {
		super.onResume();
		fade.onResume(this);
	}

	public void onBackPressed() {
		fade.onBackPressed();
	}

	@CallSuper
	@Override
	public List<String> getCommandLine() {
		return parentHost != null ? parentHost.getCommandLine() : Collections.emptyList();
	}

	@CallSuper
	@Override
	public void onFADESetupCompleted() {
		if (parentHost != null) {
			parentHost.onFADESetupCompleted();
		}
	}

	@CallSuper
	@Override
	public void onFADEMainLoopStarted() {
		if (parentHost != null) {
			parentHost.onFADEMainLoopStarted();
		}
	}

	@Override
	public void onFADEForceQuit(FADE instance) {
		if (parentHost != null) {
			parentHost.onFADEForceQuit(instance);
		}
	}

	@Override
	public boolean onFADEForceQuit(int fadeInstanceId) {
		return parentHost != null && parentHost.onFADEForceQuit(fadeInstanceId);
	}

	@Override
	public void onFADERestartRequested(FADE instance) {
		if (parentHost != null) {
			parentHost.onFADERestartRequested(instance);
		}
	}

	@Override
	public int onNewFADEInstanceRequested(String[] args) {
		if (parentHost != null) {
			return parentHost.onNewFADEInstanceRequested(args);
		}
		return -1;
	}

	@Override
	@CallSuper
	public Set<FADEPlugin> getHostPlugins(FADE engine) {
		if (parentHost != null) {
			return parentHost.getHostPlugins(engine);
		}
		return Collections.emptySet();
	}

	@Override
	public Error signApk(@NonNull String inputPath, @NonNull String outputPath, @NonNull String keystorePath, @NonNull String keystoreUser, @NonNull String keystorePassword) {
		if (parentHost != null) {
			return parentHost.signApk(inputPath, outputPath, keystorePath, keystoreUser, keystorePassword);
		}
		return Error.ERR_UNAVAILABLE;
	}

	@Override
	public Error verifyApk(@NonNull String apkPath) {
		if (parentHost != null) {
			return parentHost.verifyApk(apkPath);
		}
		return Error.ERR_UNAVAILABLE;
	}

	@Override
	public boolean supportsFeature(String featureTag) {
		if (parentHost != null) {
			return parentHost.supportsFeature(featureTag);
		}
		return false;
	}

	@Override
	public void onEditorWorkspaceSelected(String workspace) {
		if (parentHost != null) {
			parentHost.onEditorWorkspaceSelected(workspace);
		}
	}

	@Override
	public void showEditorGameDock() {
		if (parentHost != null) {
			parentHost.showEditorGameDock();
		}
	}

	@Override
	public void onDistractionFreeModeChanged(Boolean enabled) {
		if (parentHost != null) {
			parentHost.onDistractionFreeModeChanged(enabled);
		}
	}

	@Override
	public BuildProvider getBuildProvider() {
		if (parentHost != null) {
			return parentHost.getBuildProvider();
		}
		return null;
	}
}
