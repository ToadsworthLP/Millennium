using System;
using System.Runtime.CompilerServices;
using Unity.Jobs.LowLevel.Unsafe;

// Make internals visible to Unity.Burst.Editor for BurstGlobalCompilerOptions
[assembly: InternalsVisibleTo("Unity.Burst.Editor")]
// Make internals visible to burst tests
[assembly: InternalsVisibleTo("btests")]
// Make internals visible to Unity.Physics
[assembly: InternalsVisibleTo("Unity.Physics")]
[assembly: InternalsVisibleTo("Unity.Physics.Tests")]
[assembly: InternalsVisibleTo("Havok.Physics")]
[assembly: InternalsVisibleTo("Havok.Physics.Tests")]
[assembly: InternalsVisibleTo("Unity.Audio.DSPGraph")]
[assembly: InternalsVisibleTo("Unity.UNode")]
[assembly: InternalsVisibleTo("Unity.UNode.Tests")]

[assembly: InternalsVisibleTo("Unity.Burst.Tests.EditMode")]

namespace Unity.Burst
{
    // FloatMode and FloatPrecision must be kept in sync with burst.h / Burst.Backend

    public enum FloatMode
    {
        Default = 0,
        Strict = 1,
        Deterministic = 2,
        Fast = 3,
    }

    public enum FloatPrecision
    {
        Standard = 0,
        High = 1,
        Medium = 2,
        Low = 3,
    }

    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct|AttributeTargets.Method)]
    public class BurstCompileAttribute : System.Attribute
    {
        public FloatMode FloatMode { get; set; }

        public FloatPrecision FloatPrecision { get; set; }

        public bool CompileSynchronously { get; set; }

        public string[] Options { get; set; }

        public BurstCompileAttribute()
        {
        }

        public BurstCompileAttribute(FloatPrecision floatPrecision, FloatMode floatMode)
        {
            FloatMode = floatMode;
            FloatPrecision = floatPrecision;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class NoAliasAttribute : System.Attribute
    {
    }
}