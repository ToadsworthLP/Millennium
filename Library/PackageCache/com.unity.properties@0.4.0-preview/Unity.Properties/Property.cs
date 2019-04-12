#if (NET_4_6 || NET_STANDARD_2_0)

using System;

namespace Unity.Properties
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for all properties
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    public abstract class Property<TContainer> : IProperty
    {
        public virtual string Name { get; }
        public Type ContainerType => typeof(TContainer);

        protected Property(string name)
        {
            Name = name;
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)