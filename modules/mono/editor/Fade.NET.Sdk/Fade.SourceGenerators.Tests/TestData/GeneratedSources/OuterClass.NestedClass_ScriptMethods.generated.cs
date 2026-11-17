using FADE;
using FADE.NativeInterop;

partial struct OuterClass
{
partial class NestedClass
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the methods contained in this class, for fast lookup.
    /// </summary>
    public new class MethodName : global::FADE.RefCounted.MethodName {
        /// <summary>
        /// Cached name for the '_Get' method.
        /// </summary>
        public new static readonly global::FADE.StringName @_Get = "_Get";
    }
    /// <summary>
    /// Get the method information for all the methods declared in this class.
    /// This method is used by FADE to register the available methods in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::FADE.Bridge.MethodInfo> GetFADEMethodList()
    {
        var methods = new global::System.Collections.Generic.List<global::FADE.Bridge.MethodInfo>(1);
        methods.Add(new(name: MethodName.@_Get, returnVal: new(type: (global::FADE.Variant.Type)0, name: "", hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)131078, exported: false), flags: (global::FADE.MethodFlags)1, arguments: new() { new(type: (global::FADE.Variant.Type)21, name: "property", hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)6, exported: false),  }, defaultArguments: null));
        return methods;
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEMethodTrampolines(global::FADE.Bridge.MethodTrampolineCollector collector)
        {
            static fade_variant trampoline_1__Get(object fadeObject, NativeVariantPtrArgs args, ref fade_variant_call_error callError)
            {
                if (args.Count != 1) {
                    callError = fade_variant_call_error.CreateInvalidArgumentCountError(expected: 1, provided: args.Count);
                    return default;
                }
                var callRet = ((global::FADE.FADEObject)fadeObject).@_Get(global::FADE.NativeInterop.VariantUtils.ConvertTo<global::FADE.StringName>(args[0]));
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<global::FADE.Variant>(callRet);
            }
            var aux_delegate_1__Get = trampoline_1__Get;
            collector.TryAdd(new(MethodName.@_Get, 1), new(aux_delegate_1__Get.Method.MethodHandle.GetFunctionPointer(), isStatic: false));
            collector.TryAdd(new(global::FADE.FADEObject.MethodName.@_Get, 1), new(aux_delegate_1__Get.Method.MethodHandle.GetFunctionPointer(), isStatic: false));
        }
    }
#pragma warning restore CS0109
}
}
