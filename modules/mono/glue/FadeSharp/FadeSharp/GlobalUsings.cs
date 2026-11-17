#if REAL_T_IS_DOUBLE
global using real_t = System.Double;
#else
global using real_t = System.Single;
#endif

global using unsafe MethodTrampolineDelegate = delegate* managed<
    /* fadeObject: */ object,
    /* args: */ FADE.NativeInterop.NativeVariantPtrArgs,
    /* callError: */ ref FADE.NativeInterop.fade_variant_call_error,
    /* return (value): */ FADE.NativeInterop.fade_variant>;
global using unsafe PropertyGetterTrampolineDelegate = delegate* managed<
    /* fadeObject: */ object,
    /* return (value): */ FADE.NativeInterop.fade_variant>;
global using unsafe PropertySetterTrampolineDelegate = delegate* managed<
    /* fadeObject: */ object,
    /* value: */ in FADE.NativeInterop.fade_variant,
    /* return: */ void>;
global using unsafe RaiseSignalTrampolineDelegate = delegate* managed<
    /* fadeObject: */ object,
    /* args: */ FADE.NativeInterop.NativeVariantPtrArgs,
    /* callError: */ ref FADE.NativeInterop.fade_variant_call_error,
    /* return */ void>;

global using unsafe TryAddMethodTrampolineDelegate = delegate* unmanaged<
    /* scriptPtr: */ System.IntPtr,
    /* name: */ FADE.NativeInterop.fade_string_name*,
    /* argCount: */ int,
    /* trampolineDelegate: */ void*,
    /* isStatic: */ FADE.NativeInterop.fade_bool,
    /* return */ void>;
global using unsafe TryAddPropertyTrampolineDelegate = delegate* unmanaged<
    /* scriptPtr: */ System.IntPtr,
    /* name: */ FADE.NativeInterop.fade_string_name*,
    /* getterTrampolineDelegate: */ void*,
    /* setterTrampolineDelegate: */ void*,
    /* return */ void>;
global using unsafe TryAddRaiseSignalTrampolineDelegate = delegate* unmanaged<
    /* scriptPtr: */ System.IntPtr,
    /* name: */ FADE.NativeInterop.fade_string_name*,
    /* argCount: */ int,
    /* trampolineDelegate: */ void*,
    /* return */ void>;
