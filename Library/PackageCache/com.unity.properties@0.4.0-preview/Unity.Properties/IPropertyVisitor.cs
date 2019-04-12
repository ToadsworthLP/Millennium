#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections.Generic;

namespace Unity.Properties
{
    internal interface IVisitableClass
    {
        void Visit<TContainer>(TContainer container, IPropertyVisitor visitor)
            where TContainer : class, IPropertyContainer;
    }

    internal interface IVisitableStruct
    {
        void Visit<TContainer>(ref TContainer container, IPropertyVisitor visitor)
            where TContainer : struct, IPropertyContainer;
    }
    
    internal interface IVisitableClass<in TContainer>
        where TContainer : IPropertyContainer
    {
        void Visit(TContainer container, IPropertyVisitor visitor);
    }
    
    internal interface IVisitableStruct<TContainer>
        where TContainer : struct, IPropertyContainer
    {
        void Visit(ref TContainer container, IPropertyVisitor visitor);
    }
    
    /// <summary>
    /// Represents a property visit context.
    /// Object created by <see cref="IProperty"/> implementations and passed to the <see cref="IPropertyVisitor"/> API.
    /// </summary>
    /// <typeparam name="TValue">Type of the <see cref="IProperty"/> value being visited.</typeparam>
    public struct VisitContext<TValue>
    {
        /// <summary>
        /// Property being visited.
        /// </summary>
        public IProperty Property;
        
        /// <summary>
        /// Property value.
        /// </summary>
        public TValue Value;
        
        /// <summary>
        /// Property value index within the collection.
        /// If the <see cref="Property"/> is not a collection property, -1 is returned.
        /// </summary>
        public int Index;
    }

    /// <summary>
    /// Base interface for all property visitors.
    /// </summary>
    /// <remarks>
    /// The intent of this interface is to support: non-allocating visits to class and struct <see cref="IPropertyContainer"/>,
    /// visit exclusion and customization mechanisms, nested property containers, and collection properties.
    /// </remarks>
    public interface IPropertyVisitor
    {
        /// <summary>
        /// Whether or not to exclude the visit of this container in the given context.
        /// </summary>
        /// <param name="container">Container about to be visited.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        /// <returns>true if the visit should be excluded, false otherwise.</returns>
        bool ExcludeVisit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer;
        
        /// <summary>
        /// Whether or not to exclude the visit of this container in the given context.
        /// </summary>
        /// <param name="container">Container about to be visited.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        /// <returns>true if the visit should be excluded, false otherwise.</returns>
        bool ExcludeVisit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer;
        
        /// <summary>
        /// Whether or not a custom visit was performed on this container in the given context.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        /// <returns>true if a custom visit was performed, false otherwise.</returns>
        bool CustomVisit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer;
        
        /// <summary>
        /// Whether or not a custom visit was performed on this container in the given context.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        /// <returns>true if a custom visit was performed, false otherwise.</returns>
        bool CustomVisit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer;
        
        /// <summary>
        /// Performs a generic visit on this container in the given context.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        void Visit<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer;
        
        /// <summary>
        /// Performs a generic visit on this container in the given context.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        void Visit<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer;
        
        /// <summary>
        /// Method called when visiting a nested <see cref="IPropertyContainer"/> property.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        /// <returns>true if the visit should enter this nested container, false otherwise.</returns>
        bool BeginContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer
            where TValue : IPropertyContainer;
        
        /// <summary>
        /// Method called when visiting a nested <see cref="IPropertyContainer"/> property.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        /// <returns>true if the visit should enter this nested container, false otherwise.</returns>
        bool BeginContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer
            where TValue : IPropertyContainer;

        /// <summary>
        /// Method always called after <see cref="BeginContainer{TContainer,TValue}(TContainer,Unity.Properties.VisitContext{TValue})"/>,
        /// regardless of its return value.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        void EndContainer<TContainer, TValue>(TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer
            where TValue : IPropertyContainer;
        
        /// <summary>
        /// Method always called after <see cref="BeginContainer{TContainer,TValue}(ref TContainer,Unity.Properties.VisitContext{TValue})"/>,
        /// regardless of its return value.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        void EndContainer<TContainer, TValue>(ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer
            where TValue : IPropertyContainer;

        /// <summary>
        /// Method called when visiting a collection property.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        /// <returns>true if the visit should enter this collection, false otherwise.</returns>
        bool BeginCollection<TContainer, TValue>(TContainer container, VisitContext<IList<TValue>> context)
            where TContainer : class, IPropertyContainer;
        
        /// <summary>
        /// Method called when visiting a collection property.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        /// <returns>true if the visit should enter this collection, false otherwise.</returns>
        bool BeginCollection<TContainer, TValue>(ref TContainer container, VisitContext<IList<TValue>> context)
            where TContainer : struct, IPropertyContainer;

        /// <summary>
        /// Method always called after <see cref="BeginCollection{TContainer,TValue}(TContainer,Unity.Properties.VisitContext{System.Collections.Generic.IList{TValue}})"/>,
        /// regardless of its return value.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        void EndCollection<TContainer, TValue>(TContainer container, VisitContext<IList<TValue>> context)
            where TContainer : class, IPropertyContainer;
        
        /// <summary>
        /// Method always called after <see cref="BeginCollection{TContainer,TValue}(ref TContainer,Unity.Properties.VisitContext{System.Collections.Generic.IList{TValue}})"/>,
        /// regardless of its return value.
        /// </summary>
        /// <param name="container">Container to visit.</param>
        /// <param name="context">Visit context object.</param>
        /// <typeparam name="TContainer">Container type.</typeparam>
        /// <typeparam name="TValue">Property value type.</typeparam>
        void EndCollection<TContainer, TValue>(ref TContainer container, VisitContext<IList<TValue>> context)
            where TContainer : struct, IPropertyContainer;
    }
    
    /// <summary>
    /// Specialized property visit.
    /// See <see cref="PropertyVisitor"/> for usage.
    /// </summary>
    /// <typeparam name="TValue">Property value type to visit.</typeparam>
    public interface ICustomVisit<in TValue>
    {
        /// <summary>
        /// Performs the visitor logic on this property value type.
        /// </summary>
        /// <param name="value">Reference to the current property value.</param>
        void CustomVisit(TValue value);
    }
    
    /// <inheritdoc cref="ICustomVisit{TValue}"/>
    /// <summary>
    /// ICustomVisit declarations for all .NET primitive types.
    /// </summary>
    public interface ICustomVisitPrimitives :
        ICustomVisit<bool>
        , ICustomVisit<byte>
        , ICustomVisit<sbyte>
        , ICustomVisit<ushort>
        , ICustomVisit<short>
        , ICustomVisit<uint>
        , ICustomVisit<int>
        , ICustomVisit<ulong>
        , ICustomVisit<long>
        , ICustomVisit<float>
        , ICustomVisit<double>
        , ICustomVisit<char>
        , ICustomVisit<string>
    {}

    /// <summary>
    /// Specialized property visit exclusion.
    /// See <see cref="PropertyVisitor"/> for usage.
    /// </summary>
    /// <typeparam name="TValue">Property value type to exclude from the visit (contravariant).</typeparam>
    public interface IExcludeVisit<in TValue>
    {
        /// <summary>
        /// Validates whether or not a property of type TValue should be visited.
        /// </summary>
        /// <param name="value">The current property value.</param>
        /// <returns>True if the property should be visited, False otherwise.</returns>
        bool ExcludeVisit(TValue value);
    }

    public static class PropertyVisitorExtensions
    {
        public static bool ExcludeOrCustomVisit<TContainer, TValue>(this IPropertyVisitor visitor, TContainer container, VisitContext<TValue> context)
            where TContainer : class, IPropertyContainer
        {
            return visitor.ExcludeVisit(container, context) || visitor.CustomVisit(container, context);
        }
        
        public static bool ExcludeOrCustomVisit<TContainer, TValue>(this IPropertyVisitor visitor, ref TContainer container, VisitContext<TValue> context)
            where TContainer : struct, IPropertyContainer
        {
            return visitor.ExcludeVisit(ref container, context) || visitor.CustomVisit(ref container, context);
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)