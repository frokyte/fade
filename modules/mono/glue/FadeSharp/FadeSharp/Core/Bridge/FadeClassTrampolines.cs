using System;

namespace FADE.Bridge;

#nullable enable

/// <summary>
/// Struct representing a method trampoline, containing a delegate to the trampoline function.
/// The trampoline is a <c>delegate* managed</c> with a signature equivalent to the following C# delegate:
/// <code>
/// delegate fade_variant MethodTrampoline(object fadeObject, NativeVariantPtrArgs args, ref fade_variant_call_error callError);
/// </code>
/// </summary>
public readonly struct MethodTrampoline
{
    public unsafe MethodTrampoline(MethodTrampolineDelegate trampolineDelegate, bool isStatic)
    {
        TrampolineDelegate = trampolineDelegate;
        IsStatic = isStatic;
    }

    public unsafe MethodTrampoline(IntPtr trampolineDelegate, bool isStatic)
    {
        TrampolineDelegate = (MethodTrampolineDelegate)trampolineDelegate;
        IsStatic = isStatic;
    }

    public unsafe MethodTrampolineDelegate TrampolineDelegate { get; }

    public bool IsStatic { get; }
}

/// <summary>
/// Struct representing a property getter trampoline, containing a delegate to the trampoline function.
/// The trampoline is a <c>delegate* managed</c> with a signature equivalent to the following C# delegate:
/// <code>
/// delegate fade_variant PropertyGetterTrampoline(object fadeObject);
/// </code>
/// </summary>
public readonly struct PropertyGetterTrampoline
{
    public unsafe PropertyGetterTrampoline(PropertyGetterTrampolineDelegate trampolineDelegate)
        => TrampolineDelegate = trampolineDelegate;

    public unsafe PropertyGetterTrampoline(IntPtr trampolineDelegate)
        => TrampolineDelegate = (PropertyGetterTrampolineDelegate)trampolineDelegate;

    public unsafe PropertyGetterTrampolineDelegate TrampolineDelegate { get; }
}

/// <summary>
/// Struct representing a property setter trampoline, containing a delegate to the trampoline function.
/// The trampoline is a <c>delegate* managed</c> with a signature equivalent to the following C# delegate:
/// <code>
/// delegate void PropertySetterTrampoline(object fadeObject, in fade_variant value);
/// </code>
/// </summary>
public readonly struct PropertySetterTrampoline
{
    public unsafe PropertySetterTrampoline(PropertySetterTrampolineDelegate trampolineDelegate)
        => TrampolineDelegate = trampolineDelegate;

    public unsafe PropertySetterTrampoline(IntPtr trampolineDelegate)
        => TrampolineDelegate = (PropertySetterTrampolineDelegate)trampolineDelegate;

    public unsafe PropertySetterTrampolineDelegate TrampolineDelegate { get; }
}

/// <summary>
/// Struct representing a raise signal trampoline, containing a delegate to the trampoline function.
/// The trampoline is a <c>delegate* managed</c> with a signature equivalent to the following C# delegate:
/// <code>
/// delegate void RaiseSignalTrampoline(object fadeObject, NativeVariantPtrArgs args);
/// </code>
/// </summary>
public readonly struct RaiseSignalTrampoline
{
    public unsafe RaiseSignalTrampoline(RaiseSignalTrampolineDelegate trampolineDelegate)
        => TrampolineDelegate = trampolineDelegate;

    public unsafe RaiseSignalTrampoline(IntPtr trampolineDelegate)
        => TrampolineDelegate = (RaiseSignalTrampolineDelegate)trampolineDelegate;

    public unsafe RaiseSignalTrampolineDelegate TrampolineDelegate { get; }
}
