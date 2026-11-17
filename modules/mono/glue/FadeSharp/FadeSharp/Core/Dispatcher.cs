using System;
using System.Runtime.InteropServices;
using FADE.NativeInterop;

namespace FADE
{
    public static class Dispatcher
    {
        internal static FADETaskScheduler DefaultFADETaskScheduler;

        internal static void InitializeDefaultFADETaskScheduler()
        {
            DefaultFADETaskScheduler?.Dispose();
            DefaultFADETaskScheduler = new FADETaskScheduler();
        }

        public static FADESynchronizationContext SynchronizationContext => DefaultFADETaskScheduler.Context;
    }
}
