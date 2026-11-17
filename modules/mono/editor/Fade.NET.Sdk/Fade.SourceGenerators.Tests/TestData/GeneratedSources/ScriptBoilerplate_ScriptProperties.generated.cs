using FADE;
using FADE.NativeInterop;

partial class ScriptBoilerplate
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::FADE.Node.PropertyName {
        /// <summary>
        /// Cached name for the '_nodePath' field.
        /// </summary>
        public new static readonly global::FADE.StringName @_nodePath = "_nodePath";
        /// <summary>
        /// Cached name for the '_velocity' field.
        /// </summary>
        public new static readonly global::FADE.StringName @_velocity = "_velocity";
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEPropertyTrampolines(global::FADE.Bridge.PropertyTrampolineCollector collector)
        {
            static fade_variant trampoline_get__nodePath(object fadeObject)
            {
                var ret = ((global::ScriptBoilerplate)fadeObject).@_nodePath;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<global::FADE.NodePath>(ret);
            }
            static void trampoline_set__nodePath(object fadeObject, in fade_variant value)
            {
                ((global::ScriptBoilerplate)fadeObject).@_nodePath = global::FADE.NativeInterop.VariantUtils.ConvertTo<global::FADE.NodePath>(value);
            }
            static fade_variant trampoline_get__velocity(object fadeObject)
            {
                var ret = ((global::ScriptBoilerplate)fadeObject).@_velocity;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<int>(ret);
            }
            static void trampoline_set__velocity(object fadeObject, in fade_variant value)
            {
                ((global::ScriptBoilerplate)fadeObject).@_velocity = global::FADE.NativeInterop.VariantUtils.ConvertTo<int>(value);
            }
            var aux_delegate_get__nodePath = trampoline_get__nodePath;
            var aux_delegate_set__nodePath = trampoline_set__nodePath;
            collector.TryAdd(PropertyName.@_nodePath, new(aux_delegate_get__nodePath.Method.MethodHandle.GetFunctionPointer()), new(aux_delegate_set__nodePath.Method.MethodHandle.GetFunctionPointer()));
            var aux_delegate_get__velocity = trampoline_get__velocity;
            var aux_delegate_set__velocity = trampoline_set__velocity;
            collector.TryAdd(PropertyName.@_velocity, new(aux_delegate_get__velocity.Method.MethodHandle.GetFunctionPointer()), new(aux_delegate_set__velocity.Method.MethodHandle.GetFunctionPointer()));
        }
    }
    /// <summary>
    /// Get the property information for all the properties declared in this class.
    /// This method is used by FADE to register the available properties in the editor.
    /// Do not call this method.
    /// </summary>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    internal new static global::System.Collections.Generic.List<global::FADE.Bridge.PropertyInfo> GetFADEPropertyList()
    {
        var properties = new global::System.Collections.Generic.List<global::FADE.Bridge.PropertyInfo>();
        properties.Add(new(type: (global::FADE.Variant.Type)22, name: PropertyName.@_nodePath, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::FADE.Variant.Type)2, name: PropertyName.@_velocity, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4096, exported: false));
        return properties;
    }
#pragma warning restore CS0109
}
