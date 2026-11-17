/**************************************************************************/
/*  editor_internal_calls.cpp                                             */
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

#include "editor_internal_calls.h"

#include "../fadesharp_dirs.h"
#include "../interop_types.h"
#include "../utils/path_utils.h"
#include "code_completion.h"

#ifdef GD_MONO_HOT_RELOAD
#include "../csharp_script.h"
#endif

#ifdef MACOS_ENABLED
#include "../utils/macos_utils.h"
#endif

#include "core/config/project_settings.h"
#include "core/os/os.h"
#include "core/version.h"
#include "editor/debugger/editor_debugger_node.h"
#include "editor/editor_node.h"
#include "editor/export/lipo.h"
#include "editor/file_system/editor_paths.h"
#include "editor/run/editor_run_bar.h"
#include "editor/script/script_editor_plugin.h"
#include "editor/settings/editor_settings.h"
#include "editor/themes/editor_scale.h"
#include "main/main.h"

#ifdef GD_MONO_HOT_RELOAD
#include "core/object/callable_mp.h"
#endif

#ifdef UNIX_ENABLED
#include <unistd.h> // access
#endif

#ifdef __cplusplus
extern "C" {
#endif

void fade_icall_FADESharpDirs_ResMetadataDir(fade_string *r_dest) {
	memnew_placement(r_dest, String(FADESharpDirs::get_res_metadata_dir()));
}

void fade_icall_FADESharpDirs_MonoUserDir(fade_string *r_dest) {
	memnew_placement(r_dest, String(FADESharpDirs::get_mono_user_dir()));
}

void fade_icall_FADESharpDirs_BuildLogsDirs(fade_string *r_dest) {
	memnew_placement(r_dest, String(FADESharpDirs::get_build_logs_dir()));
}

void fade_icall_FADESharpDirs_DataEditorToolsDir(fade_string *r_dest) {
	memnew_placement(r_dest, String(FADESharpDirs::get_data_editor_tools_dir()));
}

void fade_icall_FADESharpDirs_CSharpProjectName(fade_string *r_dest) {
	memnew_placement(r_dest, String(Path::get_csharp_project_name()));
}

void fade_icall_EditorProgress_Create(const fade_string *p_task, const fade_string *p_label, int32_t p_amount, bool p_can_cancel) {
	String task = *reinterpret_cast<const String *>(p_task);
	String label = *reinterpret_cast<const String *>(p_label);
	EditorNode::progress_add_task(task, label, p_amount, (bool)p_can_cancel);
}

void fade_icall_EditorProgress_Dispose(const fade_string *p_task) {
	String task = *reinterpret_cast<const String *>(p_task);
	EditorNode::progress_end_task(task);
}

bool fade_icall_EditorProgress_Step(const fade_string *p_task, const fade_string *p_state, int32_t p_step, bool p_force_refresh) {
	String task = *reinterpret_cast<const String *>(p_task);
	String state = *reinterpret_cast<const String *>(p_state);
	return EditorNode::progress_task_step(task, state, p_step, (bool)p_force_refresh);
}

void fade_icall_Internal_FullExportTemplatesDir(fade_string *r_dest) {
	String full_templates_dir = EditorPaths::get_singleton()->get_export_templates_dir().path_join(GODOT_VERSION_FULL_CONFIG);
	memnew_placement(r_dest, String(full_templates_dir));
}

bool fade_icall_Internal_IsMacOSAppBundleInstalled(const fade_string *p_bundle_id) {
#ifdef MACOS_ENABLED
	String bundle_id = *reinterpret_cast<const String *>(p_bundle_id);
	return (bool)macos_is_app_bundle_installed(bundle_id);
#else
	(void)p_bundle_id; // UNUSED
	return (bool)false;
#endif
}

bool fade_icall_Internal_LipOCreateFile(const fade_string *p_output_path, const fade_packed_array *p_files) {
	String output_path = *reinterpret_cast<const String *>(p_output_path);
	PackedStringArray files = *reinterpret_cast<const PackedStringArray *>(p_files);
	return LipO::create_file(output_path, files);
}

bool fade_icall_Internal_FADEIs32Bits() {
	return sizeof(void *) == 4;
}

bool fade_icall_Internal_FADEIsRealTDouble() {
#ifdef REAL_T_IS_DOUBLE
	return (bool)true;
#else
	return (bool)false;
#endif
}

void fade_icall_Internal_FADEMainIteration() {
	Main::iteration();
}

bool fade_icall_Internal_IsAssembliesReloadingNeeded() {
#ifdef GD_MONO_HOT_RELOAD
	return (bool)CSharpLanguage::get_singleton()->is_assembly_reloading_needed();
#else
	return (bool)false;
#endif
}

void fade_icall_Internal_ReloadAssemblies() {
#ifdef GD_MONO_HOT_RELOAD
	callable_mp(MonoBind::FADESharp::get_singleton(), &MonoBind::FADESharp::reload_assemblies).call_deferred();
#endif
}

void fade_icall_Internal_EditorDebuggerNodeReloadScripts() {
	EditorDebuggerNode::get_singleton()->reload_all_scripts();
}

bool fade_icall_Internal_ScriptEditorEdit(Resource *p_resource, int32_t p_line, int32_t p_col, bool p_grab_focus) {
	Ref<Resource> resource = p_resource;
	return (bool)ScriptEditor::get_singleton()->edit(resource, p_line, p_col, (bool)p_grab_focus);
}

void fade_icall_Internal_EditorNodeShowScriptScreen() {
	ScriptEditor::get_singleton()->focus_editor();
}

void fade_icall_Internal_EditorRunPlay() {
	EditorRunBar::get_singleton()->play_main_scene();
}

void fade_icall_Internal_EditorRunStop() {
	EditorRunBar::get_singleton()->stop_playing();
}

void fade_icall_Internal_EditorPlugin_AddControlToEditorRunBar(Control *p_control) {
	EditorRunBar::get_singleton()->get_buttons_container()->add_child(p_control);
}

void fade_icall_Internal_ScriptEditorDebugger_ReloadScripts() {
	EditorDebuggerNode *ed = EditorDebuggerNode::get_singleton();
	if (ed) {
		ed->reload_all_scripts();
	}
}

void fade_icall_Internal_CodeCompletionRequest(int32_t p_kind, const fade_string *p_script_file, fade_packed_array *r_ret) {
	String script_file = *reinterpret_cast<const String *>(p_script_file);
	PackedStringArray suggestions = gdmono::get_code_completion((gdmono::CompletionKind)p_kind, script_file);
	memnew_placement(r_ret, PackedStringArray(suggestions));
}

float fade_icall_Globals_EditorScale() {
	return EDSCALE;
}

void fade_icall_Globals_GlobalDef(const fade_string *p_setting, const fade_variant *p_default_value, bool p_restart_if_changed, fade_variant *r_result) {
	String setting = *reinterpret_cast<const String *>(p_setting);
	Variant default_value = *reinterpret_cast<const Variant *>(p_default_value);
	Variant result = _GLOBAL_DEF(setting, default_value, (bool)p_restart_if_changed);
	memnew_placement(r_result, Variant(result));
}

void fade_icall_Globals_EditorDef(const fade_string *p_setting, const fade_variant *p_default_value, bool p_restart_if_changed, fade_variant *r_result) {
	String setting = *reinterpret_cast<const String *>(p_setting);
	Variant default_value = *reinterpret_cast<const Variant *>(p_default_value);
	Variant result = _EDITOR_DEF(setting, default_value, (bool)p_restart_if_changed);
	memnew_placement(r_result, Variant(result));
}

void fade_icall_Globals_EditorDefShortcut(const fade_string *p_setting, const fade_string *p_name, Key p_keycode, bool p_physical, fade_variant *r_result) {
	String setting = *reinterpret_cast<const String *>(p_setting);
	String name = *reinterpret_cast<const String *>(p_name);
	Ref<Shortcut> result = ED_SHORTCUT(setting, name, p_keycode, p_physical);
	memnew_placement(r_result, Variant(result));
}

void fade_icall_Globals_EditorGetShortcut(const fade_string *p_setting, Ref<Shortcut> *r_result) {
	String setting = *reinterpret_cast<const String *>(p_setting);
	Ref<Shortcut> result = ED_GET_SHORTCUT(setting);
	memnew_placement(r_result, Variant(result));
}

void fade_icall_Globals_EditorShortcutOverride(const fade_string *p_setting, const fade_string *p_feature, Key p_keycode, bool p_physical) {
	String setting = *reinterpret_cast<const String *>(p_setting);
	String feature = *reinterpret_cast<const String *>(p_feature);
	ED_SHORTCUT_OVERRIDE(setting, feature, p_keycode, p_physical);
}

void fade_icall_Globals_TTR(const fade_string *p_text, fade_string *r_dest) {
	String text = *reinterpret_cast<const String *>(p_text);
	memnew_placement(r_dest, String(TTR(text)));
}

void fade_icall_Utils_OS_GetPlatformName(fade_string *r_dest) {
	String os_name = OS::get_singleton()->get_name();
	memnew_placement(r_dest, String(os_name));
}

bool fade_icall_Utils_OS_UnixFileHasExecutableAccess(const fade_string *p_file_path) {
#ifdef UNIX_ENABLED
	String file_path = *reinterpret_cast<const String *>(p_file_path);
	return access(file_path.utf8().get_data(), X_OK) == 0;
#else
	ERR_FAIL_V(false);
#endif
}

#ifdef __cplusplus
}
#endif

// The order in this array must match the declaration order of
// the methods in 'FADETools/Internals/Internal.cs'.
static const void *unmanaged_callbacks[]{
	(void *)fade_icall_FADESharpDirs_ResMetadataDir,
	(void *)fade_icall_FADESharpDirs_MonoUserDir,
	(void *)fade_icall_FADESharpDirs_BuildLogsDirs,
	(void *)fade_icall_FADESharpDirs_DataEditorToolsDir,
	(void *)fade_icall_FADESharpDirs_CSharpProjectName,
	(void *)fade_icall_EditorProgress_Create,
	(void *)fade_icall_EditorProgress_Dispose,
	(void *)fade_icall_EditorProgress_Step,
	(void *)fade_icall_Internal_FullExportTemplatesDir,
	(void *)fade_icall_Internal_IsMacOSAppBundleInstalled,
	(void *)fade_icall_Internal_LipOCreateFile,
	(void *)fade_icall_Internal_FADEIs32Bits,
	(void *)fade_icall_Internal_FADEIsRealTDouble,
	(void *)fade_icall_Internal_FADEMainIteration,
	(void *)fade_icall_Internal_IsAssembliesReloadingNeeded,
	(void *)fade_icall_Internal_ReloadAssemblies,
	(void *)fade_icall_Internal_EditorDebuggerNodeReloadScripts,
	(void *)fade_icall_Internal_ScriptEditorEdit,
	(void *)fade_icall_Internal_EditorNodeShowScriptScreen,
	(void *)fade_icall_Internal_EditorRunPlay,
	(void *)fade_icall_Internal_EditorRunStop,
	(void *)fade_icall_Internal_EditorPlugin_AddControlToEditorRunBar,
	(void *)fade_icall_Internal_ScriptEditorDebugger_ReloadScripts,
	(void *)fade_icall_Internal_CodeCompletionRequest,
	(void *)fade_icall_Globals_EditorScale,
	(void *)fade_icall_Globals_GlobalDef,
	(void *)fade_icall_Globals_EditorDef,
	(void *)fade_icall_Globals_EditorDefShortcut,
	(void *)fade_icall_Globals_EditorGetShortcut,
	(void *)fade_icall_Globals_EditorShortcutOverride,
	(void *)fade_icall_Globals_TTR,
	(void *)fade_icall_Utils_OS_GetPlatformName,
	(void *)fade_icall_Utils_OS_UnixFileHasExecutableAccess,
};

const void **fadesharp::get_editor_interop_funcs(int32_t &r_size) {
	r_size = sizeof(unmanaged_callbacks);
	return unmanaged_callbacks;
}
