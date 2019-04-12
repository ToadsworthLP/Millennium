#if (NET_4_6 || NET_STANDARD_2_0)

namespace Unity.Properties
{
    public interface IVersionStorage
    {
        void IncrementVersion<TContainer>(IProperty property, TContainer container)
            where TContainer : IPropertyContainer;
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)