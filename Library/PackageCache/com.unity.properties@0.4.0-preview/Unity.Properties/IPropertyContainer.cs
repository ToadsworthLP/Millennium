#if (NET_4_6 || NET_STANDARD_2_0)

namespace Unity.Properties
{
    /// <summary>
    /// Required interface for all types supported by <see cref="IProperty"/> algorithms.
    /// </summary>
    public interface IPropertyContainer
    {
        /// <summary>
        /// <see cref="IVersionStorage"/> object this container instance uses to keep track of property changes.
        /// </summary>
        IVersionStorage VersionStorage { get; }
        
        /// <summary>
        /// <see cref="IPropertyBag"/> object defining the properties known to this container.
        /// </summary>
        IPropertyBag PropertyBag { get; }
    }
    
    /// <summary>
    /// Intermediate function call allowing callers to get a struct container by reference.
    /// </summary>
    /// <param name="container">Container object reference.</param>
    /// <param name="context">Context object.</param>
    /// <typeparam name="TContainer">Type of the container object, must be a struct.</typeparam>
    /// <typeparam name="TContext">Type of the context object.</typeparam>
    public delegate void ByRef<TContainer, in TContext>(ref TContainer container, TContext context)
        where TContainer : struct, IPropertyContainer;
    
    /// <summary>
    /// Intermediate function call allowing callers to get a struct container by reference.
    /// </summary>
    /// <param name="container">Container object reference.</param>
    /// <param name="context">Context object.</param>
    /// <typeparam name="TContainer">Type of the container object, must be a struct.</typeparam>
    /// <typeparam name="TContext">Type of the context object.</typeparam>
    /// <typeparam name="TReturn">Type of the delegate's return value.</typeparam>
    public delegate TReturn ByRef<TContainer, in TContext, out TReturn>(ref TContainer container, TContext context)
        where TContainer : struct, IPropertyContainer;

    /// <summary>
    /// Interface required on all struct <see cref="IPropertyContainer"/> types.
    /// </summary>
    public interface IStructPropertyContainer : IPropertyContainer
    {
        
    }
    
    /// <summary>
    /// Specialized <see cref="IStructPropertyContainer"/> interface for a given struct container type.
    /// </summary>
    /// <typeparam name="TContainer">Container type, must be a struct.</typeparam>
    public interface IStructPropertyContainer<TContainer> : IStructPropertyContainer
        where TContainer : struct, IPropertyContainer
    {
        /// <summary>
        /// Calls the given method using this container's reference.
        /// </summary>
        /// <param name="method">Method to call.</param>
        /// <param name="context">Context object.</param>
        /// <typeparam name="TContext">Context object type.</typeparam>
        void MakeRef<TContext>(ByRef<TContainer, TContext> method, TContext context);
        
        /// <summary>
        /// Calls the given method using this container's reference.
        /// </summary>
        /// <param name="method">Method to call.</param>
        /// <param name="context">Context object.</param>
        /// <typeparam name="TContext">Context object type.</typeparam>
        /// <typeparam name="TReturn">Return value type of the given method to call.</typeparam>
        /// <returns>Return value of the given method.</returns>
        TReturn MakeRef<TContext, TReturn>(ByRef<TContainer, TContext, TReturn> method, TContext context);
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)