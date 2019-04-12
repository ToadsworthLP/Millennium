#if (NET_4_6 || NET_STANDARD_2_0)

using System;

namespace Unity.Properties
{
    /// <inheritdoc />
    /// <summary>
    /// Base class for a property that hosts a single value
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class ValueProperty<TContainer, TValue> : Property<TContainer>, IValueProperty
    {
        public Type ValueType => typeof(TValue);
        public abstract bool IsReadOnly { get; }

        protected ValueProperty(string name) : base(name)
        {
        }

        public abstract object GetObjectValue(IPropertyContainer container);
    }
    
    /*
     * CLASS PROPERTIES
     */
    
    public abstract class ValueClassPropertyBase<TContainer, TValue> : ValueProperty<TContainer, TValue>, IValueClassProperty<TContainer, TValue>
        where TContainer : class, IPropertyContainer
    {
        protected ValueClassPropertyBase(string name) 
            :base(name)
        {
        }
        
        public override object GetObjectValue(IPropertyContainer container)
        {
            return GetValue((TContainer) container);
        }

        public object GetObjectValue(TContainer container)
        {
            return GetValue(container);
        }

        public void SetObjectValue(IPropertyContainer container, object value)
        {
            SetValue((TContainer) container, TypeConversion.Convert<TValue>(value));
        }
        
        public void SetObjectValue(TContainer container, object value)
        {
            SetValue(container, TypeConversion.Convert<TValue>(value));
        }

        public TValue GetValue(IPropertyContainer container)
        {
            return GetValue((TContainer) container);
        }

        public void SetValue(IPropertyContainer container, TValue value)
        {
            SetValue((TContainer) container, value);
        }
        
        public void Accept(IPropertyContainer container, IPropertyVisitor visitor)
        {
            Accept((TContainer) container, visitor);
        }

        public abstract TValue GetValue(TContainer container);
        public abstract void SetValue(TContainer container, TValue value);
        public abstract void Accept(TContainer container, IPropertyVisitor visitor);
    }
    
    /*
     * STRUCT PROPERTIES
     */
    
    public abstract class ValueStructPropertyBase<TContainer, TValue> : ValueProperty<TContainer, TValue>, IValueStructProperty<TContainer, TValue>
        where TContainer : struct, IPropertyContainer
    {
        private struct Locals
        {
            public ValueStructPropertyBase<TContainer, TValue> Property;
            public TValue Value;
            public IPropertyVisitor Visitor;
        }
        
        protected ValueStructPropertyBase(string name) 
            :base(name)
        {
        }
        
        public override object GetObjectValue(IPropertyContainer container)
        {
            var c = (TContainer) container;
            return GetValue(ref c);
        }
        
        public object GetObjectValue(ref TContainer container)
        {
            return GetValue(ref container);
        }

        public void SetObjectValue(ref TContainer container, object value)
        {
            SetValue(ref container, TypeConversion.Convert<TValue>(value));
        }

        public void SetObjectValue(ref IPropertyContainer container, object value)
        {
            (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, Locals l) =>
            {
                l.Property.SetValue(ref c, l.Value);
            }, new Locals { Property = this, Value = TypeConversion.Convert<TValue>(value)});
        }

        public TValue GetValue(IPropertyContainer container)
        {
            var c = (TContainer) container;
            return GetValue(ref c);
        }

        public void SetValue(ref IPropertyContainer container, TValue value)
        {
            (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, Locals l) =>
            {
                l.Property.SetValue(ref c, l.Value);
            }, new Locals { Property = this, Value = value});
        }

        public void Accept(ref IPropertyContainer container, IPropertyVisitor visitor)
        {
            (container as IStructPropertyContainer<TContainer>)?.MakeRef((ref TContainer c, Locals l) =>
            {
                l.Property.Accept(ref c, l.Visitor);
            }, new Locals { Property = this, Visitor = visitor});
        }

        public abstract TValue GetValue(ref TContainer container);
        public abstract void SetValue(ref TContainer container, TValue value);
        public abstract void Accept(ref TContainer container, IPropertyVisitor visitor);
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)