using System;
using System.Runtime.InteropServices;
using FADE.NativeInterop;

namespace FADE.Bridge
{
    internal static class GCHandleBridge
    {
        [UnmanagedCallersOnly]
        internal static void FreeGCHandle(IntPtr gcHandlePtr)
        {
            try
            {
                CustomGCHandle.Free(GCHandle.FromIntPtr(gcHandlePtr));
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
            }
        }

        // Returns true, if releasing the provided handle is necessary for assembly unloading to succeed.
        // This check is not perfect and only intended to prevent things in FADETools from being reloaded.
        [UnmanagedCallersOnly]
        internal static fade_bool GCHandleIsTargetCollectible(IntPtr gcHandlePtr)
        {
            try
            {
                var target = GCHandle.FromIntPtr(gcHandlePtr).Target;

                if (target is Delegate @delegate)
                    return DelegateUtils.IsDelegateCollectible(@delegate).ToFADEBool();

                return target.GetType().IsCollectible.ToFADEBool();
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                return fade_bool.True;
            }
        }
    }
}
