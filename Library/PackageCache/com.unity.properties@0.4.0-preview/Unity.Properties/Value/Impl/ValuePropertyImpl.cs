#if (NET_4_6 || NET_STANDARD_2_0)

using UnityEngine.Assertions;

namespace Unity.Properties
{
    /*
     * CLASS PROPERTIES
     */

    /// <inheritdoc cref="ValueProperty" />
    /// <summary>
    /// Base class for properties of a class
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class DelegateValueClassPropertyBase<TContainer, TValue> : ValueClassPropertyBase<TContainer, TValue>
        where TContainer : class, IPropertyContainer
    {
        public delegate void SetValueMethod(TContainer container, TValue value);
        public delegate TValue GetValueMethod(TContainer container);

        private readonly GetValueMethod m_GetValue;
        private readonly SetValueMethod m_SetValue;

        public override bool IsReadOnly => null == m_SetValue;

        protected DelegateValueClassPropertyBase(string name, GetValueMethod getValue, SetValueMethod setValue)
            : base(name)
        {
            m_GetValue = getValue;
            m_SetValue = setValue;
        }

        public override TValue GetValue(TContainer container)
        {
            return m_GetValue == null ? default(TValue) : m_GetValue(container);
        }

        public override void SetValue(TContainer container, TValue value)
        {
            if (Equals(container, value))
            {
                return;
            }
            
            m_SetValue(container, value);
            container.VersionStorage?.IncrementVersion(this, container);
        }

        private bool Equals(TContainer container, TValue other)
        {
            if (m_GetValue == null)
            {
                return false;
            }

            var value = m_GetValue(container);

            if (null == value && null == other)
            {
                return true;
            }

            return null != value && value.Equals(other);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Property of a class that holds a value
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ValueClassProperty<TContainer, TValue> : DelegateValueClassPropertyBase<TContainer, TValue>
        where TContainer : class, IPropertyContainer
    {
        public ValueClassProperty(string name, GetValueMethod getValue, SetValueMethod setValue) : base(name, getValue, setValue)
        {
        }

        public override void Accept(TContainer container, IPropertyVisitor visitor)
        {
            var context = new VisitContext<TValue> {Property = this, Value = GetValue(container), Index = -1};
            if (false == visitor.ExcludeOrCustomVisit(container, context))
            {
                visitor.Visit(container, context);
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Property of a class that hosts a nested class (IPropertyContainer) value
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ClassValueClassProperty<TContainer, TValue> : DelegateValueClassPropertyBase<TContainer, TValue>
        where TContainer : class, IPropertyContainer
        where TValue : class, IPropertyContainer
    {
        public ClassValueClassProperty(string name, GetValueMethod getValue, SetValueMethod setValue) : base(name, getValue, setValue)
        {
        }

        public override void Accept(TContainer container, IPropertyVisitor visitor)
        {
            var value = GetValue(container);
            var context = new VisitContext<TValue> {Property = this, Value = value, Index = -1};

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


    /// <inheritdoc />
    /// <summary>
    /// Property of a class thats hosts a nested struct (IPropertyContainer) value
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class StructValueClassProperty<TContainer, TValue> : DelegateValueClassPropertyBase<TContainer, TValue>
        where TContainer : class, IPropertyContainer
        where TValue : struct, IPropertyContainer
    {
        private readonly GetValueRefMethod m_GetValueRef;

        public delegate void ByRef(
            StructValueClassProperty<TContainer, TValue> property,
            TContainer container,
            ref TValue value,
            IPropertyVisitor visitor);

        public delegate void GetValueRefMethod(
            ByRef byRef,
            StructValueClassProperty<TContainer, TValue> property,
            TContainer container,
            IPropertyVisitor visitor);

        public StructValueClassProperty(string name, GetValueMethod getValue, SetValueMethod setValue, GetValueRefMethod getValueRef) : base(name, getValue, setValue)
        {
            Assert.IsNotNull(getValueRef);
            m_GetValueRef = getValueRef;
        }

        public override void Accept(TContainer container, IPropertyVisitor visitor)
        {
            m_GetValueRef((StructValueClassProperty<TContainer, TValue> p, TContainer c, ref TValue value, IPropertyVisitor v) =>
            {
                var context = new VisitContext<TValue> { Property = p, Value = value, Index = -1 };

                if (v.ExcludeOrCustomVisit(c, context))
                {
                    return;
                }

                if (v.BeginContainer(c, context))
                {
                    PropertyContainer.Visit(ref value, v);
                }

                v.EndContainer(c, context);
            }, this, container, visitor);
        }
    }

    /*
     * STRUCT PROPERTIES
     */

    /// <inheritdoc cref="ValueProperty" />
    /// <summary>
    /// Base class for properties of a struct
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public abstract class DelegateValueStructPropertyBase<TContainer, TValue> : ValueStructPropertyBase<TContainer, TValue>
        where TContainer : struct, IPropertyContainer
    {
        public delegate void SetValueMethod(ref TContainer container, TValue value);

        public delegate TValue GetValueMethod(ref TContainer container);

        private readonly GetValueMethod m_GetValue;
        private readonly SetValueMethod m_SetValue;

        public override bool IsReadOnly => null == m_SetValue;

        protected DelegateValueStructPropertyBase(string name, GetValueMethod getValue, SetValueMethod setValue)
            : base(name)
        {
            m_GetValue = getValue;
            m_SetValue = setValue;
        }

        public override TValue GetValue(ref TContainer container)
        {
            return m_GetValue == null ? default(TValue) : m_GetValue(ref container);
        }

        public override void SetValue(ref TContainer container, TValue value)
        {
            if (Equals(ref container, value))
            {
                return;
            }

            m_SetValue(ref container, value);
            container.VersionStorage?.IncrementVersion(this, container);
        }

        private bool Equals(ref TContainer container, TValue other)
        {
            if (m_GetValue == null)
            {
                return false;
            }

            var value = m_GetValue(ref container);

            if (null == value && null == other)
            {
                return true;
            }

            return null != value && value.Equals(other);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Property of a struct that holds a value
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ValueStructProperty<TContainer, TValue> : DelegateValueStructPropertyBase<TContainer, TValue>
        where TContainer : struct, IPropertyContainer
    {
        public ValueStructProperty(string name, GetValueMethod getValue, SetValueMethod setValue) : base(name, getValue, setValue)
        {
        }

        public override void Accept(ref TContainer container, IPropertyVisitor visitor)
        {
            var context = new VisitContext<TValue> {Property = this, Value = GetValue(ref container), Index = -1};
            if (!visitor.ExcludeOrCustomVisit(ref container, context))
            {
                visitor.Visit(ref container, context);
            }
        }
    }
    
    /// <inheritdoc />
    /// <summary>
    /// Property of a struct that holds a struct (IPropertyContainer) value
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class StructValueStructProperty<TContainer, TValue> : DelegateValueStructPropertyBase<TContainer, TValue>
        where TContainer : struct, IPropertyContainer
        where TValue : struct, IPropertyContainer
    {
        private readonly GetValueRefMethod m_GetValueRef;

        public delegate void ByRef(
            StructValueStructProperty<TContainer, TValue> property,
            ref TContainer container,
            ref TValue value,
            IPropertyVisitor visitor);

        public delegate void GetValueRefMethod(
            ByRef byRef,
            StructValueStructProperty<TContainer, TValue> property,
            ref TContainer container,
            IPropertyVisitor visitor);

        public StructValueStructProperty(string name, GetValueMethod getValue, SetValueMethod setValue, GetValueRefMethod getValueRef) : base(name, getValue, setValue)
        {
            Assert.IsNotNull(getValueRef);
            m_GetValueRef = getValueRef;
        }
        
        public override void Accept(ref TContainer container, IPropertyVisitor visitor)
        {
            m_GetValueRef((StructValueStructProperty<TContainer, TValue> p, ref TContainer c, ref TValue value, IPropertyVisitor v) =>
            {
                var context = new VisitContext<TValue> { Property = p, Value = value, Index = -1 };

                if (v.ExcludeOrCustomVisit(ref c, context))
                {
                    return;
                }

                if (v.BeginContainer(ref c, context))
                {
                    PropertyContainer.Visit(ref value, v);
                }
                v.EndContainer(ref c, context);
            }, this, ref container, visitor);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Property of a struct that holds a class (IPropertyContainer) value
    /// </summary>
    /// <typeparam name="TContainer"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ClassValueStructProperty<TContainer, TValue> : DelegateValueStructPropertyBase<TContainer, TValue>
        where TContainer : struct, IPropertyContainer
        where TValue : class, IPropertyContainer
    {
        public ClassValueStructProperty(string name, GetValueMethod getValue, SetValueMethod setValue)
            : base(name, getValue, setValue)
        {}

        public override void Accept(ref TContainer container, IPropertyVisitor visitor)
        {
            var value = GetValue(container);
            var context = new VisitContext<TValue> { Property = this, Value = value, Index = -1 };

            if (visitor.ExcludeOrCustomVisit(ref container, context))
            {
               return;
            }

            if (visitor.BeginContainer(ref container, context))
            {
                value?.Visit(visitor);
            }

            visitor.EndContainer(ref container, context);
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)