using Burst.Compiler.IL.Tests.Helpers;
using Unity.Mathematics;

namespace Burst.Compiler.IL.Tests
{
    internal partial class Vectors
    {
        public class Bools
        {
            // ---------------------------------------------------
            // ! operator
            // ---------------------------------------------------

            [TestCompiler(DataRange.Standard)]
            public static int Bool4Not(ref bool4 a)
            {
                return ConvertToInt(!a);
            }

            [TestCompiler(DataRange.Standard)]
            public static int Bool3Not(ref bool3 a)
            {
                return ConvertToInt(!a);
            }

            [TestCompiler(DataRange.Standard)]
            public static int Bool2Not(ref bool2 a)
            {
                return ConvertToInt(!a);
            }
        }
    }
}