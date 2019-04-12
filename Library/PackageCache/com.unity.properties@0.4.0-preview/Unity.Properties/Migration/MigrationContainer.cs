#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Unity.Properties
{
    public class MigrationException : Exception
    {
        public MigrationException(string message) : base(message)
        {
            
        }
    }
    
    /// <summary>
    /// A dynamic container to migrate property API data from one structure to another
    /// </summary>
    public class MigrationContainer : IPropertyContainer
    {
        /// <summary>
        /// Backing container that is being migrated
        /// </summary>
        private IPropertyContainer m_Container;

        /// <summary>
        /// Dynamic container to host changed data
        /// </summary>
        private readonly IDictionary<string, object> m_Data;

        /// <summary>
        /// Dynamic property bag for this container
        /// </summary>
        private readonly DynamicProperties.PropertyBag m_PropertyBag;

        public IVersionStorage VersionStorage => null;
        IPropertyBag IPropertyContainer.PropertyBag => m_PropertyBag;

        private MigrationContainer()
        {
            m_PropertyBag = new DynamicProperties.PropertyBag();
            m_Data = new Dictionary<string, object>();
        }
        
        public MigrationContainer(IPropertyContainer container) : this()
        {
            m_Container = container;

            // Initialize the `PropertyBag` with passthrough properties to the backing container
            foreach (var property in m_Container.PropertyBag.Properties)
            {
                m_PropertyBag.AddProperty(StaticProperties.CreateProperty(property));
            }
        }

        /// <summary>
        /// Returns true if the property exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasProperty(string name)
        {
            return null != m_PropertyBag.FindProperty(name);
        }

        public bool TryRename(string name, string newName)
        {
            try
            {
                Rename(name, newName);
            }
            catch (MigrationException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Renames a property
        /// </summary>
        /// <param name="name">Name of the property to rename</param>
        /// <param name="newName">New name for the property</param>
        public MigrationContainer Rename(string name, string newName)
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null == property)
            {
                throw new MigrationException($"{nameof(MigrationContainer)}.{nameof(Rename)} no property found with the name `{name}`");
            }
            
            if (null != m_PropertyBag.FindProperty(newName))
            {
                throw new MigrationException($"{nameof(MigrationContainer)}.{nameof(Rename)} property already exists with name `{newName}`");
            }

            var valueProperty = property as IValueProperty;
            if (null != valueProperty)
            {
                var value = (property as IValueProperty).GetObjectValue(this);
                var valueType = valueProperty.ValueType;

                // If needed we promote this object to a migration container
                // This is because we don't support strongly typed dynamic containers (only MigrationContainers)
                if (typeof(IPropertyContainer).IsAssignableFrom(valueProperty.ValueType))
                {
                    value = new MigrationContainer(value as IPropertyContainer);
                    valueType = typeof(MigrationContainer);
                }
                
                // Setup the backing storage for the value
                m_Data[newName] = value;
                m_PropertyBag.ReplaceProperty(name, DynamicProperties.CreateValueProperty(newName, valueType));
                
                // Clear old dynamic value if any
                if (m_Data.ContainsKey(name))
                {
                    m_Data.Remove(name);
                }
            }

            return this;
        }
        
        /// <summary>
        /// Removes a property from the container
        /// </summary>
        /// <param name="name">Name of the property to remove</param>
        public MigrationContainer Remove(string name)
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null == property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(Remove)} no property found with the name `{name}`");
            }
            
            m_PropertyBag.RemoveProperty(property);

            if (m_Data.ContainsKey(name))
            {
                m_Data.Remove(name);
            }
            
            return this;
        }
        
        /// <summary>
        /// Creates or sets the given property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        public void CreateOrSetValue<TValue>(string name, TValue value)
        {
            if (!HasProperty(name))
            {
                CreateValue(name, value);
            }
            else
            {
                SetValue(name, value);
            }
        }

        /// <summary>
        /// Creates a new value property of the given type with the default value
        /// </summary>
        /// <param name="name">Name of the property to create</param>
        public MigrationContainer CreateValue<TValue>(string name)
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null != property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(CreateValue)} property already exists with the name `{name}`");
            }

            m_Data[name] = default(TValue);
            m_PropertyBag.AddProperty(DynamicProperties.CreateValueProperty(name, typeof(TValue)));

            return this;
        }

        /// <summary>
        /// Creates a new value property of the given type and assigns the value
        /// </summary>
        /// <param name="name">Name of the property to create</param>
        /// <param name="value">Initial value</param>
        public MigrationContainer CreateValue<TValue>(string name, TValue value)
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null != property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(CreateValue)} property already exists with the name `{name}`");
            }
            
            // @TODO Type check
            
            m_Data[name] = value;
            m_PropertyBag.AddProperty(DynamicProperties.CreateValueProperty(name, typeof(TValue)));
            
            return this;
        }

        public TValue GetValueOrDefault<TValue>(string name, TValue defaultValue = default(TValue))
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null == property)
            {
                return defaultValue;
            }
            
            // @TODO Type check

            return TypeConversion.Convert<TValue>((property as IValueProperty)?.GetObjectValue(this));
        }

        /// <summary>
        /// Gets the property value as the given type
        /// </summary>
        /// <param name="name">Name of the property to get the value from</param>
        public TValue GetValue<TValue>(string name)
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null == property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(GetValue)} no property found with the name `{name}`");
            }
            
            // @TODO Type check

            return TypeConversion.Convert<TValue>((property as IValueProperty)?.GetObjectValue(this));
        }

        /// <summary>
        /// Set the property value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <typeparam name="TValue"></typeparam>
        public MigrationContainer SetValue<TValue>(string name, TValue value)
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null == property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(SetValue)} no property found with the name `{name}`");
            }

            (property as IValueClassProperty)?.SetObjectValue(this, value);
            
            return this;
        }

        /// <summary>
        /// Converts a value property to the given type
        /// </summary>
        /// <param name="name">Name of the property to convert</param>
        /// <param name="type">Type to convert to</param>
        public MigrationContainer ConvertValue(string name, Type type)
        {
            var property = m_PropertyBag.FindProperty(name);

            if (null == property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(ConvertValue)} no property found with the name `{name}`");
            }

            if (!(property is IValueProperty))
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(ConvertValue)} failed to convert property `{name}`. Propery is expected to be of type `{nameof(IValueProperty)}");
            }
            
            // Grab the existing value
            var value = (property as IValueProperty)?.GetObjectValue(this);
            
            // Setup the backing storage for the value
            if (null != value)
            {
                var converted = TypeConversion.Convert(value, type);

                if (null == converted)
                {
                    throw new Exception($"{nameof(MigrationContainer)}.{nameof(ConvertValue)} failed to convert property `{name}` to `{type}");
                }
                
                // Try to convert the object to the new type. If this fails, fallback to the defaultValue
                m_Data[name] = converted;
            }
            else
            {
                // @NOTE We do NOT assign the default value in this path since the value might have been explicitly set to null
                m_Data[name] = null;
            }

            // Replace the property with a newly generated one of the correct type
            m_PropertyBag.ReplaceProperty(name, DynamicProperties.CreateValueProperty(name, type));

            return this;
        }

        /// <summary>
        /// Converts a property to the given type
        /// </summary>
        /// <param name="name">Name of the property to convert</param>
        /// <param name="type">Type to convert to</param>
        /// <param name="defaultValue">Default value to use if conversion fails</param>
        public MigrationContainer ConvertValueOrDefault(string name, Type type, object defaultValue = null)
        {
            var property = m_PropertyBag.FindProperty(name);

            if (null == property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(ConvertValueOrDefault)} no property found with the name `{name}`");
            }
            
            // @TODO Type check

            // Grab the existing value
            var value = (property as IValueProperty)?.GetObjectValue(this);
            
            // Setup the backing storage for the value
            if (null != value)
            {
                // Try to convert the object to the new type. If this fails, fallback to the defaultValue
                m_Data[name] = TypeConversion.Convert(value, type) ?? defaultValue;
            }
            else
            {
                // @NOTE We do NOT assign the default value in this path since the value might have been explicitly set to null
                m_Data[name] = null;
            }

            // Replace the property with a newly generated one of the correct type
            m_PropertyBag.ReplaceProperty(name, DynamicProperties.CreateValueProperty(name, type));

            return this;
        }
        
        /// <summary>
        /// Creates a new dynamic migration container
        /// </summary>
        /// <param name="name">Name of the new property to create</param>
        public MigrationContainer CreateContainer(string name)
        {
            if (null != m_PropertyBag.FindProperty(name))
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(CreateContainer)} property already exists with the `{name}`");
            }
            
            var container = new MigrationContainer();
            m_Data[name] = container;
            m_PropertyBag.AddProperty(DynamicProperties.CreateValueProperty(name, typeof(MigrationContainer)));

            return container;
        }
        
        /// <summary>
        /// Gets a container property wrapped as a `MigrationContainer`
        /// </summary>
        /// <param name="name">Name of the property to get</param>
        public MigrationContainer GetContainer(string name)
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null == property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(GetContainer)} no property found with the name `{name}`");
            }
            
            // @TODO Type check
            
            MigrationContainer container;
            
            if (m_Data.ContainsKey(name))
            {
                // The value has already been moved to dynamic storage
                var value = m_Data[name];
                Assert.IsTrue(value is IPropertyContainer);
                
                if (value is MigrationContainer)
                {
                    // Already been promoted to a `MigrationContainer` return the value
                    return (MigrationContainer) m_Data[name];
                }
                
                // This value exists in dynamic storage but NOT as a `MigrationContainer`
                // This can happen if the property was renamed
                container = new MigrationContainer(value as IPropertyContainer);
                m_Data[name] = container;
                m_PropertyBag.ReplaceProperty(name, DynamicProperties.CreateValueProperty(name, typeof(MigrationContainer)));
            }
            else
            {
                // The value has not been moved to dynamic storage
                var backing = (property as IValueProperty)?.GetObjectValue(this) as IPropertyContainer;
                Assert.IsNotNull(backing);
                
                // Wrap the value in a `MigrationContainer` and store in dynamic storage 
                container = new MigrationContainer(backing);
                m_Data[name] = container;
                m_PropertyBag.ReplaceProperty(name, DynamicProperties.CreateValueProperty(name, typeof(MigrationContainer)));
            }
            
            return container;
        }

        /// <summary>
        /// Creates a new list of values property
        /// </summary>
        public MigrationList<TItem> CreateValueList<TItem>(string name)
        {
            var property = m_PropertyBag.FindProperty(name);

            if (null != property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(CreateValueList)} property already exists with the name `{name}`");
            }

            m_Data[name] = new MigrationList<TItem>();
            m_PropertyBag.AddProperty(DynamicProperties.CreateListProperty<TItem>(name));

            return m_Data[name] as MigrationList<TItem>;
        }

        /// <summary>
        /// Gets an existing list of values
        /// </summary>
        /// <param name="name"></param>
        public MigrationList<TItem> GetValueList<TItem>(string name)
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null == property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(GetValueList)} no property found with the name `{name}`");
            }
            
            // @TODO Type check
                
            var listProperty = property as IListClassProperty;
            Assert.IsNotNull(listProperty);

            MigrationList<TItem> list;
            
            if (m_Data.ContainsKey(name))
            {
                list = m_Data[name] as MigrationList<TItem>;

                if (null == list)
                {
                    throw new Exception($"{nameof(MigrationContainer)}.{nameof(GetValueList)} expected type of `{typeof(MigrationList<TItem>)} but found `{m_Data[name].GetType()}`");
                }
            }
            else
            {
                list = new MigrationList<TItem>();
                
                for (var i = 0; i < listProperty.Count(this); i++)
                {
                    list.Add((TItem) listProperty.GetObjectAt(this, i));
                }
                
                m_Data[name] = list;
                m_PropertyBag.ReplaceProperty(name, DynamicProperties.CreateListProperty<TItem>(name));
            }

            return list;
        }

        /// <summary>
        /// Converts each element of a list to the given type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public MigrationContainer ConvertValueList(string name, Type type)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts each element of a list to the given type
        /// </summary>
        /// <param name="name">Name of the property to convert</param>
        /// <param name="type">Type to convert each item to</param>
        /// <param name="defaultValue"></param>
        public MigrationContainer ConvertValueListOrDefault(string name, Type type, object defaultValue = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets an existing list of containers
        /// </summary>
        /// <param name="name"></param>
        public MigrationList<MigrationContainer> GetContainerList(string name)
        {
            var property = m_PropertyBag.FindProperty(name);
            
            if (null == property)
            {
                throw new Exception($"{nameof(MigrationContainer)}.{nameof(GetContainerList)} no property found with the name `{name}`");
            }
            
            // @TODO Type check
            
            var listProperty = property as IListClassProperty;
            Assert.IsNotNull(listProperty);

            MigrationList<MigrationContainer> list;
            
            if (m_Data.ContainsKey(name))
            {
                list = m_Data[name] as MigrationList<MigrationContainer>;
                
                if (null == list)
                {
                    throw new Exception($"{nameof(MigrationContainer)}.{nameof(GetValueList)} expected type of `{typeof(MigrationList<MigrationContainer>)} but found `{m_Data[name].GetType()}`");
                }
            }
            else
            {
                list = new MigrationList<MigrationContainer>();
                
                for (var i = 0; i < listProperty.Count(this); i++)
                {
                    list.Add(new MigrationContainer(listProperty.GetObjectAt(this, i) as IPropertyContainer));
                }
                
                m_Data[name] = list;
                m_PropertyBag.ReplaceProperty(name, DynamicProperties.CreateListProperty<MigrationContainer>(name));
            }

            return list;
        }

        private static class StaticProperties
        {
            public static IClassProperty<MigrationContainer> CreateProperty(IProperty property)
            {
                if (property is IValueProperty)
                {
                    var valueType = ((IValueProperty) property).ValueType;
                    
                    if (typeof(IPropertyContainer).IsAssignableFrom(valueType))
                    {
                        var propertyType = typeof(ClassValueProperty<>).MakeGenericType(valueType);
                        var instance = Activator.CreateInstance(propertyType, property);
                        return instance as IClassProperty<MigrationContainer>;
                    }
                    else
                    {
                        var propertyType = typeof(ValueProperty<>).MakeGenericType(valueType);
                        var instance = Activator.CreateInstance(propertyType, property);
                        return instance as IClassProperty<MigrationContainer>;
                    }
                }
                
                if (property is IListProperty)
                {
                    var itemType = ((IListProperty) property).ItemType;

                    if (typeof(IPropertyContainer).IsAssignableFrom(itemType))
                    {
                        if (itemType.IsValueType)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            var propertyType = typeof(ClassListProperty<>).MakeGenericType(itemType);
                            var instance = Activator.CreateInstance(propertyType, property);
                            return instance as IClassProperty<MigrationContainer>;
                        }
                    }
                    else
                    {
                        var propertyType = typeof(ValueListProperty<>).MakeGenericType(itemType);
                        var instance = Activator.CreateInstance(propertyType, property);
                        return instance as IClassProperty<MigrationContainer>;
                    }
                }

                return null;
            }
            
            private class ValueProperty<TValue> : ValueClassPropertyBase<MigrationContainer, TValue>
            {
                private readonly IValueProperty m_Property;

                public override bool IsReadOnly => m_Property.IsReadOnly;

                public ValueProperty(IValueProperty property) : base(property.Name)
                {
                    m_Property = property;
                }

                public override TValue GetValue(MigrationContainer container)
                {
                    var value = m_Property.GetObjectValue(container.m_Container);
                    return TypeConversion.Convert<TValue>(value);
                }

                public override void SetValue(MigrationContainer container, TValue value)
                {
                    (m_Property as IValueClassProperty)?.SetObjectValue(container.m_Container, value);
                    (m_Property as IValueStructProperty)?.SetObjectValue(ref container.m_Container, value);
                }

                public override void Accept(MigrationContainer container, IPropertyVisitor visitor)
                {
                    var context = new VisitContext<TValue> {Property = this, Value = GetValue(container), Index = -1};
                    if (!visitor.ExcludeOrCustomVisit(container, context))
                    {
                        visitor.Visit(container, context);
                    }
                }
            }
            
            private class ClassValueProperty<TValue> : ValueClassPropertyBase<MigrationContainer, TValue>
                where TValue : IPropertyContainer
            {
                private readonly IValueProperty m_Property;
                
                public override bool IsReadOnly => m_Property.IsReadOnly;
                
                public ClassValueProperty(IValueProperty property) : base(property.Name)
                {
                    m_Property = property;
                }

                public override TValue GetValue(MigrationContainer container)
                {
                    var value = m_Property.GetObjectValue(container.m_Container);
                    return TypeConversion.Convert<TValue>(value);
                }

                public override void SetValue(MigrationContainer container, TValue value)
                {
                    (m_Property as IValueClassProperty)?.SetObjectValue(container.m_Container, value);
                    (m_Property as IValueStructProperty)?.SetObjectValue(ref container.m_Container, value);
                }

                public override void Accept(MigrationContainer container, IPropertyVisitor visitor)
                {
                    var value = GetObjectValue(container) as IPropertyContainer;

                    var context = new VisitContext<IPropertyContainer>
                    {
                        Property = this,
                        Value = value,
                        Index = -1
                    };

                    if (visitor.ExcludeOrCustomVisit(container, context))
                    {
                        return;
                    }

                    if (visitor.BeginContainer(container, context))
                    {
                        value?.Visit(visitor);
                    }
                    visitor.EndContainer(container, context);
                }
            }

            private class ValueListProperty<TItem> : ListClassPropertyBase<MigrationContainer, TItem>
            {
                private readonly IListProperty m_Property;

                public ValueListProperty(IListProperty property) : base(property.Name)
                {
                    m_Property = property;
                }

                public override int Count(MigrationContainer container)
                {
                    return m_Property.Count(container.m_Container);
                }

                public override bool Contains(MigrationContainer container, TItem item)
                {
                    throw new NotImplementedException();
                }

                public override bool Remove(MigrationContainer container, TItem item)
                {
                    throw new NotImplementedException();
                }

                public override void RemoveAt(MigrationContainer container, int index)
                {
                    throw new NotImplementedException();
                }

                public override int IndexOf(MigrationContainer container, TItem value)
                {
                    throw new NotImplementedException();
                }

                public override void Insert(MigrationContainer container, int index, TItem value)
                {
                    throw new NotImplementedException();
                }

                public override void Clear(MigrationContainer container)
                {
                    (m_Property as IListStructProperty)?.Clear(ref container.m_Container);
                    (m_Property as IListClassProperty)?.Clear(container.m_Container);
                }

                public override TItem GetAt(MigrationContainer container, int index)
                {
                    if (m_Property is IListStructProperty)
                    {
                        return TypeConversion.Convert<TItem>(((IListStructProperty) m_Property).GetObjectAt(ref container.m_Container, index));
                    }
                    
                    if (m_Property is IListClassProperty)
                    {
                        return TypeConversion.Convert<TItem>(((IListClassProperty) m_Property).GetObjectAt(container.m_Container, index));
                    }

                    return default(TItem);
                }

                public override void SetAt(MigrationContainer container, int index, TItem item)
                {
                    (m_Property as IListStructProperty)?.SetObjectAt(ref container.m_Container, index, item);
                    (m_Property as IListClassProperty)?.SetObjectAt(container.m_Container, index, item);
                }

                public override TItem AddNew(MigrationContainer container)
                {
                    throw new NotImplementedException();
                }

                public override void Add(MigrationContainer container, TItem item)
                {
                    (m_Property as IListStructProperty)?.AddObject(ref container.m_Container, item);
                    (m_Property as IListClassProperty)?.AddObject(container.m_Container, item);
                }

                public override void Accept(MigrationContainer container, IPropertyVisitor visitor)
                {
                    var listContext = new VisitContext<IList<TItem>>
                    {
                        Property = this,
                        Value = null,
                        Index = -1
                    };

                    if (visitor.ExcludeOrCustomVisit(container, listContext))
                    {
                        return;
                    }

                    if (visitor.BeginCollection(container, listContext))
                    {
                        var itemVisitContext = new VisitContext<TItem>
                        {
                            Property = this
                        };

                        for (var i = 0; i < Count(container); i++)
                        {
                            var item = GetAt(container, i);

                            itemVisitContext.Value = item;
                            itemVisitContext.Index = i;

                            if (false == visitor.ExcludeOrCustomVisit(container, itemVisitContext))
                            {
                                visitor.Visit(container, itemVisitContext);
                            }
                        }
                    }

                    visitor.EndCollection(container, listContext);
                }
            }
            
            private class ClassListProperty<TItem> : ValueListProperty<TItem>
                where TItem : class, IPropertyContainer
            {
                public ClassListProperty(IListProperty property) : base(property)
                {
                }
                
                public override void Accept(MigrationContainer container, IPropertyVisitor visitor)
                {
                    var listContext = new VisitContext<IList<TItem>>
                    {
                        Property = this,
                        Value = null,
                        Index = -1
                    };

                    if (visitor.ExcludeOrCustomVisit(container, listContext))
                    {
                        return;
                    }

                    if (visitor.BeginCollection(container, listContext))
                    {
                        var itemVisitContext = new VisitContext<TItem>
                        {
                            Property = this
                        };

                        for (var i = 0; i < Count(container); i++)
                        {
                            var item = GetAt(container, i);

                            itemVisitContext.Value = item;
                            itemVisitContext.Index = i;

                            if (visitor.ExcludeOrCustomVisit(container, itemVisitContext))
                            {
                                continue;
                            }
                            
                            if (visitor.BeginContainer(container, itemVisitContext))
                            {
                                item?.Visit(visitor);
                            }
                            visitor.EndContainer(container, itemVisitContext);
                        }
                    }

                    visitor.EndCollection(container, listContext);
                }
            }
        }

        private static class DynamicProperties
        {
            public static IClassProperty<MigrationContainer> CreateValueProperty(string name, Type valueType)
            {
                if (typeof(IPropertyContainer).IsAssignableFrom(valueType))
                {
                    return new ClassValueProperty(name);
                }
                
                var propertyType = typeof(ValueProperty<>).MakeGenericType(valueType);
                var instance = Activator.CreateInstance(propertyType, name);
                return instance as IClassProperty<MigrationContainer>;
            }

            public static IClassProperty<MigrationContainer> CreateListProperty<TItem>(string name)
            {               
                return new ValueListProperty<TItem>(name);
            }

            private class ValueProperty<TValue> : ValueClassPropertyBase<MigrationContainer, TValue>
            {
                public override bool IsReadOnly => false;

                public ValueProperty(string name) : base(name)
                {
                }

                public override TValue GetValue(MigrationContainer container)
                {
                    var value = container.m_Data[Name];
                    return TypeConversion.Convert<TValue>(value);
                }

                public override void SetValue(MigrationContainer container, TValue value)
                {
                    container.m_Data[Name] = value;
                }

                public override void Accept(MigrationContainer container, IPropertyVisitor visitor)
                {
                    var context = new VisitContext<TValue> {Property = this, Value = GetValue(container), Index = -1};
                    if (!visitor.ExcludeOrCustomVisit(container, context))
                    {
                        visitor.Visit(container, context);
                    }
                }
            }

            private class ClassValueProperty : ValueClassPropertyBase<MigrationContainer, MigrationContainer>
            {
                public override bool IsReadOnly => false;
                
                public ClassValueProperty(string name) : base(name)
                {
                }

                public override MigrationContainer GetValue(MigrationContainer container)
                {
                    var value = container.m_Data[Name];
                    return TypeConversion.Convert<MigrationContainer>(value);
                }

                public override void SetValue(MigrationContainer container, MigrationContainer value)
                {
                    container.m_Data[Name] = value;
                }

                public override void Accept(MigrationContainer container, IPropertyVisitor visitor)
                {
                    var value = GetValue(container);

                    var context = new VisitContext<MigrationContainer>
                    {
                        Property = this,
                        Value = value,
                        Index = -1
                    };

                    if (visitor.ExcludeOrCustomVisit(container, context))
                    {
                        return;
                    }

                    if (visitor.BeginContainer(container, context))
                    {
                        value.Visit(visitor);
                    }
                    visitor.EndContainer(container, context);
                }
            }
            
            private class ValueListProperty<TItem> : ListClassPropertyBase<MigrationContainer, TItem>
            {
                public ValueListProperty(string name) : base(name)
                {
                }

                private MigrationList<TItem> GetList(MigrationContainer container)
                {
                    return container.m_Data[Name] as MigrationList<TItem>;
                }

                public override int Count(MigrationContainer container)
                {
                    return GetList(container)?.Count ?? 0;
                }

                public override bool Contains(MigrationContainer container, TItem item)
                {
                    return GetList(container)?.Contains(item) ?? false;
                }
                
                public override TItem AddNew(MigrationContainer container)
                {
                    throw new NotImplementedException();
                }

                public override void Add(MigrationContainer container, TItem item)
                {
                    GetList(container)?.Add(item);
                }

                public override bool Remove(MigrationContainer container, TItem item)
                {
                    GetList(container)?.Remove(item);
                    return false;
                }

                public override void RemoveAt(MigrationContainer container, int index)
                {
                    GetList(container)?.RemoveAt(index);
                }

                public override int IndexOf(MigrationContainer container, TItem value)
                {
                    return GetList(container)?.IndexOf(value) ?? -1;
                }

                public override void Insert(MigrationContainer container, int index, TItem value)
                {
                    GetList(container)?.Insert(index, value);
                }

                public override void Clear(MigrationContainer container)
                {
                    GetList(container)?.Clear();
                }

                public override TItem GetAt(MigrationContainer container, int index)
                {
                    return TypeConversion.Convert<TItem>(GetList(container)[index]);
                }

                public override void SetAt(MigrationContainer container, int index, TItem item)
                {
                    var list = GetList(container);
                    if (null == list)
                    {
                        return;
                    }
                    list[index] = item;
                }

                public override void Accept(MigrationContainer container, IPropertyVisitor visitor)
                {
                    var list = GetList(container);
                    
                    var listContext = new VisitContext<IList<TItem>>
                    {
                        Property = this,
                        Value = list,
                        Index = -1
                    };

                    if (visitor.ExcludeOrCustomVisit(container, listContext))
                    {
                        return;
                    }

                    if (visitor.BeginCollection(container, listContext))
                    {
                        var itemVisitContext = new VisitContext<TItem>
                        {
                            Property = this
                        };

                        for (var i = 0; i < Count(container); i++)
                        {
                            itemVisitContext.Value = TypeConversion.Convert<TItem>(list[i]);
                            itemVisitContext.Index = i;

                            if (false == visitor.ExcludeOrCustomVisit(container, itemVisitContext))
                            {
                                visitor.Visit(container, itemVisitContext);
                            }
                        }
                    }

                    visitor.EndCollection(container, listContext);
                }
            }

            public class PropertyBag : IPropertyBag, IVisitableClass<MigrationContainer>, IVisitableClass
            {
                private readonly IList<IClassProperty<MigrationContainer>> m_Properties = new List<IClassProperty<MigrationContainer>>();

                public int PropertyCount => m_Properties.Count;

                // ReSharper disable once MemberHidesStaticFromOuterClass
                public IEnumerable<IProperty> Properties => m_Properties;

                IProperty IPropertyBag.FindProperty(string name)
                {
                    return FindProperty(name);
                }

                public void AddProperty(IClassProperty<MigrationContainer> property)
                {
                    Assert.IsNotNull(property);
                    
                    m_Properties.Add(property);
                }

                public IClassProperty<MigrationContainer> FindProperty(string name)
                {
                    return m_Properties.FirstOrDefault(p => p.Name.Equals(name));
                }

                public void RemoveProperty(IClassProperty<MigrationContainer> property)
                {
                    m_Properties.Remove(property);
                }

                public void ReplaceProperty(string name, IClassProperty<MigrationContainer> property)
                {
                    Assert.IsNotNull(property);
                    
                    var index = m_Properties.IndexOf(FindProperty(name));
                    m_Properties[index] = property;
                }

                public void Visit(MigrationContainer container, IPropertyVisitor visitor)
                {
                    foreach (var property in m_Properties)
                    {
                        property.Accept(container, visitor);
                    }
                }

                public void Visit<TContainer>(TContainer container, IPropertyVisitor visitor) where TContainer : class, IPropertyContainer
                {
                    var typed = container as MigrationContainer;

                    Assert.IsNotNull(typed);
                    
                    foreach (var property in m_Properties)
                    {
                        property.Accept(typed, visitor);
                    }
                }
            }
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)