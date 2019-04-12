#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections.Generic;

namespace Unity.Properties.Tests
{
    internal class TestContainer : IPropertyContainer
    {
        public const int LeafCount = 2;

        private int _intValue;
        private TestEnum _enumValue;

        public static readonly ValueClassProperty<TestContainer, int> IntValueProperty = new ValueClassProperty<TestContainer, int>(
            nameof(IntValue),
            c => c._intValue,
            (c, v) => c._intValue = v);

        public int IntValue
        {
            get { return IntValueProperty.GetValue(this); }
            set { IntValueProperty.SetValue(this, value); }
        }

        public static readonly ValueClassProperty<TestContainer, TestEnum> EnumValueProperty = new ValueClassProperty<TestContainer, TestEnum>(
            nameof(EnumValue),
            c => c._enumValue,
            (c, v) => c._enumValue = v);

        public TestEnum EnumValue
        {
            get { return EnumValueProperty.GetValue(this); }
            set { EnumValueProperty.SetValue(this, value); }
        }

        private List<float> _floatList = new List<float>();

        public static readonly ValueListClassProperty<TestContainer, float> FloatListProperty =
            new ValueListClassProperty<TestContainer, float>(nameof(FloatList),
                c => c._floatList);

        public PropertyList<TestContainer, float> FloatList => new PropertyList<TestContainer, float>(FloatListProperty, this);

        private List<TestChildContainer> _childList = new List<TestChildContainer>();

        public static readonly ClassListClassProperty<TestContainer, TestChildContainer>
            ChildListProperty =
                new ClassListClassProperty<TestContainer, TestChildContainer>(
                    nameof(ChildList),
                    c => c._childList);

        public List<TestChildContainer> ChildList
        {
            get { return _childList; }
            set { _childList = value; }
        }

        public IVersionStorage VersionStorage => null;

        private static PropertyBag sBag = new PropertyBag(IntValueProperty, EnumValueProperty, FloatListProperty, ChildListProperty);
        public IPropertyBag PropertyBag => sBag;
    }

    public class TestChildContainer : IPropertyContainer
    {
        private int _intValue;

        public static readonly ValueClassProperty<TestChildContainer, int> IntValueProperty =
            new ValueClassProperty<TestChildContainer, int>(nameof(IntValue),
                c => c._intValue,
                (c, v) => c._intValue = v);

        public int IntValue
        {
            get { return IntValueProperty.GetValue(this); }
            set { IntValueProperty.SetValue(this, value); }
        }

        public IVersionStorage VersionStorage => null;

        private static PropertyBag sBag = new PropertyBag(IntValueProperty);
        public IPropertyBag PropertyBag => sBag;
    }

    internal class TestNestedContainer : IPropertyContainer
    {
        private TestContainer m_TestContainer;

        public static readonly ClassValueClassProperty<TestNestedContainer, TestContainer> ChildContainerProperty =
            new ClassValueClassProperty<TestNestedContainer, TestContainer>(
                nameof(ChildContainer),
                c => c.m_TestContainer,
                (c, v) => c.m_TestContainer = v);

        public TestContainer ChildContainer
        {
            get { return ChildContainerProperty.GetValue(this); }
            set { ChildContainerProperty.SetValue(this, value); }
        }

        public IVersionStorage VersionStorage => null;

        public IPropertyBag PropertyBag => sBag;
        private static PropertyBag sBag = new PropertyBag(ChildContainerProperty);
    }

    public enum TestEnum
    {
        This,
        Is,
        A,
        Test
    }
}
#endif // NET_4_6
