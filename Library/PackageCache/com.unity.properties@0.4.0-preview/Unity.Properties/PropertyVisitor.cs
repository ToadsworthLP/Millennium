#if (NET_4_6 || NET_STANDARD_2_0)
using System.Collections.Generic;

namespace Unity.Properties
{
    /// <summary>
    /// Adapter for the <see cref="IPropertyVisitor"/> interface.
    /// Only primitive Visit methods are required to override.
    /// </summary>
    public abstract class PropertyVisitorAdapter : IPropertyVisitor
    {
        public virtual bool ExcludeVisit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer
        {
            return false;
        }

        public virtual bool ExcludeVisit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer
        {
            return false;
        }

        public virtual bool CustomVisit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer
        {
            return false;
        }

        public virtual bool CustomVisit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer
        {
            return false;
        }

        public abstract void Visit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer;

        public abstract void Visit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer;

        public virtual bool BeginContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer where TValue : IPropertyContainer
        {
            return true;
        }
        
        public virtual bool BeginContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer where TValue : IPropertyContainer
        {
            return true;
        }

        public virtual void EndContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer where TValue : IPropertyContainer
        {
        }

        public virtual void EndContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer where TValue : IPropertyContainer
        {
        }

        public virtual bool BeginStructContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context) 
            where TContainer : class, IPropertyContainer where TValue : struct, IPropertyContainer
        {
            return true;
        }

        public virtual bool BeginStructContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context) 
            where TContainer : struct, IPropertyContainer where TValue : struct, IPropertyContainer
        {
            return true;
        }

        public virtual void EndStructContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context) 
            where TContainer : class, IPropertyContainer where TValue : struct, IPropertyContainer
        {
        }

        public virtual void EndStructContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context) 
            where TContainer : struct, IPropertyContainer where TValue : struct, IPropertyContainer
        {
        }

        public virtual bool BeginCollection<TContainer, TValue>(TContainer container, VisitContext<IList<TValue>> context)
            where TContainer : class, IPropertyContainer
        {
            return true;
        }
        
        public virtual bool BeginCollection<TContainer, TValue>(ref TContainer container, VisitContext<IList<TValue>> context)
            where TContainer : struct, IPropertyContainer
        {
            return true;
        }

        public virtual void EndCollection<TContainer, TValue>(TContainer container, VisitContext<IList<TValue>> context)
            where TContainer : class, IPropertyContainer
        {
        }
        
        public virtual void EndCollection<TContainer, TValue>(ref TContainer container, VisitContext<IList<TValue>> context)
            where TContainer : struct, IPropertyContainer
        {
        }
    }
    
    /// <inheritdoc />
    /// <summary>
    /// Default implementation of the <see cref="T:Unity.Properties.IPropertyVisitor" /> interface.
    /// 
    /// Use this adapter to simplify read-only use cases. For read-write scenarios (e.g. IMGUI modifying the
    /// property values inline), consider using <see cref="T:Unity.Properties.PropertyVisitorAdapter" /> directly.
    /// You can define the visitor behavior by adding <see cref="T:Unity.Properties.ICustomVisit" /> and <see cref="T:Unity.Properties.IExcludeVisit" /> mixins.
    /// </summary>
    public abstract class PropertyVisitor : PropertyVisitorAdapter
    {
        /// <summary>
        /// The property being visited.
        /// </summary>
        protected IProperty Property { get; set; }
        
        /// <summary>
        /// Index for the current list item; -1 otherwise
        /// </summary>
        protected int ListIndex { get; set; }

        /// <summary>
        /// Whether or not the current property is part of a list.
        /// </summary>
        protected bool IsListItem => ListIndex >= 0;

        /// <summary>
        /// Whether or not the current property is a list property (and not an element of the list).
        /// TODO: fix this if nested lists are introduced.
        /// </summary>
        protected bool IsListProperty => (Property is IListProperty) && (false == IsListItem);

        protected virtual void VisitSetup<TContainer, TValue>(ref TContainer container, ref VisitContext<TValue> context)
            where TContainer : IPropertyContainer
        {
            Property = context.Property;
            ListIndex = context.Index;
        }

        private bool ExcludeVisitImpl<TValue>(TValue value)
        {
            var validationHandler = this as IExcludeVisit<TValue>;
            if (validationHandler != null && validationHandler.ExcludeVisit(value) || ExcludeVisit(value))
            {
                return true;
            }
            return false;
        }
        
        private bool CustomVisitImpl<TValue>(TValue value)
        {
            var handler = this as ICustomVisit<TValue>;
            if (handler == null)
            {
                return false;
            }
            
            handler.CustomVisit(value);
            return true;
        }
        
        public override bool ExcludeVisit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            if (IsListProperty)
            {
                // lists are always visited - if required, override ExcludeVisit to return true
                return false;
            }
            return ExcludeVisitImpl(context.Value);
        }
        
        public override bool ExcludeVisit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            if (IsListProperty)
            {
                // lists are always visited - if required, override ExcludeVisit to return true
                return false;
            }
            return ExcludeVisitImpl(context.Value);
        }

        protected virtual bool ExcludeVisit<TValue>(TValue value)
        {
            return false;
        }
        
        public override bool CustomVisit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            return CustomVisitImpl(context.Value);
        }

        public override bool CustomVisit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            return CustomVisitImpl(context.Value);
        }

        /// <summary>
        /// Override this method to visit generic property types (including Enum values).
        /// </summary>
        /// <param name="value">The current property value.</param>
        /// <typeparam name="TValue">The current property value type.</typeparam>
        protected abstract void Visit<TValue>(TValue value);

        protected virtual bool BeginContainer()
        {
            return true;
        }

        protected virtual void EndContainer()
        {
            
        }

        protected virtual bool BeginCollection()
        {
            return true;
        }

        protected virtual void EndCollection()
        {
            
        }
        
        public override void Visit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            Visit(context.Value);
        }

        public override void Visit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            Visit(context.Value);
        }
        
        public override bool BeginContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            return BeginContainer();
        }
        
        public override bool BeginContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            return BeginContainer();
        }
        
        public override void EndContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            EndContainer();
        }
        
        public override void EndContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            EndContainer();
        }
        
        public override bool BeginStructContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            return BeginContainer();
        }
        
        public override bool BeginStructContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            return BeginContainer();
        }
        
        public override void EndStructContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            EndContainer();
        }
        
        public override void EndStructContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
        {
            VisitSetup(ref container, ref context);
            EndContainer();
        }
        
        public override bool BeginCollection<TContainer, TValue>(TContainer container, VisitContext<IList<TValue>> context)
        {
            VisitSetup(ref container, ref context);
            return BeginCollection();
        }
        
        public override bool BeginCollection<TContainer, TValue>(ref TContainer container, VisitContext<IList<TValue>> context)
        {
            VisitSetup(ref container, ref context);
            return BeginCollection();
        }

        public override void EndCollection<TContainer, TValue>(TContainer container, VisitContext<IList<TValue>> context)
        {
            VisitSetup(ref container, ref context);
            EndCollection();
        }
        
        public override void EndCollection<TContainer, TValue>(ref TContainer container, VisitContext<IList<TValue>> context)
        {
            VisitSetup(ref container, ref context);
            EndCollection();
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)