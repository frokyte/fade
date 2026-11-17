using FADE;
using FADE.NativeInterop;

partial class InheritanceBase
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveFADEObjectData(global::FADE.Bridge.FADESerializationInfo info)
    {
        base.SaveFADEObjectData(info);
        info.AddProperty(PropertyName.@MyString, global::FADE.Variant.From<string>(this.@MyString));
        info.AddProperty(PropertyName.@MyInteger, global::FADE.Variant.From<int>(this.@MyInteger));
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreFADEObjectData(global::FADE.Bridge.FADESerializationInfo info)
    {
        base.RestoreFADEObjectData(info);
        if (info.TryGetProperty(PropertyName.@MyString, out var _value_MyString))
            this.@MyString = _value_MyString.As<string>();
        if (info.TryGetProperty(PropertyName.@MyInteger, out var _value_MyInteger))
            this.@MyInteger = _value_MyInteger.As<int>();
    }
}
