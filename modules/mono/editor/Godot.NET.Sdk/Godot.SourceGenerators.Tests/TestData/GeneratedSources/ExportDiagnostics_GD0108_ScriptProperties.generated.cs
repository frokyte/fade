using FADE;
using FADE.NativeInterop;

partial class ExportDiagnostics_GD0108
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::FADE.Node.PropertyName {
        /// <summary>
        /// Cached name for the 'MyButton' property.
        /// </summary>
        public new static readonly global::FADE.StringName @MyButton = "MyButton";
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEPropertyTrampolines(global::FADE.Bridge.PropertyTrampolineCollector collector)
        {
            static godot_variant trampoline_get_MyButton(object godotObject)
            {
                var ret = ((global::ExportDiagnostics_GD0108)godotObject).@MyButton;
                return global::FADE.NativeInterop.VariantUtils.CreateFrom<global::FADE.Callable>(ret);
            }
            var aux_delegate_get_MyButton = trampoline_get_MyButton;
            collector.TryAdd(PropertyName.@MyButton, new(aux_delegate_get_MyButton.Method.MethodHandle.GetFunctionPointer()), new(global::System.IntPtr.Zero));
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
        return properties;
    }
#pragma warning restore CS0109
}
