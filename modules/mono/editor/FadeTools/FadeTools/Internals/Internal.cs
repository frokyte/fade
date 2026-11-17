#pragma warning disable IDE1006 // Naming rule violation
// ReSharper disable InconsistentNaming

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FADE;
using FADE.NativeInterop;
using FADE.SourceGenerators.Internal;
using FADETools.IdeMessaging.Requests;

namespace FADETools.Internals
{
    [GenerateUnmanagedCallbacks(typeof(InternalUnmanagedCallbacks))]
    internal static partial class Internal
    {
        public const string CSharpLanguageType = "CSharpScript";
        public const string CSharpLanguageExtension = ".cs";

        public static string FullExportTemplatesDir
        {
            get
            {
                fade_icall_Internal_FullExportTemplatesDir(out fade_string dest);
                using (dest)
                    return Marshaling.ConvertStringToManaged(dest);
            }
        }

        public static string SimplifyFADEPath(this string path) => FADE.StringExtensions.SimplifyPath(path);

        public static bool IsMacOSAppBundleInstalled(string bundleId)
        {
            using fade_string bundleIdIn = Marshaling.ConvertStringToNative(bundleId);
            return fade_icall_Internal_IsMacOSAppBundleInstalled(bundleIdIn);
        }

        public static bool LipOCreateFile(string outputPath, string[] files)
        {
            using fade_string outputPathIn = Marshaling.ConvertStringToNative(outputPath);
            using fade_packed_string_array filesIn = Marshaling.ConvertSystemArrayToNativePackedStringArray(files);
            return fade_icall_Internal_LipOCreateFile(outputPathIn, filesIn);
        }

        public static bool FADEIs32Bits() => fade_icall_Internal_FADEIs32Bits();

        public static bool FADEIsRealTDouble() => fade_icall_Internal_FADEIsRealTDouble();

        public static void FADEMainIteration() => fade_icall_Internal_FADEMainIteration();

        public static bool IsAssembliesReloadingNeeded() => fade_icall_Internal_IsAssembliesReloadingNeeded();

        public static void ReloadAssemblies() => fade_icall_Internal_ReloadAssemblies();

        public static void EditorDebuggerNodeReloadScripts() => fade_icall_Internal_EditorDebuggerNodeReloadScripts();

        public static bool ScriptEditorEdit(Resource resource, int line, int col, bool grabFocus = true) =>
            fade_icall_Internal_ScriptEditorEdit(resource.NativeInstance, line, col, grabFocus);

        public static void EditorNodeShowScriptScreen() => fade_icall_Internal_EditorNodeShowScriptScreen();

        public static void EditorRunPlay() => fade_icall_Internal_EditorRunPlay();

        public static void EditorRunStop() => fade_icall_Internal_EditorRunStop();

        public static void EditorPlugin_AddControlToEditorRunBar(Control control) =>
            fade_icall_Internal_EditorPlugin_AddControlToEditorRunBar(control.NativeInstance);

        public static void ScriptEditorDebugger_ReloadScripts() =>
            fade_icall_Internal_ScriptEditorDebugger_ReloadScripts();

        public static string[] CodeCompletionRequest(CodeCompletionRequest.CompletionKind kind,
            string scriptFile)
        {
            using fade_string scriptFileIn = Marshaling.ConvertStringToNative(scriptFile);
            fade_icall_Internal_CodeCompletionRequest((int)kind, scriptFileIn, out fade_packed_string_array res);
            using (res)
                return Marshaling.ConvertNativePackedStringArrayToSystemArray(res);
        }

        #region Internal

        private static bool initialized = false;

        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Global
        internal static unsafe void Initialize(IntPtr unmanagedCallbacks, int unmanagedCallbacksSize)
        {
            if (initialized)
                throw new InvalidOperationException("Already initialized.");
            initialized = true;

            if (unmanagedCallbacksSize != sizeof(InternalUnmanagedCallbacks))
                throw new ArgumentException("Unmanaged callbacks size mismatch.", nameof(unmanagedCallbacksSize));

            _unmanagedCallbacks = Unsafe.AsRef<InternalUnmanagedCallbacks>((void*)unmanagedCallbacks);
        }

        private partial struct InternalUnmanagedCallbacks
        {
        }

        /*
         * IMPORTANT:
         * The order of the methods defined in NativeFuncs must match the order
         * in the array defined at the bottom of 'editor/editor_internal_calls.cpp'.
         */

        public static partial void fade_icall_FADESharpDirs_ResMetadataDir(out fade_string r_dest);

        public static partial void fade_icall_FADESharpDirs_MonoUserDir(out fade_string r_dest);

        public static partial void fade_icall_FADESharpDirs_BuildLogsDirs(out fade_string r_dest);

        public static partial void fade_icall_FADESharpDirs_DataEditorToolsDir(out fade_string r_dest);

        public static partial void fade_icall_FADESharpDirs_CSharpProjectName(out fade_string r_dest);

        public static partial void fade_icall_EditorProgress_Create(in fade_string task, in fade_string label,
            int amount, bool canCancel);

        public static partial void fade_icall_EditorProgress_Dispose(in fade_string task);

        public static partial bool fade_icall_EditorProgress_Step(in fade_string task, in fade_string state,
            int step,
            bool forceRefresh);

        private static partial void fade_icall_Internal_FullExportTemplatesDir(out fade_string dest);

        private static partial bool fade_icall_Internal_IsMacOSAppBundleInstalled(in fade_string bundleId);

        private static partial bool fade_icall_Internal_LipOCreateFile(in fade_string outputPath, in fade_packed_string_array files);

        private static partial bool fade_icall_Internal_FADEIs32Bits();

        private static partial bool fade_icall_Internal_FADEIsRealTDouble();

        private static partial void fade_icall_Internal_FADEMainIteration();

        private static partial bool fade_icall_Internal_IsAssembliesReloadingNeeded();

        private static partial void fade_icall_Internal_ReloadAssemblies();

        private static partial void fade_icall_Internal_EditorDebuggerNodeReloadScripts();

        private static partial bool fade_icall_Internal_ScriptEditorEdit(IntPtr resource, int line, int col,
            bool grabFocus);

        private static partial void fade_icall_Internal_EditorNodeShowScriptScreen();

        private static partial void fade_icall_Internal_EditorRunPlay();

        private static partial void fade_icall_Internal_EditorRunStop();

        private static partial void fade_icall_Internal_EditorPlugin_AddControlToEditorRunBar(IntPtr p_control);

        private static partial void fade_icall_Internal_ScriptEditorDebugger_ReloadScripts();

        private static partial void fade_icall_Internal_CodeCompletionRequest(int kind, in fade_string scriptFile,
            out fade_packed_string_array res);

        public static partial float fade_icall_Globals_EditorScale();

        public static partial void fade_icall_Globals_GlobalDef(in fade_string setting, in fade_variant defaultValue,
            bool restartIfChanged, out fade_variant result);

        public static partial void fade_icall_Globals_EditorDef(in fade_string setting, in fade_variant defaultValue,
            bool restartIfChanged, out fade_variant result);

        public static partial void
            fade_icall_Globals_EditorDefShortcut(in fade_string setting, in fade_string name, Key keycode, fade_bool physical, out fade_variant result);

        public static partial void
            fade_icall_Globals_EditorGetShortcut(in fade_string setting, out fade_variant result);

        public static partial void
            fade_icall_Globals_EditorShortcutOverride(in fade_string setting, in fade_string feature, Key keycode, fade_bool physical);

        public static partial void fade_icall_Globals_TTR(in fade_string text, out fade_string dest);

        public static partial void fade_icall_Utils_OS_GetPlatformName(out fade_string dest);

        public static partial bool fade_icall_Utils_OS_UnixFileHasExecutableAccess(in fade_string filePath);

        #endregion
    }
}
