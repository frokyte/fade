using FADE;
using FADE.NativeInterop;

partial class GenericClass<T>
{
partial class NestedClass
{
#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword
    /// <summary>
    /// Cached StringNames for the methods contained in this class, for fast lookup.
    /// </summary>
    public new class MethodName : global::FADE.FADEObject.MethodName {
    }
    protected internal new static partial class FADEInternal
    {
        public static void GetFADEMethodTrampolines(global::FADE.Bridge.MethodTrampolineCollector collector)
        {
        }
    }
#pragma warning restore CS0109
}
}
