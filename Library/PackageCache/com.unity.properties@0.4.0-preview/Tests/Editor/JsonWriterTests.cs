#if (NET_4_6 || NET_STANDARD_2_0)

using UnityEngine;
using NUnit.Framework;
using Unity.Properties.Serialization;

namespace Unity.Properties.Tests.Serialization
{
    [TestFixture]
    internal class JsonWriterTests
    {
        [Test]
        public void Enum_Serialization()
        {
            var obj = new TestContainer() {EnumValue = TestEnum.Test};
            var result = JsonSerializer.Serialize(obj);
            Assert.IsTrue(result.Contains($"\"EnumValue\": {(int)TestEnum.Test}"));
        }
        
        [Test]
        public void WhenEmptyPropertyContainer_JsonSerialization_ReturnsAnEmptyResult()
        {
            var obj = new NullStructContainer();
            var result = JsonSerializer.Serialize(ref obj);
            Assert.NotZero(result.Length);
        }

        [Test]
        public void WhenStructPropertyContainer_JsonSerialization_ReturnsAValidResult()
        {
            var obj = new TestStructContainer();
            var result = JsonSerializer.Serialize(ref obj);
            Debug.Log(result);
            Assert.IsTrue(result.Contains("FloatValue"));
        }

        private struct NullStructContainer : IPropertyContainer
        {
            public IVersionStorage VersionStorage => null;
            private static readonly PropertyBag s_PropertyBag = new PropertyBag();
            public IPropertyBag PropertyBag => s_PropertyBag;
        }

        private struct TestStructContainer : IPropertyContainer
        {
            private float m_FloatValue;

            public float FloatValue
            {
                get { return FloatValueProperty.GetValue(ref this); }
                set { FloatValueProperty.SetValue(ref this, value); }
            }

            public static readonly ValueStructProperty<TestStructContainer, float> FloatValueProperty =
                new ValueStructProperty<TestStructContainer, float>(
                    nameof(FloatValue),
                    (ref TestStructContainer c) => c.m_FloatValue,
                    (ref TestStructContainer c, float v) => c.m_FloatValue = v);

            public IVersionStorage VersionStorage => null;

            private static readonly PropertyBag s_PropertyBag = new PropertyBag(FloatValueProperty);

            public IPropertyBag PropertyBag => s_PropertyBag;
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)