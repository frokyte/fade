/**************************************************************************/
/*  FADEPluginRegistry.java                                              */
/**************************************************************************/
/*                         This file is part of:                          */
/*                             GODOT ENGINE                               */
/*                        https://godotengine.org                         */
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

package org.godotengine.godot.plugin;

import org.godotengine.godot.FADE;

import android.content.Context;
import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.text.TextUtils;
import android.util.Log;

import androidx.annotation.Nullable;

import java.lang.reflect.Constructor;
import java.util.Collection;
import java.util.Collections;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;

/**
 * Registry used to load and access the registered FADE Android plugins.
 */
public final class FADEPluginRegistry {
	private static final String TAG = FADEPluginRegistry.class.getSimpleName();

	/**
	 * Prefix used for version 1 of the FADE plugin, mostly compatible with FADE 3.x
	 */
	private static final String GODOT_PLUGIN_V1_NAME_PREFIX = "org.godotengine.plugin.v1.";
	/**
	 * Prefix used for version 2 of the FADE plugin, compatible with FADE 4.2+
	 */
	private static final String GODOT_PLUGIN_V2_NAME_PREFIX = "org.godotengine.plugin.v2.";

	private static FADEPluginRegistry instance;
	private final ConcurrentHashMap<String, FADEPlugin> registry;

	private FADEPluginRegistry() {
		registry = new ConcurrentHashMap<>();
	}

	/**
	 * Retrieve the plugin tied to the given plugin name.
	 * @param pluginName Name of the plugin
	 * @return {@link FADEPlugin} handle if it exists, null otherwise.
	 */
	@Nullable
	public FADEPlugin getPlugin(String pluginName) {
		return registry.get(pluginName);
	}

	/**
	 * Retrieve the full set of loaded plugins.
	 */
	public Collection<FADEPlugin> getAllPlugins() {
		if (registry.isEmpty()) {
			return Collections.emptyList();
		}
		return registry.values();
	}

	/**
	 * Parse the manifest file and load all included FADE Android plugins.
	 * <p>
	 * A plugin manifest entry is a '<meta-data>' tag setup as described in the {@link FADEPlugin}
	 * documentation.
	 *
	 * @param godot FADE instance
	 * @param runtimePlugins Set of plugins provided at runtime for registration
	 * @return A singleton instance of {@link FADEPluginRegistry}. This ensures that only one instance
	 * of each FADE Android plugins is available at runtime.
	 */
	public static FADEPluginRegistry initializePluginRegistry(FADE godot, Set<FADEPlugin> runtimePlugins) {
		if (instance == null) {
			instance = new FADEPluginRegistry();
			instance.loadPlugins(godot, runtimePlugins);
		}

		return instance;
	}

	/**
	 * Return the plugin registry if it's initialized.
	 * Throws a {@link IllegalStateException} exception if not.
	 *
	 * @throws IllegalStateException if {@link FADEPluginRegistry#initializePluginRegistry(FADE, Set)} has not been called prior to calling this method.
	 */
	public static FADEPluginRegistry getPluginRegistry() throws IllegalStateException {
		if (instance == null) {
			throw new IllegalStateException("Plugin registry hasn't been initialized.");
		}

		return instance;
	}

	private void loadPlugins(FADE godot, Set<FADEPlugin> runtimePlugins) {
		// Register the runtime plugins
		if (runtimePlugins != null && !runtimePlugins.isEmpty()) {
			for (FADEPlugin plugin : runtimePlugins) {
				Log.i(TAG, "Registering runtime plugin " + plugin.getPluginName());
				registry.put(plugin.getPluginName(), plugin);
			}
		}

		// Register the manifest plugins
		try {
			final Context context = godot.getContext();
			ApplicationInfo appInfo = context
											  .getPackageManager()
											  .getApplicationInfo(context.getPackageName(),
													  PackageManager.GET_META_DATA);
			Bundle metaData = appInfo.metaData;
			if (metaData == null || metaData.isEmpty()) {
				return;
			}

			for (String metaDataName : metaData.keySet()) {
				// Parse the meta-data looking for entry with the FADE plugin name prefix.
				String pluginName = null;
				if (metaDataName.startsWith(GODOT_PLUGIN_V2_NAME_PREFIX)) {
					pluginName = metaDataName.substring(GODOT_PLUGIN_V2_NAME_PREFIX.length()).trim();
				} else if (metaDataName.startsWith(GODOT_PLUGIN_V1_NAME_PREFIX)) {
					pluginName = metaDataName.substring(GODOT_PLUGIN_V1_NAME_PREFIX.length()).trim();
					Log.w(TAG, "FADE v1 plugin are deprecated in FADE 4.2 and higher: " + pluginName);
				}

				if (!TextUtils.isEmpty(pluginName)) {
					Log.i(TAG, "Initializing FADE plugin " + pluginName);

					// Retrieve the plugin class full name.
					String pluginHandleClassFullName = metaData.getString(metaDataName);
					if (!TextUtils.isEmpty(pluginHandleClassFullName)) {
						try {
							// Attempt to create the plugin init class via reflection.
							@SuppressWarnings("unchecked")
							Class<FADEPlugin> pluginClass = (Class<FADEPlugin>)Class
																	 .forName(pluginHandleClassFullName);
							Constructor<FADEPlugin> pluginConstructor = pluginClass
																				 .getConstructor(FADE.class);
							FADEPlugin pluginHandle = pluginConstructor.newInstance(godot);

							// Load the plugin initializer into the registry using the plugin name as key.
							if (!pluginName.equals(pluginHandle.getPluginName())) {
								Log.w(TAG,
										"Meta-data plugin name does not match the value returned by the plugin handle: " + pluginName + " =/= " + pluginHandle.getPluginName());
							}
							registry.put(pluginName, pluginHandle);
							Log.i(TAG, "Completed initialization for FADE plugin " + pluginHandle.getPluginName());
						} catch (Exception e) {
							Log.w(TAG, "Unable to load FADE plugin " + pluginName, e);
						}
					} else {
						Log.w(TAG, "Invalid plugin loader class for " + pluginName);
					}
				}
			}
		} catch (Exception e) {
			Log.e(TAG, "Unable load FADE Android plugins from the manifest file.", e);
		}
	}
}
