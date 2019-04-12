#if (NET_4_6 || NET_STANDARD_2_0)

namespace Unity.Properties
{
    public static partial class PropertyContainer
    {
        public static void Visit<TContainer>(this TContainer container, IPropertyVisitor visitor)
            where TContainer : class, IPropertyContainer
        {
            var properties = container.PropertyBag;

            var typed = properties as IVisitableClass<TContainer>;

            if (typed != null)
            {
                typed.Visit(container, visitor);
            }
            else
            {
                (properties as IVisitableClass).Visit(container, visitor);
            }
        }

        public static void Visit<TContainer>(ref TContainer container, IPropertyVisitor visitor)
            where TContainer : struct, IPropertyContainer
        {
            var properties = container.PropertyBag;
            
            var typed = properties as IVisitableStruct<TContainer>;

            if (typed != null)
            {
                typed.Visit(ref container, visitor);
            }
            else
            {
                (properties as IVisitableStruct).Visit(ref container, visitor);
            }
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)