#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections.Generic;

namespace Unity.Properties
{
    /// <summary>
    /// Represents a collection of <see cref="IProperty"/> objects.
    /// </summary>
    public interface IPropertyBag
    {
        /// <summary>
        /// Number of properties in the bag.
        /// </summary>
        int PropertyCount { get; }
        
        /// <summary>
        /// Enumeration of all available properties.
        /// </summary>
        IEnumerable<IProperty> Properties { get; }
        
        /// <summary>
        /// Finds a property by <see cref="IProperty.Name"/> in this bag.
        /// </summary>
        /// <param name="name">Name of the property to look for.</param>
        /// <returns>The <see cref="IProperty"/> instance, or null if not found.</returns>
        IProperty FindProperty(string name);
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)