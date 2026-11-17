using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace FADE.SourceGenerators
{
    [Generator]
    public class FADEPluginsInitializerGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.IsFADEToolsProject() || context.IsFADESourceGeneratorDisabled("FADEPluginsInitializer"))
                return;

            string source =
                @"using System;
using System.Runtime.InteropServices;
using FADE.Bridge;
using FADE.NativeInterop;

namespace FADEPlugins.Game
{
    internal static partial class Main
    {
        [UnmanagedCallersOnly(EntryPoint = ""godotsharp_game_main_init"")]
        private static godot_bool InitializeFromGameProject(IntPtr godotDllHandle, IntPtr outManagedCallbacks,
            IntPtr unmanagedCallbacks, int unmanagedCallbacksSize)
        {
            try
            {
                NativeFuncs.Initialize(unmanagedCallbacks, unmanagedCallbacksSize);

                Marshaling.Initialize();

                DllImportResolver dllImportResolver = new FADEDllImportResolver(godotDllHandle).OnResolveDllImport;

                var coreApiAssembly = typeof(global::FADE.FADEObject).Assembly;

                NativeLibrary.SetDllImportResolver(coreApiAssembly, dllImportResolver);

                ManagedCallbacks.Create(outManagedCallbacks);

                ScriptManagerBridge.LookupScriptsInAssembly(typeof(global::FADEPlugins.Game.Main).Assembly);

                return godot_bool.True;
            }
            catch (Exception e)
            {
                global::System.Console.Error.WriteLine(e);
                return false.ToFADEBool();
            }
        }
    }
}
";

            context.AddSource("FADEPlugins.Game.generated",
                SourceText.From(source, Encoding.UTF8));
        }
    }
}
