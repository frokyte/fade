using FADE;
using FADE.NativeInterop;

partial struct OuterClass
{
partial class NestedClass
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the properties and fields contained in this class, for fast lookup.
    /// </summary>
    public new class PropertyName : global::FADE.RefCounted.PropertyName {
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEPropertyTrampolines(global::FADE.Bridge.PropertyTrampolineCollector collector)
        {
        }
    }
#pragma warning restore CS0109
}
}
