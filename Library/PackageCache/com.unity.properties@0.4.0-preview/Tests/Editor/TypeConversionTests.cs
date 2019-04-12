#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using NUnit.Framework;

namespace Unity.Properties.Tests
{
    [TestFixture]
    internal class TypeConversionTests
    {
        private interface ITestInterface
        {
            
        }
        
        private class TestClass : ITestInterface
        {
        }
        
        private enum TestEnum
        {
            First,
            Second,
            Third,
            Fourth
        }
        
        [Test]
        public static void BuiltIn_Convert_Int_To_String()
        {
            Assert.AreEqual("10", TypeConversion.Convert<string>(10));
        }
        
        [Test]
        public static void BuiltIn_Convert_String_To_Int()
        {
            Assert.AreEqual(10, TypeConversion.Convert<int>("10"));
        }

        [Test]
        public static void BuiltIn_Convert_String_To_Enum()
        {
            Assert.AreEqual(TestEnum.First, TypeConversion.Convert<TestEnum>("First"));
        }
        
        [Test]
        public static void BuiltIn_Convert_Enum_To_String()
        {
            Assert.AreEqual("Second", TypeConversion.Convert<string>(TestEnum.Second));
        }
        
        [Test]
        public static void BuiltIn_Convert_Int_To_Enum()
        {
            Assert.AreEqual(TestEnum.Third, TypeConversion.Convert<TestEnum>(2));
        }
        
        [Test]
        public static void BuiltIn_Convert_Enum_To_Int()
        {
            Assert.AreEqual(3, TypeConversion.Convert<int>(TestEnum.Fourth));
        }
        
        [Test]
        public static void Custom_Convert_Class_To_String()
        {
            TypeConversion.Register<TestClass, string>(t => "Test");
            try
            {
                Assert.AreEqual("Test", TypeConversion.Convert<string>(new TestClass()));
            }
            finally
            {
                TypeConversion.Unregister<TestClass, string>();
            }
        }
        
        [Test]
        public static void Custom_Convert_String_To_Class()
        {
            TypeConversion.Register<string, TestClass>(v => new TestClass(/* v */));
            try
            {
                Assert.IsNotNull(TypeConversion.Convert<TestClass>("Test"));
            }
            finally
            {
                TypeConversion.Unregister<string, TestClass>();
            }
        }
        
        [Test]
        public static void Custom_Convert_Interface_To_String()
        {
            TypeConversion.Register<ITestInterface, string>(v => "Test");
            try
            {
                Assert.AreEqual("Test", TypeConversion.Convert<string>(new TestClass()));
            }
            finally
            {
                TypeConversion.Unregister<ITestInterface, string>();
            }
        }
        
        [Test]
        public static void Custom_Convert_Interface_To_String_NoImpl()
        {
            Assert.Throws<Exception>(() =>
            {
                TypeConversion.Convert<string>(new TestClass());
            });
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)