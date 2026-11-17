using FADE;
using FADE.NativeInterop;

partial class AllReadOnly
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::FADE.FADEObject.PropertyName {
        /// <summary>
        /// Cached name for the 'ReadOnlyAutoProperty' property.
        /// </summary>
        public new static readonly global::FADE.StringName @ReadOnlyAutoProperty = "ReadOnlyAutoProperty";
        /// <summary>
        /// Cached name for the 'ReadOnlyProperty' property.
        /// </summary>
        public new static readonly global::FADE.StringName @ReadOnlyProperty = "ReadOnlyProperty";
        /// <summary>
        /// Cached name for the 'InitOnlyAutoProperty' property.
        /// </summary>
        public new static readonly global::FADE.StringName @InitOnlyAutoProperty = "InitOnlyAutoProperty";
        /// <summary>
        /// Cached name for the 'ReadOnlyField' field.
        /// </summary>
        public new static readonly global::FADE.StringName @ReadOnlyField = "ReadOnlyField";
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEPropertyTrampolines(global::FADE.Bridge.PropertyTrampolineCollector collector)
        {
            static fade_variant trampoline_get_ReadOnlyAutoProperty(object fadeObject)
            {
                var ret = ((global::AllReadOnly)fadeObject).@ReadOnlyAutoProperty;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<string>(ret);
            }
            static fade_variant trampoline_get_ReadOnlyProperty(object fadeObject)
            {
                var ret = ((global::AllReadOnly)fadeObject).@ReadOnlyProperty;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<string>(ret);
            }
            static fade_variant trampoline_get_InitOnlyAutoProperty(object fadeObject)
            {
                var ret = ((global::AllReadOnly)fadeObject).@InitOnlyAutoProperty;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<string>(ret);
            }
            static fade_variant trampoline_get_ReadOnlyField(object fadeObject)
            {
                var ret = ((global::AllReadOnly)fadeObject).@ReadOnlyField;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<string>(ret);
            }
            var aux_delegate_get_ReadOnlyAutoProperty = trampoline_get_ReadOnlyAutoProperty;
            collector.TryAdd(PropertyName.@ReadOnlyAutoProperty, new(aux_delegate_get_ReadOnlyAutoProperty.Method.MethodHandle.GetFunctionPointer()), new(global::System.IntPtr.Zero));
            var aux_delegate_get_ReadOnlyProperty = trampoline_get_ReadOnlyProperty;
            collector.TryAdd(PropertyName.@ReadOnlyProperty, new(aux_delegate_get_ReadOnlyProperty.Method.MethodHandle.GetFunctionPointer()), new(global::System.IntPtr.Zero));
            var aux_delegate_get_InitOnlyAutoProperty = trampoline_get_InitOnlyAutoProperty;
            collector.TryAdd(PropertyName.@InitOnlyAutoProperty, new(aux_delegate_get_InitOnlyAutoProperty.Method.MethodHandle.GetFunctionPointer()), new(global::System.IntPtr.Zero));
            var aux_delegate_get_ReadOnlyField = trampoline_get_ReadOnlyField;
            collector.TryAdd(PropertyName.@ReadOnlyField, new(aux_delegate_get_ReadOnlyField.Method.MethodHandle.GetFunctionPointer()), new(global::System.IntPtr.Zero));
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
        properties.Add(new(type: (global::FADE.Variant.Type)4, name: PropertyName.@ReadOnlyField, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::FADE.Variant.Type)4, name: PropertyName.@ReadOnlyAutoProperty, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::FADE.Variant.Type)4, name: PropertyName.@ReadOnlyProperty, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::FADE.Variant.Type)4, name: PropertyName.@InitOnlyAutoProperty, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4096, exported: false));
        return properties;
    }
#pragma warning restore CS0109
}
