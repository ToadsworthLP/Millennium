#if (NET_4_6 || NET_STANDARD_2_0)

using System;

namespace Unity.Properties
{
    /// <inheritdoc />
    /// <summary>
    /// Property representing a list or array of items
    /// </summary>
    public interface IListProperty : IProperty
    {
        Type ItemType { get; }
        
        int Count(IPropertyContainer container);
    }

    /*
     * CLASS PROPERTIES
     */
    
    public interface IListClassProperty : IListProperty
    {
        object GetObjectAt(IPropertyContainer container, int index);
        void SetObjectAt(IPropertyContainer container, int index, object item);
        void AddObject(IPropertyContainer container, object item);
        void RemoveAt(IPropertyContainer container, int index);
        void Clear(IPropertyContainer container);
        object AddNew(IPropertyContainer container);
    }

    public interface IListTypedItemClassProperty<TItem> : IListClassProperty
    {
        TItem GetAt(IPropertyContainer container, int index);
        void SetAt(IPropertyContainer container, int index, TItem item);
        void Add(IPropertyContainer container, TItem item);
    }

    public interface IListClassProperty<in TContainer> : IListClassProperty, IClassProperty<TContainer>
        where TContainer : class, IPropertyContainer
    {
        int Count(TContainer container);
    }

    public interface IListClassProperty<in TContainer, TItem> : IListClassProperty<TContainer>, IListTypedItemClassProperty<TItem>
        where TContainer : class, IPropertyContainer
    {
        TItem GetAt(TContainer container, int index);
        void SetAt(TContainer container, int index, TItem item);
        void Add(TContainer container, TItem item);
        bool Remove(TContainer container, TItem item);
        bool Contains(TContainer container, TItem item);
        void Clear(TContainer container);
        void Insert(TContainer container, int index, TItem value);
        void RemoveAt(TContainer container, int index);
        int IndexOf(TContainer container, TItem value);

        /// <summary>
        /// Creates and Adds a new instance of `TItem` to the list
        /// </summary>
        TItem AddNew(TContainer container);
    }

    /*
     * STRUCT PROPERTIES
     */

    public interface IListStructProperty : IListProperty
    {
        object GetObjectAt(ref IPropertyContainer container, int index);
        void SetObjectAt(ref IPropertyContainer container, int index, object item);
        void AddObject(ref IPropertyContainer container, object item);
        void Clear(ref IPropertyContainer container);
    }
    
    public interface IListTypedItemStructProperty<TItem> : IListStructProperty
    {
        TItem GetAt(ref IPropertyContainer container, int index);
        void SetAt(ref IPropertyContainer container, int index, TItem item);
        void Add(ref IPropertyContainer container, TItem item);
    }

    public interface IListStructProperty<TContainer> : IListStructProperty, IStructProperty<TContainer>
        where TContainer : struct, IPropertyContainer
    {
        int Count(ref TContainer container);
    }

    public interface IListStructProperty<TContainer, TItem> : IListStructProperty<TContainer>, IListTypedItemStructProperty<TItem>
        where TContainer : struct, IPropertyContainer
    {
        TItem GetAt(ref TContainer container, int index);
        void SetAt(ref TContainer container, int index, TItem item);
        void Add(ref TContainer container, TItem item);
        bool Remove(ref TContainer container, TItem item);
        bool Contains(ref TContainer container, TItem item);
        void Clear(ref TContainer container);
        void Insert(ref TContainer container, int index, TItem value);
        void RemoveAt(ref TContainer container, int index);
        int IndexOf(ref TContainer container, TItem value);
        
        /// <summary>
        /// Creates and Adds a new instance of `TItem` to the list
        /// </summary>
        void AddNew(ref TContainer container);
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)