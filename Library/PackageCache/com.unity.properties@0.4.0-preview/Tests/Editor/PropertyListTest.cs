#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections;
using NUnit.Framework;
using Unity.Properties.Tests;
using System.Collections.Generic;
using Unity.Properties;
using System.Linq;

namespace Packages.com.unity.properties.Tests.Editor
{
    [TestFixture]
    internal class PropertyListTest
    {
        [Test]
        public void Enumerable_Count()
        {
            var container = new TestContainer();

            PropertyList<TestContainer, float> propertyList = container.FloatList;
            
            propertyList.Add(1);
            propertyList.Add(2);
            propertyList.Add(3);

            Assert.AreEqual(3, propertyList.Count());
        }
        
        [Test]
        public void Enumerator_InitializesCurrentToDefault()
        {
            var container = new TestContainer();

            PropertyList<TestContainer, float> propertyList = container.FloatList;
            
            propertyList.Add(1);
            propertyList.Add(2);
            propertyList.Add(3);

            IEnumerator e = propertyList.GetEnumerator();
            
            Assert.AreEqual(default(float), e.Current);
        }
        
        [Test]
        public void AddRange_ToNonEmptyList()
        {
            var list = new List<float> {1, 2, 3};
            
            var container = new TestContainer();

            PropertyList<TestContainer, float> propertyList = container.FloatList;
            
            propertyList.Add(4);
            propertyList.Add(5);
            propertyList.Add(6);
            
            list.AddRange(propertyList);

            Assert.AreEqual(6, list.Count);
            
            // The elements should have been added correctly to the end of the list
            Assert.AreEqual(4, list[3]);
            Assert.AreEqual(5, list[4]);
            Assert.AreEqual(6, list[5]);
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)