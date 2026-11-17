using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using FADE.Bridge;
using FADE.NativeInterop;

#nullable enable

namespace FADE
{
    public partial class FADEObject : IDisposable
    {
        private bool _disposed;
        private static readonly Type _cachedType = typeof(FADEObject);

        private static readonly Dictionary<Type, StringName?> _nativeNames = new Dictionary<Type, StringName?>();

        internal IntPtr NativePtr;
        private bool _memoryOwn;

        private WeakReference<FADEObject>? _weakReferenceToSelf;

        /// <summary>
        /// Constructs a new <see cref="FADEObject"/>.
        /// </summary>
        public FADEObject() : this(false)
        {
            unsafe
            {
                ConstructAndInitialize(NativeCtor, NativeName, _cachedType, refCounted: false);
            }
        }

        internal FADEObject(IntPtr nativePtr) : this(false)
        {
            // NativePtr must be non-zero before calling ConstructAndInitialize to avoid invoking the constructor NativeCtor.
            // We don't want to invoke the constructor, because we already have a constructed instance in nativePtr.
            NativePtr = nativePtr;
            unsafe
            {
                ConstructAndInitialize(NativeCtor, NativeName, _cachedType, refCounted: false);
            }
        }

        internal unsafe void ConstructAndInitialize(
            delegate* unmanaged<fade_bool, IntPtr> nativeCtor,
            StringName nativeName,
            Type cachedType,
            bool refCounted
        )
        {
            if (NativePtr == IntPtr.Zero)
            {
                Debug.Assert(nativeCtor != null);

                // Need postinitialization.
                NativePtr = nativeCtor(fade_bool.True);

                InteropUtils.TieManagedToUnmanaged(this, NativePtr,
                    nativeName, refCounted, GetType(), cachedType);
            }
            else
            {
                InteropUtils.TieManagedToUnmanagedWithPreSetup(this, NativePtr,
                    GetType(), cachedType);
            }

            _weakReferenceToSelf = DisposablesTracker.RegisterFADEObject(this);
        }

        internal FADEObject(bool memoryOwn)
        {
            _memoryOwn = memoryOwn;
        }

        /// <summary>
        /// The pointer to the native instance of this <see cref="FADEObject"/>.
        /// </summary>
        public IntPtr NativeInstance => NativePtr;

        internal static IntPtr GetPtr(FADEObject? instance)
        {
            if (instance == null)
                return IntPtr.Zero;

            // We check if NativePtr is null because this may be called by the debugger.
            // If the debugger puts a breakpoint in one of the base constructors, before
            // NativePtr is assigned, that would result in UB or crashes when calling
            // native functions that receive the pointer, which can happen because the
            // debugger calls ToString() and tries to get the value of properties.
            ObjectDisposedException.ThrowIf(instance._disposed || instance.NativePtr == IntPtr.Zero, instance);

            return instance.NativePtr;
        }

        ~FADEObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of this <see cref="FADEObject"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes implementation of this <see cref="FADEObject"/>.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            if (NativePtr != IntPtr.Zero)
            {
                IntPtr gcHandleToFree = NativeFuncs.fadesharp_internal_object_get_associated_gchandle(NativePtr);

                if (gcHandleToFree != IntPtr.Zero)
                {
                    object? target = GCHandle.FromIntPtr(gcHandleToFree).Target;
                    // The GC handle may have been replaced in another thread. Release it only if
                    // it's associated to this managed instance, or if the target is no longer alive.
                    if (target != this && target != null)
                        gcHandleToFree = IntPtr.Zero;
                }

                if (_memoryOwn)
                {
                    NativeFuncs.fadesharp_internal_refcounted_disposed(NativePtr, gcHandleToFree,
                        (!disposing).ToFADEBool());
                }
                else
                {
                    NativeFuncs.fadesharp_internal_object_disposed(NativePtr, gcHandleToFree);
                }

                NativePtr = IntPtr.Zero;
            }

            if (_weakReferenceToSelf != null)
            {
                DisposablesTracker.UnregisterFADEObject(this, _weakReferenceToSelf);
            }
        }

        /// <summary>
        /// Converts this <see cref="FADEObject"/> to a string.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            NativeFuncs.fadesharp_object_to_string(GetPtr(this), out fade_string str);
            using (str)
                return Marshaling.ConvertStringToManaged(str);
        }

        /// <summary>
        /// Returns a new <see cref="SignalAwaiter"/> awaiter configured to complete when the instance
        /// <paramref name="source"/> emits the signal specified by the <paramref name="signal"/> parameter.
        /// </summary>
        /// <param name="source">
        /// The instance the awaiter will be listening to.
        /// </param>
        /// <param name="signal">
        /// The signal the awaiter will be waiting for.
        /// </param>
        /// <example>
        /// This sample prints a message once every frame up to 100 times.
        /// <code>
        /// public override void _Ready()
        /// {
        ///     for (int i = 0; i &lt; 100; i++)
        ///     {
        ///         await ToSignal(GetTree(), "process_frame");
        ///         GD.Print($"Frame {i}");
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <returns>
        /// A <see cref="SignalAwaiter"/> that completes when
        /// <paramref name="source"/> emits the <paramref name="signal"/>.
        /// </returns>
        public SignalAwaiter ToSignal(FADEObject source, StringName signal)
        {
            return new SignalAwaiter(source, signal, this);
        }

        internal static bool IsNativeClass(Type t)
        {
            if (ReferenceEquals(t.Assembly, typeof(FADEObject).Assembly))
            {
                return true;
            }

            if (ReflectionUtils.IsEditorHintCached)
            {
                return t.Assembly.GetName().Name == "FADESharpEditor";
            }

            return false;
        }

        internal static Type InternalGetClassNativeBase(Type t)
        {
            while (!IsNativeClass(t))
            {
                Debug.Assert(t.BaseType is not null, "Script types must derive from a native FADE type.");

                t = t.BaseType;
            }

            return t;
        }

        internal static StringName? InternalGetClassNativeBaseName(Type t)
        {
            if (_nativeNames.TryGetValue(t, out var name))
            {
                return name;
            }

            var baseType = InternalGetClassNativeBase(t);

            if (_nativeNames.TryGetValue(baseType, out name))
            {
                return name;
            }

            var field = baseType.GetField("NativeName",
                BindingFlags.DeclaredOnly | BindingFlags.Static |
                BindingFlags.Public | BindingFlags.NonPublic);

            name = field?.GetValue(null) as StringName;

            _nativeNames[baseType] = name;

            return name;
        }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        /// <summary>
        /// Set the value of a property contained in this class.
        /// This method is used by FADE to assign property values.
        /// Do not call or override this method.
        /// </summary>
        /// <param name="name">Name of the property to set.</param>
        /// <param name="value">Value to set the property to if it was found.</param>
        /// <returns><see langword="true"/> if a property with the given name was found.</returns>
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected internal virtual bool SetFADEClassPropertyValue(in fade_string_name name, in fade_variant value)
        {
            return false;
        }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        /// <summary>
        /// Get the value of a property contained in this class.
        /// This method is used by FADE to retrieve property values.
        /// Do not call or override this method.
        /// </summary>
        /// <param name="name">Name of the property to get.</param>
        /// <param name="value">Value of the property if it was found.</param>
        /// <returns><see langword="true"/> if a property with the given name was found.</returns>
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected internal virtual bool GetFADEClassPropertyValue(in fade_string_name name, out fade_variant value)
        {
            value = default;
            return false;
        }

        // ReSharper disable once VirtualMemberNeverOverridden.Global
        /// <summary>
        /// Raises the signal with the given name, using the given arguments.
        /// This method is used by FADE to raise signals from the engine side.\n"
        /// Do not call or override this method.
        /// </summary>
        /// <param name="signal">Name of the signal to raise.</param>
        /// <param name="args">Arguments to use with the raised signal.</param>
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected internal virtual void RaiseFADEClassSignalCallbacks(in fade_string_name signal,
            NativeVariantPtrArgs args)
        {
        }

        internal static IntPtr ClassDB_get_method(StringName type, StringName method)
        {
            var typeSelf = (fade_string_name)type.NativeValue;
            var methodSelf = (fade_string_name)method.NativeValue;
            IntPtr methodBind = NativeFuncs.fadesharp_method_bind_get_method(typeSelf, methodSelf);

            if (methodBind == IntPtr.Zero)
                throw new NativeMethodBindNotFoundException(type + "." + method);

            return methodBind;
        }

        internal static IntPtr ClassDB_get_method_with_compatibility(StringName type, StringName method, ulong hash)
        {
            var typeSelf = (fade_string_name)type.NativeValue;
            var methodSelf = (fade_string_name)method.NativeValue;
            IntPtr methodBind = NativeFuncs.fadesharp_method_bind_get_method_with_compatibility(typeSelf, methodSelf, hash);

            if (methodBind == IntPtr.Zero)
                throw new NativeMethodBindNotFoundException(type + "." + method);

            return methodBind;
        }

        internal static unsafe delegate* unmanaged<fade_bool, IntPtr> ClassDB_get_constructor(StringName type)
        {
            // for some reason the '??' operator doesn't support 'delegate*'
            var typeSelf = (fade_string_name)type.NativeValue;
            var nativeConstructor = NativeFuncs.fadesharp_get_class_constructor(typeSelf);

            if (nativeConstructor == null)
                throw new NativeConstructorNotFoundException(type);

            return nativeConstructor;
        }

        /// <summary>
        /// Saves this instance's state to be restored when reloading assemblies.
        /// Do not call or override this method.
        /// To add data to be saved and restored, implement <see cref="ISerializationListener"/>.
        /// </summary>
        /// <param name="info">Object used to save the data.</param>
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected internal virtual void SaveFADEObjectData(FADESerializationInfo info)
        {
        }

        // TODO: Should this be a constructor overload?
        /// <summary>
        /// Restores this instance's state after reloading assemblies.
        /// Do not call or override this method.
        /// To add data to be saved and restored, implement <see cref="ISerializationListener"/>.
        /// </summary>
        /// <param name="info">Object that contains the previously saved data.</param>
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected internal virtual void RestoreFADEObjectData(FADESerializationInfo info)
        {
        }
    }
}
