using System;
using System.Runtime.InteropServices;
using FADE.NativeInterop;

namespace FADE
{
    public class SignalAwaiter : IAwaiter<Variant[]>, IAwaitable<Variant[]>
    {
        private bool _completed;
        private Variant[] _result;
        private Action _continuation;

        public SignalAwaiter(FADEObject source, StringName signal, FADEObject target)
        {
            var awaiterGcHandle = CustomGCHandle.AllocStrong(this);
            using fade_string_name signalSrc = NativeFuncs.fadesharp_string_name_new_copy(
                (fade_string_name)(signal?.NativeValue ?? default));
            NativeFuncs.fadesharp_internal_signal_awaiter_connect(FADEObject.GetPtr(source), in signalSrc,
                FADEObject.GetPtr(target), GCHandle.ToIntPtr(awaiterGcHandle));
        }

        public bool IsCompleted => _completed;

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        public Variant[] GetResult() => _result;

        public IAwaiter<Variant[]> GetAwaiter() => this;

        [UnmanagedCallersOnly]
        internal static unsafe void SignalCallback(IntPtr awaiterGCHandlePtr, fade_variant** args, int argCount,
            fade_bool* outAwaiterIsNull)
        {
            try
            {
                var awaiter = (SignalAwaiter)GCHandle.FromIntPtr(awaiterGCHandlePtr).Target;

                if (awaiter == null)
                {
                    *outAwaiterIsNull = fade_bool.True;
                    return;
                }

                *outAwaiterIsNull = fade_bool.False;

                awaiter._completed = true;

                if (argCount > 0)
                {
                    Variant[] signalArgs = new Variant[argCount];

                    for (int i = 0; i < argCount; i++)
                        signalArgs[i] = Variant.CreateCopyingBorrowed(*args[i]);

                    awaiter._result = signalArgs;
                }
                else
                {
                    awaiter._result = [];
                }

                awaiter._continuation?.Invoke();
            }
            catch (Exception e)
            {
                ExceptionUtils.LogException(e);
                *outAwaiterIsNull = fade_bool.False;
            }
        }
    }
}
