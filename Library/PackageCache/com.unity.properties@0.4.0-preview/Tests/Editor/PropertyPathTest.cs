#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Unity.Properties.Tests
{
    [TestFixture]
    internal class PropertyPathTest
    {
        [Test]
        public void Parse_Valid_Path()
        {
            var path = PropertyPath.Parse("Property.Path[123].Works");
            Debug.Log(path.ToString());
            
            Assert.AreEqual(3, path.PartsCount);
            Assert.AreEqual("Property", path[0].propertyName);
            Assert.AreEqual("Path", path[1].propertyName);
            Assert.AreEqual(123, path[1].listIndex);
            Assert.AreEqual("Works", path[2].propertyName);
            Assert.AreEqual(PropertyPath.InvalidListIndex, path[2].listIndex);
            
            Assert.AreEqual(string.Empty, PropertyPath.Parse("").ToString());
            Assert.AreEqual("Success", PropertyPath.Parse("Success").ToString());
            
            Assert.AreEqual(0, new PropertyPath().PartsCount);
        }
        
        [Test]
        public void Parse_Invalid_Path()
        {
            PropertyPath path;
            Assert.AreEqual(false, PropertyPath.TryParse(".", out path));        // empty property name
            Assert.AreEqual(false, PropertyPath.TryParse("a[b]", out path));     // non-integer list index
            Assert.AreEqual(false, PropertyPath.TryParse("a.]", out path));      // invalid token
            Assert.AreEqual(false, PropertyPath.TryParse("a.[", out path));      // invalid token
            Assert.AreEqual(false, PropertyPath.TryParse("a[.]", out path));     // invalid token
            Assert.AreEqual(false, PropertyPath.TryParse("a[[123]]", out path)); // invalid token
            Assert.AreEqual(false, PropertyPath.TryParse("[0]", out path));      // missing property name
        }
        
        [Test]
        public void Push_Pop()
        {
            var path = new PropertyPath();
            path.Push("hello");
            path.Push("world", 123);
            
            Assert.AreEqual(2, path.PartsCount);
            Assert.IsTrue(path[1].IsListItem);
            
            path.Pop();
            path.Pop();
            
            Assert.AreEqual(0, path.PartsCount);
        }

        [Test]
        public void Resolve()
        {
            var path = PropertyPath.Parse(nameof(TestContainer.IntValue));
            var container = new TestContainer
            {
                IntValue = 123
            };
            
            container.FloatList.Add(1);
            container.FloatList.Add(2);
            container.FloatList.Add(3);
            
            var resolution = path.Resolve(container);

            Assert.IsTrue(resolution.success);
            Assert.AreEqual(container, resolution.container);
            Assert.AreEqual(TestContainer.IntValueProperty, resolution.property);
            Assert.AreEqual(PropertyPath.InvalidListIndex, resolution.listIndex);
            Assert.AreEqual(123, resolution.value);
        }

        [Test]
        public void Resolve_List()
        {
            var path = PropertyPath.Parse("FloatList[1]");
            var container = new TestContainer
            {
                IntValue = 123
            };
            
            container.FloatList.Add(1);
            container.FloatList.Add(2);
            container.FloatList.Add(3);
            
            var resolution = path.Resolve(container);
            
            Assert.IsTrue(resolution.success);
            Assert.AreEqual(container, resolution.container);
            Assert.AreEqual(TestContainer.FloatListProperty, resolution.property);
            Assert.AreEqual(1, resolution.listIndex);
            Assert.AreEqual(2f, resolution.value);
        }

        [Test]
        public void Resolve_Container_List_Item()
        {
            var path = PropertyPath.Parse("ChildContainer.ChildList[1].IntValue");
            var container = new TestNestedContainer
            {
                ChildContainer = new TestContainer
                {
                    IntValue = 123,
                    ChildList =
                    {
                        new TestChildContainer {IntValue = 123},
                        new TestChildContainer {IntValue = 456}
                    }
                }
            };

            var resolution = path.Resolve(container);

            Assert.IsTrue(resolution.success);
            Assert.AreEqual(container.ChildContainer.ChildList[1], resolution.container);
            Assert.AreEqual(TestChildContainer.IntValueProperty, resolution.property);
            Assert.AreEqual(PropertyPath.InvalidListIndex, resolution.listIndex);
            Assert.AreEqual(456, resolution.value);
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)

