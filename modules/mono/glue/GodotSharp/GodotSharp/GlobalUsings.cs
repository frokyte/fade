#if REAL_T_IS_DOUBLE
global using real_t = System.Double;
#else
global using real_t = System.Single;
#endif

global using unsafe MethodTrampolineDelegate = delegate* managed<
    /* godotObject: */ object,
    /* args: */ FADE.NativeInterop.NativeVariantPtrArgs,
    /* callError: */ ref FADE.NativeInterop.godot_variant_call_error,
    /* return (value): */ FADE.NativeInterop.godot_variant>;
global using unsafe PropertyGetterTrampolineDelegate = delegate* managed<
    /* godotObject: */ object,
    /* return (value): */ FADE.NativeInterop.godot_variant>;
global using unsafe PropertySetterTrampolineDelegate = delegate* managed<
    /* godotObject: */ object,
    /* value: */ in FADE.NativeInterop.godot_variant,
    /* return: */ void>;
global using unsafe RaiseSignalTrampolineDelegate = delegate* managed<
    /* godotObject: */ object,
    /* args: */ FADE.NativeInterop.NativeVariantPtrArgs,
    /* callError: */ ref FADE.NativeInterop.godot_variant_call_error,
    /* return */ void>;

global using unsafe TryAddMethodTrampolineDelegate = delegate* unmanaged<
    /* scriptPtr: */ System.IntPtr,
    /* name: */ FADE.NativeInterop.godot_string_name*,
    /* argCount: */ int,
    /* trampolineDelegate: */ void*,
    /* isStatic: */ FADE.NativeInterop.godot_bool,
    /* return */ void>;
global using unsafe TryAddPropertyTrampolineDelegate = delegate* unmanaged<
    /* scriptPtr: */ System.IntPtr,
    /* name: */ FADE.NativeInterop.godot_string_name*,
    /* getterTrampolineDelegate: */ void*,
    /* setterTrampolineDelegate: */ void*,
    /* return */ void>;
global using unsafe TryAddRaiseSignalTrampolineDelegate = delegate* unmanaged<
    /* scriptPtr: */ System.IntPtr,
    /* name: */ FADE.NativeInterop.godot_string_name*,
    /* argCount: */ int,
    /* trampolineDelegate: */ void*,
    /* return */ void>;
