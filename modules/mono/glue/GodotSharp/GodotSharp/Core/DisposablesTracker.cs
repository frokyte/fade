using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using FADE.NativeInterop;

#nullable enable

namespace FADE
{
    internal static class DisposablesTracker
    {
        [UnmanagedCallersOnly]
        internal static void OnFADEShuttingDown()
        {
            try
            {
                OnFADEShuttingDownImpl();
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
            }
        }

        private static void OnFADEShuttingDownImpl()
        {
            bool isStdoutVerbose;

            try
            {
                isStdoutVerbose = OS.IsStdOutVerbose();
            }
            catch (ObjectDisposedException)
            {
                // OS singleton already disposed. Maybe OnUnloading was called twice.
                isStdoutVerbose = false;
            }

            if (isStdoutVerbose)
                GD.Print("Unloading: Disposing tracked instances...");

            // Dispose FADE Objects first, and only then dispose other disposables
            // like StringName, NodePath, FADE.Collections.Array/Dictionary, etc.
            // The FADE Object Dispose() method may need any of the later instances.

            foreach (WeakReference<FADEObject> item in FADEObjectInstances.Keys)
            {
                if (item.TryGetTarget(out FADEObject? self))
                    self.Dispose();
            }

            foreach (WeakReference<IDisposable> item in OtherInstances.Keys)
            {
                if (item.TryGetTarget(out IDisposable? self))
                    self.Dispose();
            }

            if (isStdoutVerbose)
                GD.Print("Unloading: Finished disposing tracked instances.");
        }

        private static ConcurrentDictionary<WeakReference<FADEObject>, byte> FADEObjectInstances { get; } =
            new();

        private static ConcurrentDictionary<WeakReference<IDisposable>, byte> OtherInstances { get; } =
            new();

        public static WeakReference<FADEObject> RegisterFADEObject(FADEObject godotObject)
        {
            var weakReferenceToSelf = new WeakReference<FADEObject>(godotObject);
            FADEObjectInstances.TryAdd(weakReferenceToSelf, 0);
            return weakReferenceToSelf;
        }

        public static WeakReference<IDisposable> RegisterDisposable(IDisposable disposable)
        {
            var weakReferenceToSelf = new WeakReference<IDisposable>(disposable);
            OtherInstances.TryAdd(weakReferenceToSelf, 0);
            return weakReferenceToSelf;
        }

        public static void UnregisterFADEObject(FADEObject godotObject, WeakReference<FADEObject> weakReferenceToSelf)
        {
            if (!FADEObjectInstances.TryRemove(weakReferenceToSelf, out _))
                throw new ArgumentException("FADE Object not registered.", nameof(weakReferenceToSelf));
        }

        public static void UnregisterDisposable(WeakReference<IDisposable> weakReference)
        {
            if (!OtherInstances.TryRemove(weakReference, out _))
                throw new ArgumentException("Disposable not registered.", nameof(weakReference));
        }
    }
}
