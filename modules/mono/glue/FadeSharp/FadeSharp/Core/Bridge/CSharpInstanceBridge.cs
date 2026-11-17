using System;
using System.Runtime.InteropServices;
using FADE.NativeInterop;

namespace FADE.Bridge
{
    internal static class CSharpInstanceBridge
    {
        [UnmanagedCallersOnly]
        internal static unsafe fade_bool Call(IntPtr fadeObjectGCHandle, fade_string_name* method,
            fade_variant** args, int argCount, fade_variant_call_error* refCallError, fade_variant* outRet)
        {
            try
            {
                var fadeObject = (FADEObject)GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (fadeObject == null)
                {
                    *outRet = default;
                    (*refCallError).Error = fade_variant_call_error_error.GODOT_CALL_ERROR_CALL_ERROR_INSTANCE_IS_NULL;
                    return fade_bool.False;
                }

                bool methodInvoked = fadeObject.InvokeFADEClassMethod(CustomUnsafe.AsRef(method),
                    new NativeVariantPtrArgs(args, argCount), out fade_variant retValue);

                if (!methodInvoked)
                {
                    *outRet = default;
                    // This is important, as it tells Object::call that no method was called.
                    // Otherwise, it would prevent Object::call from calling native methods.
                    (*refCallError).Error = fade_variant_call_error_error.GODOT_CALL_ERROR_CALL_ERROR_INVALID_METHOD;
                    return fade_bool.False;
                }

                *outRet = retValue;
                return fade_bool.True;
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                *outRet = default;
                return fade_bool.False;
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe fade_bool Set(IntPtr fadeObjectGCHandle, fade_string_name* name, fade_variant* value)
        {
            try
            {
                var fadeObject = (FADEObject)GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (fadeObject == null)
                    throw new InvalidOperationException("FADE Object instance is null.");

                if (fadeObject.SetFADEClassPropertyValue(CustomUnsafe.AsRef(name), CustomUnsafe.AsRef(value)))
                {
                    return fade_bool.True;
                }

                if (!fadeObject.HasFADEClassMethod(FADEObject.MethodName._Set.NativeValue.DangerousSelfRef))
                {
                    return fade_bool.False;
                }

                var nameManaged = StringName.CreateTakingOwnershipOfDisposableValue(
                    NativeFuncs.fadesharp_string_name_new_copy(CustomUnsafe.AsRef(name)));

                Variant valueManaged = Variant.CreateCopyingBorrowed(*value);

                return fadeObject._Set(nameManaged, valueManaged).ToFADEBool();
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                return fade_bool.False;
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe fade_bool Get(IntPtr fadeObjectGCHandle, fade_string_name* name,
            fade_variant* outRet)
        {
            try
            {
                var fadeObject = (FADEObject)GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (fadeObject == null)
                    throw new InvalidOperationException("FADE Object instance is null.");

                // Properties
                if (fadeObject.GetFADEClassPropertyValue(CustomUnsafe.AsRef(name), out fade_variant outRetValue))
                {
                    *outRet = outRetValue;
                    return fade_bool.True;
                }

                // Signals
                if (fadeObject.HasFADEClassSignal(CustomUnsafe.AsRef(name)))
                {
                    fade_signal signal = new fade_signal(NativeFuncs.fadesharp_string_name_new_copy(*name),
                        fadeObject.GetInstanceId());
                    *outRet = VariantUtils.CreateFromSignalTakingOwnershipOfDisposableValue(signal);
                    return fade_bool.True;
                }

                // Methods
                if (fadeObject.HasFADEClassMethod(CustomUnsafe.AsRef(name)))
                {
                    fade_callable method = new fade_callable(NativeFuncs.fadesharp_string_name_new_copy(*name),
                        fadeObject.GetInstanceId());
                    *outRet = VariantUtils.CreateFromCallableTakingOwnershipOfDisposableValue(method);
                    return fade_bool.True;
                }

                if (!fadeObject.HasFADEClassMethod(FADEObject.MethodName._Get.NativeValue.DangerousSelfRef))
                {
                    return fade_bool.False;
                }

                var nameManaged = StringName.CreateTakingOwnershipOfDisposableValue(
                    NativeFuncs.fadesharp_string_name_new_copy(CustomUnsafe.AsRef(name)));

                Variant ret = fadeObject._Get(nameManaged);

                if (ret.VariantType == Variant.Type.Nil)
                {
                    *outRet = default;
                    return fade_bool.False;
                }

                *outRet = ret.CopyNativeVariant();
                return fade_bool.True;
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                *outRet = default;
                return fade_bool.False;
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe fade_bool CallViaTrampoline(
            MethodTrampolineDelegate methodTrampoline,
            IntPtr fadeObjectGCHandle, fade_variant** args, int argCount,
            fade_variant_call_error* refCallError, fade_variant* outRet)
        {
            try
            {
                object fadeObject = GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (fadeObject == null)
                {
                    *outRet = default;
                    (*refCallError).Error = fade_variant_call_error_error.GODOT_CALL_ERROR_CALL_ERROR_INSTANCE_IS_NULL;
                    return fade_bool.False;
                }

                *outRet = methodTrampoline(fadeObject, new NativeVariantPtrArgs(args, argCount), ref *refCallError);
                return fade_bool.True;
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                *outRet = default;
                return fade_bool.False;
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe fade_bool SetViaTrampoline(
            PropertySetterTrampolineDelegate setterTrampoline,
            IntPtr fadeObjectGCHandle, fade_variant* value)
        {
            try
            {
                object fadeObject = GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (fadeObject == null)
                    throw new InvalidOperationException("FADE Object instance is null.");

                setterTrampoline(fadeObject, *value);
                return fade_bool.True;
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                return fade_bool.False;
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe fade_bool GetViaTrampoline(
            PropertyGetterTrampolineDelegate getterTrampoline,
            IntPtr fadeObjectGCHandle, fade_variant* outRet)
        {
            try
            {
                object fadeObject = GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (fadeObject == null)
                    throw new InvalidOperationException("FADE Object instance is null.");

                *outRet = getterTrampoline(fadeObject);
                return fade_bool.True;
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                *outRet = default;
                return fade_bool.False;
            }
        }

        [UnmanagedCallersOnly]
        internal static void CallDispose(IntPtr fadeObjectGCHandle, fade_bool okIfNull)
        {
            try
            {
                var fadeObject = (FADEObject)GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (okIfNull.ToBool())
                    fadeObject?.Dispose();
                else
                    fadeObject!.Dispose();
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe void CallToString(IntPtr fadeObjectGCHandle, fade_string* outRes, fade_bool* outValid)
        {
            try
            {
                var self = (FADEObject)GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (self == null)
                {
                    *outRes = default;
                    *outValid = fade_bool.False;
                    return;
                }

                var resultStr = self.ToString();

                if (resultStr == null)
                {
                    *outRes = default;
                    *outValid = fade_bool.False;
                    return;
                }

                *outRes = Marshaling.ConvertStringToNative(resultStr);
                *outValid = fade_bool.True;
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                *outRes = default;
                *outValid = fade_bool.False;
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe fade_bool HasMethodUnknownParams(IntPtr fadeObjectGCHandle, fade_string_name* method)
        {
            try
            {
                var fadeObject = (FADEObject)GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (fadeObject == null)
                    return fade_bool.False;

                return fadeObject.HasFADEClassMethod(CustomUnsafe.AsRef(method)).ToFADEBool();
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                return fade_bool.False;
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe void SerializeState(
            IntPtr fadeObjectGCHandle,
            fade_dictionary* propertiesState,
            fade_dictionary* signalEventsState
        )
        {
            try
            {
                var fadeObject = (FADEObject)GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (fadeObject == null)
                    return;

                // Call OnBeforeSerialize

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (fadeObject is ISerializationListener serializationListener)
                    serializationListener.OnBeforeSerialize();

                // Save instance state

                using var info = FADESerializationInfo.CreateCopyingBorrowed(
                    *propertiesState, *signalEventsState);

                fadeObject.SaveFADEObjectData(info);
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
            }
        }

        [UnmanagedCallersOnly]
        internal static unsafe void DeserializeState(
            IntPtr fadeObjectGCHandle,
            fade_dictionary* propertiesState,
            fade_dictionary* signalEventsState
        )
        {
            try
            {
                var fadeObject = (FADEObject)GCHandle.FromIntPtr(fadeObjectGCHandle).Target;

                if (fadeObject == null)
                    return;

                // Restore instance state

                using var info = FADESerializationInfo.CreateCopyingBorrowed(
                    *propertiesState, *signalEventsState);

                fadeObject.RestoreFADEObjectData(info);

                // Call OnAfterDeserialize

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (fadeObject is ISerializationListener serializationListener)
                    serializationListener.OnAfterDeserialize();
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
            }
        }
    }
}
