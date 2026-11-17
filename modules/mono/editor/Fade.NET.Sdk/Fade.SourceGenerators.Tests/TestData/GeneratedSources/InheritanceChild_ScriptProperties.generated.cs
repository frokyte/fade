using FADE;
using FADE.NativeInterop;

partial class InheritanceChild
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::InheritanceBase.PropertyName {
        /// <summary>
        /// Cached name for the 'MyString' property.
        /// </summary>
        public new static readonly global::FADE.StringName @MyString = "MyString";
        /// <summary>
        /// Cached name for the 'MyInteger' property.
        /// </summary>
        public new static readonly global::FADE.StringName @MyInteger = "MyInteger";
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEPropertyTrampolines(global::FADE.Bridge.PropertyTrampolineCollector collector)
        {
            static fade_variant trampoline_get_MyString(object fadeObject)
            {
                var ret = ((global::InheritanceChild)fadeObject).@MyString;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<string>(ret);
            }
            static void trampoline_set_MyString(object fadeObject, in fade_variant value)
            {
                ((global::InheritanceChild)fadeObject).@MyString = global::FADE.NativeInterop.VariantUtils.ConvertTo<string>(value);
            }
            static fade_variant trampoline_get_MyInteger(object fadeObject)
            {
                var ret = ((global::InheritanceChild)fadeObject).@MyInteger;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<int>(ret);
            }
            static void trampoline_set_MyInteger(object fadeObject, in fade_variant value)
            {
                ((global::InheritanceChild)fadeObject).@MyInteger = global::FADE.NativeInterop.VariantUtils.ConvertTo<int>(value);
            }
            var aux_delegate_get_MyString = trampoline_get_MyString;
            var aux_delegate_set_MyString = trampoline_set_MyString;
            collector.TryAdd(PropertyName.@MyString, new(aux_delegate_get_MyString.Method.MethodHandle.GetFunctionPointer()), new(aux_delegate_set_MyString.Method.MethodHandle.GetFunctionPointer()));
            var aux_delegate_get_MyInteger = trampoline_get_MyInteger;
            var aux_delegate_set_MyInteger = trampoline_set_MyInteger;
            collector.TryAdd(PropertyName.@MyInteger, new(aux_delegate_get_MyInteger.Method.MethodHandle.GetFunctionPointer()), new(aux_delegate_set_MyInteger.Method.MethodHandle.GetFunctionPointer()));
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
        properties.Add(new(type: (global::FADE.Variant.Type)4, name: PropertyName.@MyString, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4096, exported: false));
        properties.Add(new(type: (global::FADE.Variant.Type)2, name: PropertyName.@MyInteger, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4102, exported: true));
        return properties;
    }
#pragma warning restore CS0109
}
