#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Unity.Properties
{
    /*
     * CLASS PROPERTIES
     */
    
    /// <summary>
    /// Generic implementation for lists backed by a .NET IList
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public abstract class BackedListClassPropertyBase<TContainer, TItem> : ListClassPropertyBase<TContainer, TItem> 
        where TContainer : class, IPropertyContainer
    {
        protected abstract IList<TItem> GetList(TContainer container);

        protected BackedListClassPropertyBase(string name) : base(name)
        {
        }

        public override int Count(TContainer container)
        {
            return GetList(container).Count;
        }

        public override TItem GetAt(TContainer container, int index)
        {
            return GetList(container)[index];
        }

        public override void SetAt(TContainer container, int index, TItem item)
        {
            GetList(container)[index] = item;
            container.VersionStorage?.IncrementVersion(this, container);
        }

        public override void Add(TContainer container, TItem item)
        {
            GetList(container).Add(item);
            container.VersionStorage?.IncrementVersion(this, container);
        }

        public override bool Contains(TContainer container, TItem item)
        {
            return GetList(container).Contains(item);
        }

        public override bool Remove(TContainer container, TItem item)
        {
            var result = GetList(container).Remove(item);
            if (result)
            {
                container.VersionStorage?.IncrementVersion(this, container);
            }
            return result;
        }

        public override int IndexOf(TContainer container, TItem value)
        {
            return GetList(container).IndexOf(value);
        }

        public override void Insert(TContainer container, int index, TItem value)
        {
            GetList(container).Insert(index, value);
            container.VersionStorage?.IncrementVersion(this, container);
        }
        
        public override void RemoveAt(TContainer container, int index)
        {
            GetList(container).RemoveAt(index);
            container.VersionStorage?.IncrementVersion(this, container);
        }

        public override void Clear(TContainer container)
        {
            var list = GetList(container);
            if (list.Count == 0)
            {
                return;
            }
            list.Clear();
            container.VersionStorage?.IncrementVersion(this, container);
        }
    }

    public abstract class DelegateListClassPropertyBase<TContainer, TItem> : BackedListClassPropertyBase<TContainer, TItem>
        where TContainer : class, IPropertyContainer
    {
        public delegate IList<TItem> GetListMethod(TContainer container);
        public delegate TItem CreateInstanceMethod(TContainer container);

        private readonly GetListMethod m_GetList;
        private readonly CreateInstanceMethod m_CreateInstance;
        
        protected DelegateListClassPropertyBase(string name, GetListMethod getList, CreateInstanceMethod createInstance) : base(name)
        {
            Assert.IsNotNull(getList);
            
            m_GetList = getList;
            m_CreateInstance = createInstance ?? (c => default(TItem));
        }

        protected override IList<TItem> GetList(TContainer container)
        {
            return m_GetList(container);
        }

        public override TItem AddNew(TContainer container)
        {
            var instance = m_CreateInstance(container);
            Add(container, instance);
            return instance;
        }
    }

    public class ValueListClassProperty<TContainer, TItem> : DelegateListClassPropertyBase<TContainer, TItem>
        where TContainer : class, IPropertyContainer
    {
        public ValueListClassProperty(string name, GetListMethod getList, CreateInstanceMethod createInstance = null) : base(name, getList, createInstance)
        {
        }

        public override void Accept(TContainer container, IPropertyVisitor visitor)
        {
            var listContext = new VisitContext<IList<TItem>> {Property = this, Index = -1, Value = GetList(container) };

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
                    visitor.Visit(container, itemVisitContext);
                }
            }
            visitor.EndCollection(container, listContext);
        }
    }
    
    public class ClassListClassProperty<TContainer, TItem> : DelegateListClassPropertyBase<TContainer, TItem>
        where TContainer : class, IPropertyContainer
        where TItem : class, IPropertyContainer
    {
        public ClassListClassProperty(string name, GetListMethod getList, CreateInstanceMethod createInstance = null) : base(name, getList, createInstance)
        {
        }

        public override void Accept(TContainer container, IPropertyVisitor visitor)
        {
            var listContext = new VisitContext<IList<TItem>> {Property = this, Index = -1, Value = GetList(container) };
            
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

                var count = Count(container);
                for (var i = 0; i < count; i++)
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
    
    public class StructListClassProperty<TContainer, TItem> : DelegateListClassPropertyBase<TContainer, TItem>
        where TContainer : class, IPropertyContainer
        where TItem : struct, IPropertyContainer
    {
        public StructListClassProperty(string name, GetListMethod getList, CreateInstanceMethod createInstance = null) : base(name, getList, createInstance)
        {
        }

        public override void Accept(TContainer container, IPropertyVisitor visitor)
        {
            var listContext = new VisitContext<IList<TItem>> {Property = this, Index = -1, Value = GetList(container) };
            
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

                var count = Count(container);
                for (var i = 0; i < count; i++)
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
                        PropertyContainer.Visit(ref item, visitor);
                    }
                    visitor.EndContainer(container, itemVisitContext);
                }
            }
            visitor.EndCollection(container, listContext);
        }
    }
    
    /*
     * STRUCT PROPERTIES
     */
    
    /// <summary>
    /// Generic implementation for lists backed by a .NET IList
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public abstract class BackedListStructPropertyBase<TContainer, TItem> : ListStructPropertyBase<TContainer, TItem> 
        where TContainer : struct, IPropertyContainer
    {
        protected abstract IList<TItem> GetList(ref TContainer container);

        protected BackedListStructPropertyBase(string name) : base(name)
        {
        }

        public override int Count(ref TContainer container)
        {
            return GetList(ref container).Count;
        }

        public override TItem GetAt(ref TContainer container, int index)
        {
            return GetList(ref container)[index];
        }

        public override void SetAt(ref TContainer container, int index, TItem item)
        {
            GetList(ref container)[index] = item;
            container.VersionStorage?.IncrementVersion(this, container);
        }

        public override void Add(ref TContainer container, TItem item)
        {
            GetList(ref container).Add(item);
            container.VersionStorage?.IncrementVersion(this, container);
        }

        public override bool Contains(ref TContainer container, TItem item)
        {
            return GetList(ref container).Contains(item);
        }

        public override bool Remove(ref TContainer container, TItem item)
        {
            var result = GetList(ref container).Remove(item);
            if (result)
            {
                container.VersionStorage?.IncrementVersion(this, container);
            }
            return result;
        }

        public override int IndexOf(ref TContainer container, TItem value)
        {
            return GetList(ref container).IndexOf(value);
        }

        public override void Insert(ref TContainer container, int index, TItem value)
        {
            GetList(ref container).Insert(index, value);
            container.VersionStorage?.IncrementVersion(this, container);
        }
        
        public override void RemoveAt(ref TContainer container, int index)
        {
            GetList(ref container).RemoveAt(index);
            container.VersionStorage?.IncrementVersion(this, container);
        }

        public override void Clear(ref TContainer container)
        {
            var list = GetList(ref container);
            if (list.Count == 0)
            {
                return;
            }
            list.Clear();
            container.VersionStorage?.IncrementVersion(this, container);
        }
    }
    
    public abstract class DelegateListStructPropertyBase<TContainer, TItem> : BackedListStructPropertyBase<TContainer, TItem>
        where TContainer : struct, IPropertyContainer
    {
        public delegate IList<TItem> GetListMethod(ref TContainer container);
        public delegate TItem CreateInstanceMethod(ref TContainer container);

        private readonly GetListMethod m_GetList;
        private readonly CreateInstanceMethod m_CreateInstance;
        
        protected DelegateListStructPropertyBase(string name, GetListMethod getList, CreateInstanceMethod createInstance) : base(name)
        {
            m_GetList = getList;
            m_CreateInstance = createInstance ?? ((ref TContainer c) => default(TItem));
        }

        protected override IList<TItem> GetList(ref TContainer container)
        {
            return m_GetList(ref container);
        }

        public override void AddNew(ref TContainer container)
        {
            Add(ref container, m_CreateInstance(ref container));
        }
    }

    public class ValueListStructProperty<TContainer, TItem> : DelegateListStructPropertyBase<TContainer, TItem>
        where TContainer : struct, IPropertyContainer
    {
        public ValueListStructProperty(string name, GetListMethod getList, CreateInstanceMethod createInstance = null) : base(name, getList, createInstance)
        {
        }

        public override void Accept(ref TContainer container, IPropertyVisitor visitor)
        {
            var listContext = new VisitContext<IList<TItem>> {Property = this, Index = -1, Value = GetList(ref container) };
            
            if (visitor.ExcludeOrCustomVisit(ref container, listContext))
            {
                return;
            }
           
            if (visitor.BeginCollection(ref container, listContext))
            {
                var itemVisitContext = new VisitContext<TItem>
                {
                    Property = this
                };

                for (var i = 0; i < Count(ref container); i++)
                {
                    var item = GetAt(ref container, i);
                    
                    itemVisitContext.Value = item;
                    itemVisitContext.Index = i;

                    if (false == visitor.ExcludeOrCustomVisit(ref container, itemVisitContext))
                    {
                        visitor.Visit(ref container, itemVisitContext);
                    }
                }
            }
            visitor.EndCollection(ref container, listContext);
        }
    }
    
    public class ClassListStructProperty<TContainer, TItem> : DelegateListStructPropertyBase<TContainer, TItem>
        where TContainer : struct, IPropertyContainer
        where TItem : class, IPropertyContainer
    {
        public ClassListStructProperty(string name, GetListMethod getList, CreateInstanceMethod createInstance = null) : base(name, getList, createInstance)
        {
        }

        public override void Accept(ref TContainer container, IPropertyVisitor visitor)
        {
            var listContext = new VisitContext<IList<TItem>> {Property = this, Index = -1, Value = GetList(ref container) };
            
            if (visitor.ExcludeOrCustomVisit(ref container, listContext))
            {
                return;
            }
            
            if (visitor.BeginCollection(ref container, listContext))
            {
                var itemVisitContext = new VisitContext<TItem>
                {
                    Property = this
                };

                var count = Count(container);
                for (var i = 0; i < count; i++)
                {
                    var item = GetAt(ref container, i);
                    itemVisitContext.Value = item;
                    itemVisitContext.Index = i;

                    if (visitor.ExcludeOrCustomVisit(ref container, itemVisitContext))
                    {
                        continue;
                    }
                    
                    if (visitor.BeginContainer(ref container, itemVisitContext))
                    {
                        item?.Visit(visitor);
                    }
                    visitor.EndContainer(ref container, itemVisitContext);
                }
            }
            visitor.EndCollection(ref container, listContext);
        }
    }
    
    public class StructListStructProperty<TContainer, TItem> : DelegateListStructPropertyBase<TContainer, TItem>
        where TContainer : struct, IPropertyContainer
        where TItem : struct, IPropertyContainer
    {
        public StructListStructProperty(string name, GetListMethod getList, CreateInstanceMethod createInstance = null) : base(name, getList, createInstance)
        {
        }

        public override void Accept(ref TContainer container, IPropertyVisitor visitor)
        {
            var listContext = new VisitContext<IList<TItem>> {Property = this, Index = -1, Value = GetList(ref container) };
            
            if (visitor.ExcludeOrCustomVisit(ref container, listContext))
            {
                return;
            }
            
            if (visitor.BeginCollection(ref container, listContext))
            {
                var itemVisitContext = new VisitContext<TItem>
                {
                    Property = this
                };

                var count = Count(container);
                for (var i = 0; i < count; i++)
                {
                    var item = GetAt(ref container, i);
                    itemVisitContext.Value = item;
                    itemVisitContext.Index = i;

                    if (visitor.ExcludeOrCustomVisit(ref container, itemVisitContext))
                    {
                        continue;
                    }
                    
                    if (visitor.BeginContainer(ref container, itemVisitContext))
                    {
                        PropertyContainer.Visit(ref item, visitor);
                    }
                    visitor.EndContainer(ref container, itemVisitContext);
                }
            }
            visitor.EndCollection(ref container, listContext);
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)