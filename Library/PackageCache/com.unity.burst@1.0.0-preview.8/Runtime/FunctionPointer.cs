#if !UNITY_ZEROPLAYER && !UNITY_CSHARP_TINY
using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Burst
{
#if UNITY_BURST_FEATURE_FUNCPTR
    public interface IFunctionPointer
#else
    internal interface IFunctionPointer
#endif
    {
        IFunctionPointer FromIntPtr(IntPtr ptr);
    }

#if UNITY_BURST_FEATURE_FUNCPTR
    public struct FunctionPointer<T> : IFunctionPointer
#else
    internal struct FunctionPointer<T> : IFunctionPointer
#endif
    {
        [NativeDisableUnsafePtrRestriction]
        private readonly IntPtr _ptr;

        public FunctionPointer(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public IFunctionPointer FromIntPtr(IntPtr ptr)
        {
            return new FunctionPointer<T>(ptr);
        }

        public T Invoke => (T) (object) Marshal.GetDelegateForFunctionPointer(_ptr, typeof(T));
    }
}
#endif