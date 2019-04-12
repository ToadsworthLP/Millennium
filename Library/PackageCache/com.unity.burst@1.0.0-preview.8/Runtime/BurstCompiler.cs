// For some reasons Unity.Burst.LowLevel is not part of UnityEngine in 2018.2 but only in UnityEditor
// In 2018.3 It should be fine
#if !UNITY_ZEROPLAYER && !UNITY_CSHARP_TINY && ((UNITY_2018_2_OR_NEWER && UNITY_EDITOR) || UNITY_2018_3_OR_NEWER)
using System;
using System.Runtime.InteropServices;

namespace Unity.Burst
{
    /// <summary>
    /// The burst compiler runtime frontend.
    /// </summary>
#if UNITY_BURST_FEATURE_FUNCPTR
    public static class BurstCompiler
#else
    internal static class BurstCompiler
#endif
    {
        /// <summary>
        /// Compile the following delegate with burst and return a new delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delegateMethod"></param>
        /// <returns></returns>
        public static unsafe T CompileDelegate<T>(T delegateMethod) where T : class
        {
            // We have added support for runtime CompileDelegate in 2018.2+
            void* function = Compile(delegateMethod);
            object res = System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer((IntPtr)function, delegateMethod.GetType());
            return (T)res;
        }

        /// <summary>
        /// Compile the following delegate into a function pointer with burst.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delegateMethod"></param>
        /// <returns></returns>
        public static unsafe FunctionPointer<T> CompileFunctionPointer<T>(T delegateMethod) where T : class
        {
            // We have added support for runtime CompileDelegate in 2018.2+
            void* function = Compile(delegateMethod);
            return new FunctionPointer<T>(new IntPtr(function));
        }

        private static unsafe void* Compile<T>(T delegateObj) where T : class
        {
            if (delegateObj == null) throw new ArgumentNullException(nameof(delegateObj));
            if (!(delegateObj is Delegate)) throw new ArgumentException("object instance must be a System.Delegate", nameof(delegateObj));

            var delegateMethod = (Delegate)(object)delegateObj;
            if (!delegateMethod.Method.IsStatic)
            {
                throw new InvalidOperationException($"The method `{delegateMethod.Method}` must be static. Instance methods are not supported");
            }

            string defaultOptions = "--enable-synchronous-compilation";
            // TODO: Disable this part as it is using Editor code that is not accessible from the runtime. We will have to move the editor code to here
            string extraOptions;

            void* function = null;

            // The attribute is directly on the method, so we recover the underlying method here
            if (BurstCompilerOptions.Global.TryGetOptions(delegateMethod.Method, false, out extraOptions))
            {
                if (!string.IsNullOrWhiteSpace(extraOptions))
                {
                    defaultOptions += "\n" + extraOptions;
                }
                int delegateMethodID = Unity.Burst.LowLevel.BurstCompilerService.CompileAsyncDelegateMethod(delegateObj, defaultOptions);
                function = Unity.Burst.LowLevel.BurstCompilerService.GetAsyncCompiledAsyncDelegateMethod(delegateMethodID);
                if (function == null)
                {
                    throw new InvalidOperationException("Burst failed to compile the given delegate");
                }
            }
            // In case burst compilation is disabled, we are still returning a valid function pointer (the a pointer to the managed function)
            // so that CompileFunctionPointer actually returns a delegate in all cases
            return function == null ? (void*)Marshal.GetFunctionPointerForDelegate(delegateMethod) : function;
        }
    }
}
#endif
