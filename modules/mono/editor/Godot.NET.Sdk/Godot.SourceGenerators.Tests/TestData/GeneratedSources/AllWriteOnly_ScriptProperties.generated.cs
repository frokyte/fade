using FADE;
using FADE.NativeInterop;

partial class AllWriteOnly
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::FADE.FADEObject.PropertyName {
        /// <summary>
        /// Cached name for the 'WriteOnlyProperty' property.
        /// </summary>
        public new static readonly global::FADE.StringName @WriteOnlyProperty = "WriteOnlyProperty";
        /// <summary>
        /// Cached name for the '_writeOnlyBackingField' field.
        /// </summary>
        public new static readonly global::FADE.StringName @_writeOnlyBackingField = "_writeOnlyBackingField";
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEPropertyTrampolines(global::FADE.Bridge.PropertyTrampolineCollector collector)
        {
            static void trampoline_set_WriteOnlyProperty(object godotObject, in godot_variant value)
            {
                ((global::AllWriteOnly)godotObject).@WriteOnlyProperty = global::FADE.NativeInterop.VariantUtils.ConvertTo<bool>(value);
            }
            static godot_variant trampoline_get__writeOnlyBackingField(object godotObject)
            {
                var ret = ((global::AllWriteOnly)godotObject).@_writeOnlyBackingField;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<bool>(ret);
            }
            static void trampoline_set__writeOnlyBackingField(object godotObject, in godot_variant value)
            {
                ((global::AllWriteOnly)godotObject).@_writeOnlyBackingField = global::FADE.NativeInterop.VariantUtils.ConvertTo<bool>(value);
            }
            var aux_delegate_set_WriteOnlyProperty = trampoline_set_WriteOnlyProperty;
            collector.TryAdd(PropertyName.@WriteOnlyProperty, new(global::System.IntPtr.Zero), new(aux_delegate_set_WriteOnlyProperty.Method.MethodHandle.GetFunctionPointer()));
            var aux_delegate_get__writeOnlyBackingField = trampoline_get__writeOnlyBackingField;
            var aux_delegate_set__writeOnlyBackingField = trampoline_set__writeOnlyBackingField;
            collector.TryAdd(PropertyName.@_writeOnlyBackingField, new(aux_delegate_get__writeOnlyBackingField.Method.MethodHandle.GetFunctionPointer()), new(aux_delegate_set__writeOnlyBackingField.Method.MethodHandle.GetFunctionPointer()));
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
        properties.Add(new(type: (global::FADE.Variant.Type)1, name: PropertyName.@_writeOnlyBackingField, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::FADE.Variant.Type)1, name: PropertyName.@WriteOnlyProperty, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4096, exported: false));
        return properties;
    }
#pragma warning restore CS0109
}
