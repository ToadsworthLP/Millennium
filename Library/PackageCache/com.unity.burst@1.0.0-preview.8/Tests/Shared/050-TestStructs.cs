using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace Burst.Compiler.IL.Tests
{
    internal partial class TestStructs
    {
        [TestCompiler]
        public static float test_struct_func_call_by_value()
        {
            var localVar = new CustomStruct();
            localVar.firstfield = 94;
            localVar.value = 123;
            return byvalue_function_helper(localVar);
        }

        [TestCompiler]
        public static float test_struct_func_call_by_ref()
        {
            var localVar = new CustomStruct
            {
                firstfield = 94,
                value = 123
            };
            byref_function_helper(ref localVar);
            return localVar.value;
        }

        [TestCompiler]
        public static float test_struct_func_call_instance()
        {
            var localVar = new CustomStruct2 { value = 123 };
            return localVar.returnDoubleValue();
        }

        [TestCompiler]
        public static float test_struct_constructor_nondefault()
        {
            var localVar = new CustomStruct2(123.0f);
            return localVar.value;
        }

        [TestCompiler]
        public static float test_struct_constructor_default()
        {
            var localVar = new CustomStruct2();
            localVar.value = 1;
            return localVar.value;
        }

        [TestCompiler]
        public static float test_struct_copysemantic()
        {
            var a = new CustomStruct2 { value = 123.0f };
            var b = a;
            b.value = 345;
            return b.value;
        }

        [TestCompiler]
        public static float test_struct_nested()
        {
            var a = new TestNestedStruct { v1 = { x = 5 } };
            return a.v1.x;
        }

        [TestCompiler(1.0f)]
        public static float test_struct_multiple_fields(float x)
        {
            var v = new TestVector4
            {
                x = 1.0f,
                y = 2.0f,
                z = 3.0f,
                w = 4.0f
            };
            return x + v.x + v.y + v.z + v.w;
        }

        [TestCompiler]
        public static float test_struct_multi_assign()
        {
            var a = new MultiAssignStruct(2.0F);
            return a.x + a.y + a.z;
        }

        [TestCompiler]
        public static int test_custom_struct_return_simple()
        {
            var a = return_value_helper_simple(1, 2);
            return a.firstfield + a.value;
        }

        [TestCompiler]
        public static int test_custom_struct_return_constructor()
        {
            var a = return_value_helper_constructor(1, 2);
            return a.firstfield + a.value;
        }

        [TestCompiler]
        public static int test_struct_self_reference()
        {
            var a = new SelfReferenceStruct
            {
                Value = 1
            };
            return a.Value;
        }

        [TestCompiler]
        public static int test_struct_deep()
        {
            var deep = new DeepStruct2();
            deep.value.value.SetValue(10);
            return deep.value.value.GetValue() + deep.value.value.value;
        }

        // No longer working as EmptyStruct is generating a struct with a fixed size of 1
        [TestCompiler(2)]
        public static int test_struct_empty(int x)
        {
            var emptyStruct = new EmptyStruct();
            var result = emptyStruct.Increment(x);
            return result;
        }

        //[TestCompiler(typeof(StructInvalidProvider), ExpectedException = typeof(CompilerException))]
        //public static void test_struct_unsupported_class_in_struct(StructInvalid value)
        //{
        //}

        [TestCompiler]
        public static float test_struct_with_static_fields()
        {
            StructWithStaticVariables myStruct = new StructWithStaticVariables();
            myStruct.myFloat = 5;
            myStruct = copy_struct_with_static_by_value(myStruct);
            mutate_struct_with_static_by_ref_value(ref myStruct);
            return myStruct.myFloat;
        }

        [TestCompiler(true)]
        [TestCompiler(false)]
        public static bool TestStructWithBoolAsInt(bool value)
        {
            var structWithBoolAsInt = new StructWithBoolAsInt(value);
            return structWithBoolAsInt;
        }

        [TestCompiler]
        [Ignore("IL Instruction LEAVE not yet supported")]
        public static int TestStructDisposable()
        {
            using (var structDisposable = new StructDisposable())
            {
                return structDisposable.x + 1;
            }
        }

        [TestCompiler(ExpectCompilerException = true)]
        public static void TestStructWithStaticFieldWrite()
        {
            var test = new StructWithStaticField();
            test.CheckWrite();
        }

        [TestCompiler(ExpectCompilerException = true)]
        public static void TestStructWithStaticFieldRead()
        {
            var test = new StructWithStaticField();
            test.CheckRead();
        }

        [TestCompiler]
        public static int TestExplicitLayoutStruct()
        {
            var color = new Color() { Value = 0xAABBCCDD };
            var a = color.Value + GetColorR(ref color) + GetColorG(color) + color.GetColorB() + color.A;
            var pair = new NumberPair()
            {
                SignedA = -13,
                UnsignedB = 37
            };
            var b = pair.SignedA - ((int) pair.UnsignedA) + pair.SignedB - ((int) pair.UnsignedB);
            return ((int)a) + b;
        }

        static uint GetColorR(ref Color color)
        {
            return color.R;
        }

        static uint GetColorG(Color color)
        {
            return color.G;
        }

        [TestCompiler()]
        public static uint TestExplicitLayoutWrite()
        {
            var color = new Color() { Value = 0xAABBCCDD };
            color.G = 3;
            ColorWriteBByRef(ref color, 7);
            return color.Value;
        }

        static void ColorWriteBByRef(ref Color color, byte v)
        {
            color.B = v;
        }

        [TestCompiler]
        public static int TestNestedExplicitLayouts()
        {
            var nested = new NestedExplicit0()
            {
                Next = new NestedExplicit1()
                {
                    Next = new NestedExplicit2()
                    {
                        FValue = 13.37f
                    }
                }
            };
            var a = nested.NextAsInt + nested.Next.NextAsInt + nested.Next.Next.IValue;
            nested.Next.Next.FValue = 0.0042f;
            var b = nested.NextAsInt + nested.Next.NextAsInt + nested.Next.Next.IValue;
            return a + b;
        }

        [TestCompiler]
        public static uint TestBitcast()
        {
            return new FloatRepr()
            {
                Value = 13.37f
            }.AsUint;
        }
        
        [TestCompiler]
        public static uint TestExplicitStructFromCall()
        {
            return ReturnStruct().Value + ReturnStruct().R;
        }
        
        static Color ReturnStruct()
        {
            return new Color()
            {
                R = 10,
                G = 20,
                B = 30,
                A = 255
            };
        }
        
        [TestCompiler]
        public static unsafe uint TestExplicitLayoutStructWithFixedArray()
        {
            var x = new FixedArrayExplitLayoutStruct()
            {
                UpperUInt = 0xAABBCCDD,
                LowerUInt = 0xEEFF3344
            };

            uint sum = 0;
            for (int i = 0; i < 8; i++)
            {
                sum += x.Bytes[i];
                if (i < 4) sum += x.Shorts[i];
            }
            
            return x.UpperUInt + x.LowerUInt + sum;
        }

        public struct StructInvalid
        {
            public string WowThatStringIsNotSupported;
        }

        //private struct StructInvalidProvider : IArgumentProvider
        //{
        //    public object[] Arguments => new object[] { new StructInvalid() };
        //}

        private static CustomStruct return_value_helper_simple(int a, int b)
        {
            CustomStruct val;
            val.firstfield = a;
            val.value = b;
            return val;
        }

        private static CustomStruct return_value_helper_constructor(int a, int b)
        {
            return new CustomStruct(a, b);
        }

        private static float byvalue_function_helper(CustomStruct customStruct)
        {
            return customStruct.value * 2;
        }

        private static void byref_function_helper(ref CustomStruct customStruct)
        {
            customStruct.value = customStruct.value * 2;
        }

        static StructWithStaticVariables copy_struct_with_static_by_value(StructWithStaticVariables byValue)
        {
            byValue.myFloat += 2;
            return byValue;
        }

        static void mutate_struct_with_static_by_ref_value(ref StructWithStaticVariables byValue)
        {
            byValue.myFloat += 2;
        }


        private struct EmptyStruct
        {
            public int Increment(int x)
            {
                return x + 1;
            }
        }

        private struct CustomStruct
        {
            public int firstfield;
            public int value;

            public CustomStruct(int a, int b)
            {
                firstfield = a;
                value = b;
            }
        }

        struct DeepStruct2
        {
#pragma warning disable 0649
            public DeepStruct1 value;
#pragma warning restore 0649
        }

        struct DeepStruct1
        {
#pragma warning disable 0649
            public DeepStruct0 value;
#pragma warning restore 0649
        }

        struct DeepStruct0
        {
            public int value;

            public void SetValue(int value)
            {
                this.value = value;
            }

            public int GetValue()
            {
                return value;
            }
        }

        private struct CustomStruct2
        {
            public float value;

            public float returnDoubleValue()
            {
                return value;
            }

            public CustomStruct2(float initialValue)
            {
                value = initialValue;
            }
        }

        private struct TestVector4
        {
            public float x;
            public float y;
            public float z;
            public float w;
        }

        private struct StructWithBoolAsInt
        {
            private int _value;

            public StructWithBoolAsInt(bool value)
            {
                _value = value ? 1 : 0;
            }

            public static implicit operator bool(StructWithBoolAsInt val)
            {
                return val._value != 0;
            }
        }

        private struct TestNestedStruct
        {
            public TestVector4 v1;
        }

        private struct MultiAssignStruct
        {
            public float x;
            public float y;
            public float z;

            public MultiAssignStruct(float val)
            {
                x = y = z = val;
            }
        }

        private struct SelfReferenceStruct
        {
#pragma warning disable 0649
            public int Value;
            public unsafe SelfReferenceStruct* Left;
            public unsafe SelfReferenceStruct* Right;
#pragma warning restore 0649
        }

        private struct StructForSizeOf
        {
#pragma warning disable 0649

            public IntPtr Value1;

            public Float4 Vec1;

            public IntPtr Value2;

            public Float4 Vec2;
#pragma warning disable 0649
        }

        private struct StructWithStaticField
        {
            public static int MyField;

            public void CheckWrite()
            {
                MyField = 0;
            }

            public int CheckRead()
            {
                return MyField;
            }
        }

        private struct Float4
        {
#pragma warning disable 0649
            public float x;
            public float y;
            public float z;
            public float w;
#pragma warning restore 0649
        }

        private struct StructWithStaticVariables
        {
#pragma warning disable 0414
#pragma warning disable 0649
            const float static_const_float = 9;
            static string static_string = "hello";

            public float myFloat;
            public Float4 myFloat4;

            static float static_float_2 = 5;
#pragma warning restore 0649
#pragma warning restore 0414
        }

        struct StructDisposable : IDisposable
        {
            public int x;


            public void Dispose()
            {
                x++;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct Color
        {
            [FieldOffset(0)] public uint Value;
            [FieldOffset(0)] public byte R;
            [FieldOffset(1)] public byte G;
            [FieldOffset(2)] public byte B;
            [FieldOffset(3)] public byte A;

            public byte GetColorB()
            {
                return B;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NumberPair
        {
            [FieldOffset(0)] public uint UnsignedA;
            [FieldOffset(0)] public int SignedA;
            [FieldOffset(4)] public uint UnsignedB;
            [FieldOffset(4)] public int SignedB;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NestedExplicit0
        {
            [FieldOffset(0)] public NestedExplicit1 Next;
            [FieldOffset(0)] public int NextAsInt;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NestedExplicit1
        {
            [FieldOffset(0)] public NestedExplicit2 Next;
            [FieldOffset(0)] public int NextAsInt;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NestedExplicit2
        {
            [FieldOffset(0)] public float FValue;
            [FieldOffset(0)] public int IValue;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatRepr
        {
            [FieldOffset(0)] public float Value;
            [FieldOffset(0)] public uint AsUint;
        }
        
        [StructLayout(LayoutKind.Explicit, Size =  24)]
        private struct PaddedStruct
        {
            [FieldOffset(8)] public int Value;
        }

        [StructLayout(LayoutKind.Explicit)]
        private unsafe struct FixedArrayExplitLayoutStruct
        {
            [FieldOffset(0)] public fixed byte Bytes[8];
            [FieldOffset(0)] public fixed ushort Shorts[4];
            [FieldOffset(0)] public uint UpperUInt;
            [FieldOffset(4)] public uint LowerUInt;
        }

        [StructLayout(LayoutKind.Explicit)]
        public unsafe struct Chunk
        {
            [FieldOffset(0)] public Chunk* Archetype;
            [FieldOffset(8)] public Chunk* metaChunkEntity;

            [FieldOffset(16)] public int Count;
        }

        [TestCompiler]
        public static unsafe int TestRegressionInvalidGetElementPtrStructLayout()
        {
            Chunk* c = stackalloc Chunk[1];
            c[0].Archetype = null;
            c[0].metaChunkEntity = null;
            c[0].Count = 0;
            
            return TestRegressionInvalidGetElementPtrStructLayoutInternal(0, 1, &c);
        }
        
        public static unsafe int TestRegressionInvalidGetElementPtrStructLayoutInternal(int index, int limit, Chunk** currentChunk)
        {
            int rValue = 0;
            while (index >= limit + 1)
            {
                rValue += (*currentChunk)->Count;
                index += 1;
            }

            return rValue;
        }
    }
}

