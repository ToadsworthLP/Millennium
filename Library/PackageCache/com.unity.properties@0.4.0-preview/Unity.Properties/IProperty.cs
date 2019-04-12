#if (NET_4_6 || NET_STANDARD_2_0)

using System;

namespace Unity.Properties
{
    /// <summary>
    /// Interface required for all property types.
    /// </summary>
    public interface IProperty 
    {
        /// <summary>
        /// The name of this property.
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Container type that hosts the data for this property.
        /// </summary>
        Type ContainerType { get; }
    }
    
    /*
     * CLASS PROPERTIES
     */

    /// <summary>
    /// Interface required for all properties where <see cref="IProperty.ContainerType"/> is a class.
    /// </summary>
    public interface IClassProperty : IProperty
    {
        /// <summary>
        /// Performs a visit on the given container.
        /// </summary>
        /// <param name="container">Generic container to visit.</param>
        /// <param name="visitor">Visitor performing the visit.</param>
        void Accept(IPropertyContainer container, IPropertyVisitor visitor);
    }
    
    /// <summary>
    /// Specialized <see cref="IClassProperty"/> for a given class container type.
    /// </summary>
    /// <typeparam name="TContainer">Property container type.</typeparam>
    public interface IClassProperty<in TContainer> : IClassProperty
        where TContainer : class, IPropertyContainer
    {
        /// <summary>
        /// Performs a visit on the given container.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="visitor">Visitor performing the visit.</param>
        void Accept(TContainer container, IPropertyVisitor visitor);
    }
    
    /*
     * STRUCT PROPERTIES
     */
    
    /// <summary>
    /// Interface required for all properties where <see cref="IProperty.ContainerType"/> is a struct.
    /// </summary>
    public interface IStructProperty : IProperty
    {
        /// <summary>
        /// Performs a visit on the given container.
        /// </summary>
        /// <param name="container">Generic container reference to visit.</param>
        /// <param name="visitor">Visitor performing the visit.</param>
        void Accept(ref IPropertyContainer container, IPropertyVisitor visitor);
    }

    /// <summary>
    /// Specialized <see cref="IStructProperty"/> for a given struct container type.
    /// </summary>
    /// <typeparam name="TContainer">Property container type.</typeparam>
    public interface IStructProperty<TContainer> : IStructProperty
        where TContainer : struct, IPropertyContainer
    {
        /// <summary>
        /// Performs a visit on the given container.
        /// </summary>
        /// <param name="container">Container reference to visit.</param>
        /// <param name="visitor">Visitor performing the visit.</param>
        void Accept(ref TContainer container, IPropertyVisitor visitor);
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)