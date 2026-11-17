using FADE;
using FADE.NativeInterop;

partial class AbstractGenericNode<T>
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::FADE.Node.PropertyName {
        /// <summary>
        /// Cached name for the 'MyArray' property.
        /// </summary>
        public new static readonly global::FADE.StringName @MyArray = "MyArray";
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEPropertyTrampolines(global::FADE.Bridge.PropertyTrampolineCollector collector)
        {
            static fade_variant trampoline_get_MyArray(object fadeObject)
            {
                var ret = ((global::AbstractGenericNode<T>)fadeObject).@MyArray;
                return global::FADE.NativeInterop.VariantUtils.CreateFromArray(ret);
            }
            static void trampoline_set_MyArray(object fadeObject, in fade_variant value)
            {
                ((global::AbstractGenericNode<T>)fadeObject).@MyArray = global::FADE.NativeInterop.VariantUtils.ConvertToArray<T>(value);
            }
            var aux_delegate_get_MyArray = trampoline_get_MyArray;
            var aux_delegate_set_MyArray = trampoline_set_MyArray;
            collector.TryAdd(PropertyName.@MyArray, new(aux_delegate_get_MyArray.Method.MethodHandle.GetFunctionPointer()), new(aux_delegate_set_MyArray.Method.MethodHandle.GetFunctionPointer()));
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
        properties.Add(new(type: (global::FADE.Variant.Type)28, name: PropertyName.@MyArray, hint: (global::FADE.PropertyHint)0, hintString: "", usage: (global::FADE.PropertyUsageFlags)4102, exported: true));
        return properties;
    }
#pragma warning restore CS0109
}
