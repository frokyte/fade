using FADE;
using FADE.NativeInterop;

partial class ScriptBoilerplate
{
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void SaveFADEObjectData(global::FADE.Bridge.FADESerializationInfo info)
    {
        base.SaveFADEObjectData(info);
        info.AddProperty(PropertyName.@_nodePath, global::FADE.Variant.From<global::FADE.NodePath>(this.@_nodePath));
        info.AddProperty(PropertyName.@_velocity, global::FADE.Variant.From<int>(this.@_velocity));
    }
    /// <inheritdoc/>
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    protected override void RestoreFADEObjectData(global::FADE.Bridge.FADESerializationInfo info)
    {
        base.RestoreFADEObjectData(info);
        if (info.TryGetProperty(PropertyName.@_nodePath, out var _value__nodePath))
            this.@_nodePath = _value__nodePath.As<global::FADE.NodePath>();
        if (info.TryGetProperty(PropertyName.@_velocity, out var _value__velocity))
            this.@_velocity = _value__velocity.As<int>();
    }
}
