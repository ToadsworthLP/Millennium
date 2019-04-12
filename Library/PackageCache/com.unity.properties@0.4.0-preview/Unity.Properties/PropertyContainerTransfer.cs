#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Properties
{
    public static partial class PropertyContainer
    {
        public static void Transfer<TSource, TDestination>(TSource source, TDestination destination)
            where TSource : class, IPropertyContainer
            where TDestination : class, IPropertyContainer
        {
            source.Visit(new TransferVisitor(destination));
        }
        
        public static TDestination Transfer<TSource, TDestination>(TSource source)
            where TSource : class, IPropertyContainer
            where TDestination : struct, IStructPropertyContainer<TDestination>
        {
            var visitor = new TransferVisitor(new TDestination());
            source.Visit(visitor);
            return (TDestination) visitor.Pop();
        }
        
        public static void Transfer<TSource, TDestination>(ref TSource source, TDestination destination)
            where TSource : struct, IPropertyContainer
            where TDestination : class, IPropertyContainer
        {
            Visit(ref source, new TransferVisitor(destination));
        }
        
        public static TDestination Transfer<TSource, TDestination>(ref TSource source)
            where TSource : struct, IPropertyContainer
            where TDestination : struct, IStructPropertyContainer<TDestination>
        {
            var visitor = new TransferVisitor(new TDestination());
            Visit(ref source, visitor);
            return (TDestination) visitor.Pop();
        }
        
        private class TransferVisitor : IPropertyVisitor
        {
            private readonly Stack<IPropertyContainer> m_PropertyContainers = new Stack<IPropertyContainer>();

            public TransferVisitor(IPropertyContainer container)
            {
                m_PropertyContainers.Push(container);
            }
            
            public IPropertyContainer Pop()
            {
                return m_PropertyContainers.Pop();
            }
            
            public bool ExcludeVisit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
                where TContainer : class, IPropertyContainer
            {
                return false;
            }

            public bool ExcludeVisit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context) 
                where TContainer : struct, IPropertyContainer
            {
                return false;
            }

            public bool CustomVisit<TContainer, TValue>(TContainer container, VisitContext<TValue> context) where TContainer : class, IPropertyContainer
            {
                return false;
            }

            public bool CustomVisit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context) where TContainer : struct, IPropertyContainer
            {
                return false;
            }

            public void Visit<TContainer, TValue>(TContainer container, VisitContext<TValue> context) 
                where TContainer : class, IPropertyContainer
            {
                SetPropertyValue(context.Property.Name, context.Value, context.Index);
            }

            public void Visit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
                where TContainer : struct, IPropertyContainer
            {
                SetPropertyValue(context.Property.Name, context.Value, context.Index);
            }

            public bool BeginContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context) 
                where TContainer : class, IPropertyContainer where TValue : IPropertyContainer
            {
                return PushContainer(context.Property.Name, context.Index);
            }

            public bool BeginContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context) where TContainer : struct, IPropertyContainer where TValue : IPropertyContainer
            {
                return PushContainer(context.Property.Name, context.Index);
            }
            
            public void EndContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context) where TContainer : class, IPropertyContainer where TValue : IPropertyContainer
            {
                PopContainer(context.Property.Name,  context.Value, context.Index);
            }

            public void EndContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context) where TContainer : struct, IPropertyContainer where TValue : IPropertyContainer
            {
                PopContainer(context.Property.Name, context.Value, context.Index);
            }
            
            public bool BeginCollection<TContainer, TValue>(TContainer container, VisitContext<IList<TValue>> context) where TContainer : class, IPropertyContainer
            {
                return BeginCollection(context.Property.Name);
            }

            public bool BeginCollection<TContainer, TValue>(ref TContainer container, VisitContext<IList<TValue>> context) where TContainer : struct, IPropertyContainer
            {
                return BeginCollection(context.Property.Name);
            }

            public void EndCollection<TContainer, TValue>(TContainer container, VisitContext<IList<TValue>> context) where TContainer : class, IPropertyContainer
            {
                
            }

            public void EndCollection<TContainer, TValue>(ref TContainer container, VisitContext<IList<TValue>> context) where TContainer : struct, IPropertyContainer
            {
            }

            private bool BeginCollection(string name)
            {
                var container = m_PropertyContainers.Peek();
                if (container == null)
                {
                    return false;
                }
                var property = container.PropertyBag.FindProperty(name);
                
                // When copying a list from a source to a dest we ALWAYS clear the destination list
                // There are way too many cases to cover if we attempt to do a partial copy of list elements
                
                // class properties
                (property as IListClassProperty)?.Clear(container);
                
                // struct properties
                (property as IListStructProperty)?.Clear(ref container);
                return true;
            }
            
            private bool PushContainer(string name, int index)
            {
                var container = m_PropertyContainers.Peek();
                var property = container?.PropertyBag.FindProperty(name);

                if (null == property)
                {
                    m_PropertyContainers.Push(null);
                    return false;
                }

                // @NOTE When dealing with struct containers we will incurs boxing allocations here
                //       Theres no reasonable way around this.
                IPropertyContainer value = null;
                Type type;
                
                // Non list elements
                if (index < 0)
                {
                    // Keep track of the value type for validation
                    type = (property as IValueProperty)?.ValueType;

                    // Try to unpack the value regardless of the container
                    value = value ?? (property as IValueClassProperty)?.GetObjectValue(container) as IPropertyContainer;
                    value = value ?? (property as IValueStructProperty)?.GetObjectValue(container) as IPropertyContainer;
                }
                // List elements
                else
                {
                    var listProperty = property as IListProperty;
                    
                    // Keep track of the value type for validation
                    type = listProperty?.ItemType;
                    
                    var listClassProperty = property as IListClassProperty;
                    if (null != listClassProperty)
                    {
                        // Assumptions here are based on the fact that `BeginCollection` clears the list
                        // Ensure our collection is in a valid state
                        Assert.IsTrue(listClassProperty.Count(container) == index);
                        
                        // Have the list create a new element and push it to the list
                        value = listClassProperty.AddNew(container) as IPropertyContainer;
                        
                        // Ensure an element was actually added
                        Assert.IsTrue(listClassProperty.Count(container) == index + 1);
                        
                        // We MUST remove since the add always happens in the `SetPropertyValue` method
                        // This is to support use cases such as assigning a value type to a property container through type conversions
                        listClassProperty.RemoveAt(container, index);
                    }

                    var listStructProperty = property as IListStructProperty;
                    if (null != listStructProperty)
                    {
                        // @TODO as above, missing API calls
                        throw new NotImplementedException(); 
                    }
                }

                // @NOTE It is possible that `value` is null at this point
                // This can happen in a case where dest container structure does not match the source container
                m_PropertyContainers.Push(value);
                var result = typeof(IPropertyContainer).IsAssignableFrom(type);
                
                // We should only visit the properties if we have some dest container to write to
                return result;
            }

            private void PopContainer<TValue>(string name, TValue value, int index)
            {
                // Fetch the dest container from the stack
                var container = m_PropertyContainers.Pop();

                if (null != container)
                {
                    // This container will have had `SetPropertyValue` called for any properies that match the source container
                    // We MUST call set value when dealing with collections AND to invoke the custom type conversions
                    SetPropertyValue(name, container, index);
                }
                else
                {
                    // There is NO matching dest container to write to
                    // Instead what we do is invoke the setter for the dest property and pass the source container as the value
                    //
                    // @NOTE This fits our immediate use cases but is maybe not the most generic or `correct` solution
                    //       we need to investigate this furthur based on needs
                    //
                    SetPropertyValue(name, value, index);
                }
            }

            private void SetPropertyValue<TValue>(string name, TValue value, int index)
            {
                var target = m_PropertyContainers.Peek();
                
                if (null == target)
                {
                    // No container to write to, this means the destination object is either null or the structures
                    // of the containers do not match (i.e. The nested container property is missing)
                    return;
                }
                
                var property = target.PropertyBag.FindProperty(name);
                
                if (null == property)
                {
                    // No property to write to, this means the destination object does not match the source object
                    return;
                }

                // This is a readonly property, skip assinment
                if ((property as IValueProperty)?.IsReadOnly ?? false)
                {
                    return;
                }

                // Handle class properties
                if (property is IClassProperty)
                {
                    // try to use a typed interface to avoid boxing
                    var valueTypedValueProperty = property as IValueTypedValueClassProperty<TValue>;
                    if (valueTypedValueProperty != null)
                    {
                        valueTypedValueProperty.SetValue(target, value);
                        return;
                    }
                
                    var listTypedItemProperty = property as IListTypedItemClassProperty<TValue>;
                    if (listTypedItemProperty != null)
                    {
                        listTypedItemProperty.Add(target, value);
                        return;
                    }
                
                    // fallback to object interface methods
                    (property as IValueClassProperty)?.SetObjectValue(target, value);
                    (property as IListClassProperty)?.AddObject(target, value);
                }
                // Handle struct properties
                else if (property is IStructProperty)
                {
                    // Our target container in this case is a value type
                    // We need to pop, modify and re-push to update the value type
                    m_PropertyContainers.Pop();
                    
                    // try to use a typed interface to avoid boxing
                    var valueTypedValueProperty = property as IValueTypedValueStructProperty<TValue>;
                    if (valueTypedValueProperty != null)
                    {
                        valueTypedValueProperty.SetValue(ref target, value);
                        m_PropertyContainers.Push(target);
                        return;
                    }
                
                    var listTypedItemProperty = property as IListTypedItemStructProperty<TValue>;
                    if (listTypedItemProperty != null)
                    {
                        listTypedItemProperty.Add(ref target, value);
                        m_PropertyContainers.Push(target);
                        return;
                    }
                    
                    // fallback to object interface methods
                    (property as IValueStructProperty)?.SetObjectValue(ref target, value);
                    (property as IListStructProperty)?.AddObject(ref target, value);
                    m_PropertyContainers.Push(target);
                }
            }
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)