#if (NET_4_6 || NET_STANDARD_2_0)

using System;

namespace Unity.Properties
{
    /*
     * CLASS PROPERTIES
     */
    
    public abstract class ListClassPropertyBase<TContainer, TItem> : Property<TContainer>, IListClassProperty<TContainer, TItem>
        where TContainer : class, IPropertyContainer
    {
        public Type ItemType => typeof(TItem);

        protected ListClassPropertyBase(string name) : base(name)
        {
        }

        public object GetObjectAt(IPropertyContainer container, int index)
        {
            return GetAt((TContainer) container, index);
        }

        public void SetObjectAt(IPropertyContainer container, int index, object item)
        {
            SetAt((TContainer) container, index, TypeConversion.Convert<TItem>(item));
        }

        public TItem GetAt(IPropertyContainer container, int index)
        {
            return GetAt((TContainer) container, index);
        }

        public void SetAt(IPropertyContainer container, int index, TItem item)
        {
            SetAt((TContainer) container, index, item);
        }
        
        public int Count(IPropertyContainer container)
        {
            return Count((TContainer) container);
        }
        
        public void Clear(IPropertyContainer container)
        {
            Clear((TContainer) container);
        }
        
        public void AddObject(IPropertyContainer container, object item)
        {
            Add((TContainer) container, TypeConversion.Convert<TItem>(item));
        }

        public void RemoveAt(IPropertyContainer container, int index)
        {
            RemoveAt((TContainer) container, index);
        }

        public void Add(IPropertyContainer container, TItem item)
        {
            Add((TContainer) container, item);
        }

        public object AddNew(IPropertyContainer container)
        {
            return AddNew((TContainer) container);
        }
        
        public void Accept(IPropertyContainer container, IPropertyVisitor visitor)
        {
            Accept((TContainer) container, visitor);
        }

        public abstract int Count(TContainer container);
        public abstract void Clear(TContainer container);
        public abstract TItem GetAt(TContainer container, int index);
        public abstract void SetAt(TContainer container, int index, TItem item);
        public abstract void Add(TContainer container, TItem item);
        public abstract TItem AddNew(TContainer container);
        public abstract bool Contains(TContainer container, TItem item);
        public abstract bool Remove(TContainer container, TItem item);
        public abstract int IndexOf(TContainer container, TItem value);
        public abstract void Insert(TContainer container, int index, TItem value);
        public abstract void RemoveAt(TContainer container, int index);

        public abstract void Accept(TContainer container, IPropertyVisitor visitor);
    }
    
    /*
     * STRUCT PROPERTIES
     */
    
    public abstract class ListStructPropertyBase<TContainer, TItem> : Property<TContainer>, IListStructProperty<TContainer, TItem>
        where TContainer : struct, IPropertyContainer
    {
        public Type ItemType => typeof(TItem);

        protected ListStructPropertyBase(string name) : base(name)
        {
        }

        private struct Locals
        {
            public ListStructPropertyBase<TContainer, TItem> Property;
            public int Index;
            public TItem Item;
            public IPropertyVisitor Visitor;
        }

        public object GetObjectAt(ref IPropertyContainer container, int index)
        {
            return (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, Locals l) => l.Property.GetAt(ref c, l.Index) as object,
                new Locals {Property = this, Index = index});
        }

        public void SetObjectAt(ref IPropertyContainer container, int index, object item)
        {
            (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, Locals l) => { l.Property.SetAt(ref c, l.Index, l.Item); },
                new Locals {Property = this, Index = index, Item = TypeConversion.Convert<TItem>(item)});
        }

        public TItem GetAt(ref IPropertyContainer container, int index)
        {
            // ReSharper disable once PossibleNullReferenceException
            return (container as IStructPropertyContainer<TContainer>).MakeRef((ref TContainer c, Locals l) => l.Property.GetAt(ref c, l.Index),
                new Locals {Property = this, Index = index});
        }

        public void SetAt(ref IPropertyContainer container, int index, TItem item)
        {
            (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, Locals l) => { l.Property.SetAt(ref c, l.Index, l.Item); },
                new Locals {Property = this, Index = index, Item = item});
        }
        
        public void AddObject(ref IPropertyContainer container, object item)
        {
            (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, Locals l) => { l.Property.Add(ref c, l.Item); },
                new Locals {Property = this, Item = TypeConversion.Convert<TItem>(item)});
        }

        public void Add(ref IPropertyContainer container, TItem item)
        {
            (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, Locals l) => { l.Property.Add(ref c, l.Item); },
                new Locals {Property = this, Item = item});
        }
        
        public int Count(IPropertyContainer container)
        {
            var typed = (TContainer) container;
            return Count(ref typed);
        }

        public void Clear(ref IPropertyContainer container)
        {
            (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, ListStructPropertyBase<TContainer, TItem> p) => { p.Clear(ref c); }, this);
        }

        public void Accept(ref IPropertyContainer container, IPropertyVisitor visitor)
        {
            (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, Locals l) => { l.Property.Accept(ref c, l.Visitor); },
                new Locals {Property = this, Visitor = visitor });
        }

        public abstract int Count(ref TContainer container);
        public abstract void Clear(ref TContainer container);
        public abstract TItem GetAt(ref TContainer container, int index);
        public abstract void SetAt(ref TContainer container, int index, TItem item);
        public abstract void Add(ref TContainer container, TItem item);
        public abstract void AddNew(ref TContainer container);
        public abstract bool Contains(ref TContainer container, TItem item);
        public abstract bool Remove(ref TContainer container, TItem item);
        public abstract int IndexOf(ref TContainer container, TItem value);
        public abstract void Insert(ref TContainer container, int index, TItem value);
        public abstract void RemoveAt(ref TContainer container, int index);

        public abstract void Accept(ref TContainer container, IPropertyVisitor visitor);
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)