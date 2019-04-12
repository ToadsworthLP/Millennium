#if (NET_4_6 || NET_STANDARD_2_0)

using System;

namespace Unity.Properties
{
    /// <inheritdoc />
    /// <summary>
    /// Property over a single value
    /// </summary>
    public interface IValueProperty : IProperty
    {
        Type ValueType { get; }
        bool IsReadOnly { get; }
        
        object GetObjectValue(IPropertyContainer container);
    }

    public interface ITypedValueProperty<out TValue> : IProperty
    {
        TValue GetValue(IPropertyContainer container);
    }
    
    /*
     * CLASS PROPERTIES
     */
    
    public interface IValueClassProperty : IValueProperty
    {
        void SetObjectValue(IPropertyContainer container, object value);
    }
    
    public interface IValueClassProperty<in TContainer> : IValueClassProperty, IClassProperty<TContainer>
        where TContainer : class, IPropertyContainer
    {
        object GetObjectValue(TContainer container);
        void SetObjectValue(TContainer container, object value);
    }

    public interface IValueTypedValueClassProperty<TValue> : ITypedValueProperty<TValue>
    {
        void SetValue(IPropertyContainer container, TValue value);
    }

    public interface IValueClassProperty<in TContainer, TValue> : IValueClassProperty<TContainer>, IValueTypedValueClassProperty<TValue>
        where TContainer : class, IPropertyContainer
    {
        TValue GetValue(TContainer container);
        void SetValue(TContainer container, TValue value);
    }

    /*
     * STRUCT PROPERTIES
     */

    public interface IValueStructProperty : IValueProperty
    {
        void SetObjectValue(ref IPropertyContainer container, object value);
    }
    
    public interface IValueStructProperty<TContainer> : IValueStructProperty, IStructProperty<TContainer>
        where TContainer : struct, IPropertyContainer
    {
        object GetObjectValue(ref TContainer container);
        void SetObjectValue(ref TContainer container, object value);
    }

    public interface IValueTypedValueStructProperty<TValue> : ITypedValueProperty<TValue>
    {
        void SetValue(ref IPropertyContainer container, TValue value);
    }

    public interface IValueStructProperty<TContainer, TValue> : IValueStructProperty<TContainer>, IValueTypedValueStructProperty<TValue>
        where TContainer : struct, IPropertyContainer
    {
        TValue GetValue(ref TContainer container);
        void SetValue(ref TContainer container, TValue value);
    }
    
}

#endif // (NET_4_6 || NET_STANDARD_2_0)