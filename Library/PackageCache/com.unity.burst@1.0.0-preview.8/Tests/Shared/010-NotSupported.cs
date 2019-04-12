using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace Burst.Compiler.IL.Tests
{
    /// <summary>
    /// Tests types
    /// </summary>
    internal class NotSupported
    {
        [TestCompiler(1, ExpectCompilerException = true)]
        public static int TestDelegate(int data)
        {
            return ProcessData(i => i + 1, data);
        }

        [TestCompiler(1, ExpectCompilerException = true)]
        public static bool TestIsOfType(object data)
        {
            var check = data as NotSupported;
            return (check != null);
        }

        private static int ProcessData(Func<int, int> yo, int value)
        {
            return yo(value);
        }
    }
}