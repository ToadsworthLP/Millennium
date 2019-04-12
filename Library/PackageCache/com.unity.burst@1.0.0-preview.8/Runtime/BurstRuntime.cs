using System;

#if BURST_INTERNAL
namespace Burst.Compiler.IL
#else
namespace Unity.Burst
#endif
{
    /// <summary>
    /// Provides helper intrinsics that can be used at runtime.
    /// </summary>
    internal static class BurstRuntime
    {
        /// <summary>
        /// Gets a 32-bits hashcode from a type computed for the <see cref="System.Type.FullName"/>
        /// </summary>
        /// <typeparam name="T">The type to compute the hash from</typeparam>
        public static int GetHashCode32<T>()
        {
            return HashCode32<T>.Value;
        }

        /// <summary>
        /// Gets a 64-bits hashcode from a type computed for the <see cref="System.Type.FullName"/>
        /// </summary>
        /// <typeparam name="T">The type to compute the hash from</typeparam>
        public static long GetHashCode64<T>()
        {
            return HashCode64<T>.Value;
        }

        // method internal as it is used by the compiler directly
        internal static int HashStringWithFNV1A32(string text)
        {
            // Using http://www.isthe.com/chongo/tech/comp/fnv/index.html#FNV-1a
            // with basis and prime:
            const uint offsetBasis = 2166136261;
            const uint prime = 16777619;

            uint result = offsetBasis;
            foreach (var c in text)
            {
                result = prime * (result ^ (byte)(c & 255));
                result = prime * (result ^ (byte)(c >> 8));
            }
            return (int)result;
        }

        // method internal as it is used by the compiler directly
        internal static long HashStringWithFNV1A64(string text)
        {
            // Using http://www.isthe.com/chongo/tech/comp/fnv/index.html#FNV-1a
            // with basis and prime:
            const ulong offsetBasis = 14695981039346656037;
            const ulong prime = 1099511628211;

            ulong result = offsetBasis;
            foreach (var c in text)
            {
                result = prime * (result ^ (byte)(c & 255));
                result = prime * (result ^ (byte)(c >> 8));
            }
            return (long)result;
        }

        private struct HashCode32<T>
        {
            public static readonly int Value = HashStringWithFNV1A32(typeof(T).AssemblyQualifiedName);
        }

        private struct HashCode64<T>
        {
            public static readonly long Value = HashStringWithFNV1A64(typeof(T).AssemblyQualifiedName);
        }
    }
}