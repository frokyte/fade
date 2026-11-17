using FADE;
using FADE.NativeInterop;

partial struct OuterClass
{
partial class NestedClass
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveFADEObjectData(global::FADE.Bridge.FADESerializationInfo info)
    {
        base.SaveFADEObjectData(info);
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreFADEObjectData(global::FADE.Bridge.FADESerializationInfo info)
    {
        base.RestoreFADEObjectData(info);
    }
}
}
