#if (NET_4_6 || NET_STANDARD_2_0)

using UnityEngine;
using NUnit.Framework;

namespace Unity.Properties.Tests
{
	[TestFixture]
	internal class PropertyTest
	{
		[Test]
		public void Simple_Visitor()
		{
			// test PropertyBag
			var container = new TestContainer { IntValue = 123, EnumValue = TestEnum.Test};

			// test Set/GetValue
			Assert.AreEqual(123, TestContainer.IntValueProperty.GetValue(container));
			Assert.AreEqual(TestEnum.Test, TestContainer.EnumValueProperty.GetValue(container));

			// test visitor
			var leafVisitor = new LeafCountVisitor();
			container.Visit(leafVisitor);
			Assert.AreEqual(TestContainer.LeafCount, leafVisitor.LeafCount);
		}

		private class DerivedContainer : TestContainer
		{
		}

		[Test]
		public void Contravariant_Cast()
		{
			// this test is to make sure properties from base classes can be used in
			// derived classes to build their property bag.
			var p = TestContainer.IntValueProperty as IValueClassProperty<DerivedContainer, int>;
			Assert.NotNull(p);
		}

		private class LeafCountVisitor : PropertyVisitor
		{
			public int LeafCount { get; private set; }

			protected override void Visit<TValue>(TValue value)
			{
				++LeafCount;
			}
		}
	}
}

#endif // (NET_4_6 || NET_STANDARD_2_0)

