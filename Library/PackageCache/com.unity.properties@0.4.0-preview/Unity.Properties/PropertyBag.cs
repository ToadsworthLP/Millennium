#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Unity.Properties
{
    /// <inheritdoc cref="IPropertyBag" />
    ///  <summary>
    ///  Base class for property bags
    ///  Implemented the default visit path
    ///  </summary>
    ///  <typeparam name="TProperty"></typeparam>
    public abstract class PropertyBag<TProperty> : IPropertyBag, IVisitableClass, IVisitableStruct
        where TProperty : IProperty
    {
        // ReSharper disable once InconsistentNaming
        protected readonly List<TProperty> m_PropertiesList = new List<TProperty>();
        private readonly Dictionary<string, TProperty> m_PropertiesDictionary = new Dictionary<string, TProperty>();
        
        public int PropertyCount => m_PropertiesList.Count;
        IEnumerable<IProperty> IPropertyBag.Properties => m_PropertiesList.Cast<IProperty>();
        public IEnumerable<TProperty> Properties => m_PropertiesList;

        protected PropertyBag(params TProperty[] properties)
            : this((IEnumerable<TProperty>)properties)
        {
        }

        protected PropertyBag(IEnumerable<TProperty> properties)
        {
            m_PropertiesList.AddRange(properties);
            
            foreach (var property in m_PropertiesList)
            {
                Assert.IsFalse(m_PropertiesDictionary.ContainsKey(property.Name), $"PropertyBag already contains a property named {property.Name}");
                m_PropertiesDictionary[property.Name] = property;
            }
        }

        IProperty IPropertyBag.FindProperty(string name)
        {
            return FindProperty(name);
        }

        public TProperty FindProperty(string name)
        {
            TProperty property;
            return m_PropertiesDictionary.TryGetValue(name, out property) ? property : default(TProperty);
        }
        
        public void AddProperty(TProperty property)
        {
            Assert.IsFalse(m_PropertiesDictionary.ContainsKey(property.Name));
            
            m_PropertiesList.Add(property);
            m_PropertiesDictionary[property.Name] = property;
        }

        public void RemoveProperty(TProperty property)
        {
            m_PropertiesList.Remove(property);
            m_PropertiesDictionary.Remove(property.Name);
        }

        public void Clear()
        {
            m_PropertiesList.Clear();
            m_PropertiesDictionary.Clear();
        }

        /// <summary>
        /// Default visit path, this will cast each property and perform a visit
        /// </summary>
        /// <param name="container"></param>
        /// <param name="visitor"></param>
        /// <typeparam name="TContainer"></typeparam>
        public void Visit<TContainer>(TContainer container, IPropertyVisitor visitor)
            where TContainer : class, IPropertyContainer
        {
            for (int i = 0, count = m_PropertiesList.Count; i < count; i++)
            {
                var property = m_PropertiesList[i];
                var typed = property as IClassProperty<TContainer>;
                
                if (typed != null)
                {
                    typed.Accept(container, visitor);
                }
                else
                {
                    // Valid scenario when IPropertyContainer is used as TContainer for a class
                    (property as IClassProperty)?.Accept(container, visitor);

                    // Valid scenario when IPropertyContainer is used as TContainer for a struct
                    // @TODO Fixme
                    IPropertyContainer c = container;
                    (property as IStructProperty)?.Accept(ref c, visitor);
                }
            }
        }

        /// <summary>
        /// Default visit path, this will cast each property and perform a visit
        /// </summary>
        /// <param name="container"></param>
        /// <param name="visitor"></param>
        /// <typeparam name="TContainer"></typeparam>
        public void Visit<TContainer>(ref TContainer container, IPropertyVisitor visitor)
            where TContainer : struct, IPropertyContainer
        {
            for (int i = 0, count = m_PropertiesList.Count; i < count; i++)
            {
                (m_PropertiesList[i] as IStructProperty<TContainer>)?.Accept(ref container, visitor);
            }
        }
    }
    
    /// <inheritdoc />
    /// <summary>
    /// Simple untyped property bag
    /// </summary>
    public class PropertyBag : PropertyBag<IProperty>
    {
        public PropertyBag()
        {
        }

        public PropertyBag(params IProperty[] properties) : base(properties)
        {
        }

        public PropertyBag(IEnumerable<IProperty> properties) : base(properties)
        {
        }
    }
    
    /// <inheritdoc />
    /// <summary>
    /// Typed container property bag for class types
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    public class ClassPropertyBag<TContainer> : PropertyBag<IClassProperty<TContainer>>, IVisitableClass<TContainer>
        where TContainer : class, IPropertyContainer
    {
        public ClassPropertyBag()
        {
        }

        public ClassPropertyBag(params IClassProperty<TContainer>[] properties) : base(properties)
        {
        }

        public ClassPropertyBag(IEnumerable<IClassProperty<TContainer>> properties) : base(properties)
        {
        }
        
        /// <summary>
        /// Fast visit path
        /// </summary>
        /// <param name="container"></param>
        /// <param name="visitor"></param>
        public void Visit(TContainer container, IPropertyVisitor visitor)
        {
            for (int i = 0, count = m_PropertiesList.Count; i < count; i++)
            {
                m_PropertiesList[i].Accept(container, visitor);
            }
        }
    }
    
    /// <summary>
    /// Typed container property bag for struct types
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    public class StructPropertyBag<TContainer> : PropertyBag<IStructProperty<TContainer>>, IVisitableStruct<TContainer>
        where TContainer : struct, IPropertyContainer
    {
        public StructPropertyBag()
        {
        }

        public StructPropertyBag(params IStructProperty<TContainer>[] properties) : base(properties)
        {
        }

        public StructPropertyBag(IEnumerable<IStructProperty<TContainer>> properties) : base(properties)
        {
        }
        
        /// <summary>
        /// Fast visit path
        /// </summary>
        /// <param name="container"></param>
        /// <param name="visitor"></param>
        public void Visit(ref TContainer container, IPropertyVisitor visitor)
        {
            for (int i = 0, count = m_PropertiesList.Count; i < count; i++)
            {
                m_PropertiesList[i].Accept(ref container, visitor);
            }
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)