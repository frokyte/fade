using FADE;
using FADE.NativeInterop;

partial class ScriptBoilerplate
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the methods contained in this class, for fast lookup.
    /// </summary>
    public new class MethodName : global::FADE.Node.MethodName {
        /// <summary>
        /// Cached name for the '_Process' method.
        /// </summary>
        public new static readonly global::FADE.StringName @_Process = "_Process";
        /// <summary>
        /// Cached name for the 'Bazz' method.
        /// </summary>
        public new static readonly global::FADE.StringName @Bazz = "Bazz";
    }
    /// <summary>
    /// Get the method information for all the methods declared in this class.
    /// This method is used by FADE to register the available methods in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::FADE.Bridge.MethodInfo> GetFADEMethodList()
    {
        var methods = new global::System.Collections.Generic.List<global::FADE.Bridge.MethodInfo>(2);
        methods.Add(new(name: MethodName.@_Process, returnVal: new(type: (global::FADE.Variant.Type)0, name: "", hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)6, exported: false), flags: (global::FADE.MethodFlags)1, arguments: new() { new(type: (global::FADE.Variant.Type)3, name: "delta", hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)6, exported: false),  }, defaultArguments: null));
        methods.Add(new(name: MethodName.@Bazz, returnVal: new(type: (global::FADE.Variant.Type)2, name: "", hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)6, exported: false), flags: (global::FADE.MethodFlags)1, arguments: new() { new(type: (global::FADE.Variant.Type)21, name: "name", hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)6, exported: false),  }, defaultArguments: null));
        return methods;
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEMethodTrampolines(global::FADE.Bridge.MethodTrampolineCollector collector)
        {
            static fade_variant trampoline_1__Process(object fadeObject, NativeVariantPtrArgs args, ref fade_variant_call_error callError)
            {
                if (args.Count != 1) {
                    callError = fade_variant_call_error.CreateInvalidArgumentCountError(expected: 1, provided: args.Count);
                    return default;
                }
                ((global::FADE.Node)fadeObject).@_Process(global::FADE.NativeInterop.VariantUtils.ConvertTo<double>(args[0]));
                return default;
            }
            var aux_delegate_1__Process = trampoline_1__Process;
            collector.TryAdd(new(MethodName.@_Process, 1), new(aux_delegate_1__Process.Method.MethodHandle.GetFunctionPointer(), isStatic: false));
            collector.TryAdd(new(global::FADE.Node.MethodName.@_Process, 1), new(aux_delegate_1__Process.Method.MethodHandle.GetFunctionPointer(), isStatic: false));
            static fade_variant trampoline_1_Bazz(object fadeObject, NativeVariantPtrArgs args, ref fade_variant_call_error callError)
            {
                if (args.Count != 1) {
                    callError = fade_variant_call_error.CreateInvalidArgumentCountError(expected: 1, provided: args.Count);
                    return default;
                }
                var callRet = ((global::ScriptBoilerplate)fadeObject).@Bazz(global::FADE.NativeInterop.VariantUtils.ConvertTo<global::FADE.StringName>(args[0]));
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<int>(callRet);
            }
            var aux_delegate_1_Bazz = trampoline_1_Bazz;
            collector.TryAdd(new(MethodName.@Bazz, 1), new(aux_delegate_1_Bazz.Method.MethodHandle.GetFunctionPointer(), isStatic: false));
        }
    }
#pragma warning restore CS0109
}
