#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Properties
{
    /// <summary>
    /// Base interface to for a stongly typed conversion
    /// </summary>
    /// <typeparam name="TDestination"></typeparam>
    internal interface ITypeConverter<out TDestination>
    {
        TDestination Convert(object value);
    }

    /// <inheritdoc />
    /// <summary>
    /// Interface to map a strong conversion between two types
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    internal interface ITypeConverter<in TSource, out TDestination> : ITypeConverter<TDestination>
    {
    }

    /// <inheritdoc />
    /// <summary>
    /// Delegate based type converter to host user defined implementations
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    internal class TypeConverter<TSource, TDestination> : ITypeConverter<TSource, TDestination>
    {
        private readonly Func<TSource, TDestination> m_Conversion;

        public TypeConverter(Func<TSource, TDestination> conversion)
        {
            m_Conversion = conversion;
        }

        public TDestination Convert(object value)
        {
            return m_Conversion((TSource) value);
        }
    }

    /// <summary>
    /// Conversion from a source type type to a destination type
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    internal static class TypeConversion<TSource, TDestination>
    {
        private static ITypeConverter<TSource, TDestination> s_Converter;

        /// <summary>
        /// Registers a strongly typed converter
        /// </summary>
        /// <param name="converter"></param>
        /// <typeparam name="TSource"></typeparam>
        internal static void Register(ITypeConverter<TSource, TDestination> converter)
        {
            s_Converter = converter;
        }
        
        /// <summary>
        /// Unregister the given type converter
        /// </summary>
        internal static void Unregister()
        {
            s_Converter = null;
        }

        /// <summary>
        /// Fast conversion path where types are known ahead of time
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TDestination Convert(TSource value)
        {
            return s_Converter.Convert(value);
        }
    }

    /// <summary>
    /// Conversion from a generic object source to a strongly typed destination
    /// </summary>
    /// <typeparam name="TDestination"></typeparam>
    internal static class TypeConversion<TDestination>
    {
        private static readonly Dictionary<Type, ITypeConverter<TDestination>> s_Converters = new Dictionary<Type, ITypeConverter<TDestination>>();
        
        /// <summary>
        /// Registers a new type converter
        /// </summary>
        /// <param name="converter"></param>
        /// <typeparam name="TSource"></typeparam>
        internal static void Register<TSource>(ITypeConverter<TSource, TDestination> converter)
        {
            var type = typeof(TSource);
            if (s_Converters.ContainsKey(type))
            {
                throw new Exception($"TypeConverter<{typeof(TSource)}, {typeof(TDestination)}> has already been registered");
            }

            s_Converters[typeof(TSource)] = converter;
        }

        /// <summary>
        /// Unregisters the converter from TSource to TDestination.
        /// </summary>
        internal static void Unregister<TSource>()
        {
            s_Converters.Remove(typeof(TSource));
        }

        /// <summary>
        /// Unregister all type converters
        /// </summary>
        internal static void UnregisterAll()
        {
            s_Converters.Clear();
        }

        /// <summary>
        /// Generic conversion path where destination type is known ahead of time and source type must be resolved
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TDestination Convert(object value)
        {
            var type = value.GetType();
            var converter = GetTypeConverter(type);

            if (null == converter)
            {
                throw new NullReferenceException($"No TypeConverter<,> has been registered to convert '{type}' to '{typeof(TDestination)}'");
            }

            return converter.Convert(value);
        }

        /// <summary>
        /// Generic conversion path where destination type is known ahead of time and source type must be resolved
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryConvert(object value, out TDestination result)
        {
            var type = value.GetType();
            var converter = GetTypeConverter(type);

            if (null == converter)
            {
                result = default(TDestination);
                return false;
            }

            result = converter.Convert(value);
            return true;
        }

        /// <summary>
        /// Enumerates the class hierarchy and returns the first converter found
        ///
        /// @NOTE This method will resolve base classes before interfaces
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ITypeConverter<TDestination> GetTypeConverter(Type type)
        {
            var t = type;
            while (null != t)
            {
                ITypeConverter<TDestination> converter;
                if (s_Converters.TryGetValue(t, out converter))
                {
                    return converter;
                }

                t = t.BaseType;
            }

            var interfaces = type.GetInterfaces();
            foreach (var i in interfaces)
            {
                ITypeConverter<TDestination> converter;
                if (s_Converters.TryGetValue(i, out converter))
                {
                    return converter;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// TypeConversion API
    /// </summary>
    public static class TypeConversion
    {
        private static readonly HashSet<Action> s_UnregisterMethods = new HashSet<Action>();
        
        /// <summary>
        /// Registers a new type conversion from the given source type to the given destination type
        /// </summary>
        /// <param name="conversion">Conversion delegate method</param>
        /// <typeparam name="TSource">Input type</typeparam>
        /// <typeparam name="TDestination">Output type</typeparam>
        public static void Register<TSource, TDestination>(Func<TSource, TDestination> conversion)
        {
            if (typeof(TSource) == typeof(TDestination))
            {
                throw new Exception("Failed to register conversion method, source type and destination are the same.");
            }

            var converter = new TypeConverter<TSource, TDestination>(conversion);
            
            TypeConversion<TSource, TDestination>.Register(converter);
            TypeConversion<TDestination>.Register(converter);

            s_UnregisterMethods.Add(TypeConversion<TSource, TDestination>.Unregister);
            s_UnregisterMethods.Add(TypeConversion<TDestination>.UnregisterAll);
        }

        /// <summary>
        /// Unregisters the converter for the given (TSource, TDestination) pair.
        /// </summary>
        public static void Unregister<TSource, TDestination>()
        {
            // unhook specific converter if any
            TypeConversion<TSource, TDestination>.Unregister();
            // remove generic converter if any
            TypeConversion<TDestination>.Unregister<TSource>();
        }

        private static bool IsNull<TValue>(TValue value)
        {
            if (ReferenceEquals(value, null))
            {
                return true;
            }

            // Handle fake nulls
            var obj = value as UnityEngine.Object;
            return null != obj && !obj;
        }

        /// <summary>
        /// Converts given the value to the destination type
        /// 
        /// @NOTE Fastest conversion method
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static TValue Convert<TSource, TValue>(TSource value)
        {
            return TypeConversion<TSource, TValue>.Convert(value);
        }

        /// <summary>
        /// Try convert the given value to the destination type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static bool TryConvert<TValue>(object value, out TValue result)
        {
            try
            {
                result = Convert<TValue>(value);
                return true;
            }
            catch
            {
                // ignored
            }

            result = default(TValue);
            return false;
        }
        
        /// <summary>
        /// Converts given the value to the destination type
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TDestination"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static TDestination Convert<TDestination>(object source)
        {
            // Try a straightforward cast, this is always the best case scenario
            if (source is TDestination)
            {
                return (TDestination) source;
            }

            if (typeof(TDestination).IsValueType)
            {
                // There is no elegant default behaviour we can do here, we must throw in this case
                if (source == null)
                {
                    throw new Exception($"Failed to convert from 'null' to '{typeof(TDestination)}' value is null.");
                }
            }
            else if (IsNull(source))
            {
                return default(TDestination);
            }

            // Try to forward to a user defined implementation for conversion
            TDestination result;
            if (TypeConversion<TDestination>.TryConvert(source, out result))
            {
                return result;
            }
            
            // At this point we can try our best to convert the value
            
            // Special handling of enum types
            if (typeof(TDestination).IsEnum)
            {
                var s = source as string;
                if (s != null)
                {
                    return (TDestination) Enum.Parse(typeof(TDestination), s, true);
                }

                // Try to convert to the underlying type
                var v = System.Convert.ChangeType(source, Enum.GetUnderlyingType(typeof(TDestination)));
                return (TDestination) v;
            }

            if (source is IConvertible)
            {
                return (TDestination) System.Convert.ChangeType(source, typeof(TDestination));
            }

            throw new Exception($"Failed to convert from '{source?.GetType()}' to '{typeof(TDestination)}'.");
        }

        /// <summary>
        /// Converts given the value to the destination type
        /// </summary>
        /// <param name="source"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object Convert(object source, Type type)
        {
            if (type == source?.GetType())
            {
                return source;
            }
            
            if (type.IsValueType)
            {
                // There is no elegant default behaviour we can do here, we must throw in this case
                if (source == null)
                {
                    throw new Exception($"Failed to convert from 'null' to '{type}' value is null.");
                }
            }
            else if (IsNull(source))
            {
                return null;
            }
            
            
            // Try to forward to a user defined implementation for conversion
            // object result;

            var converter = typeof(TypeConversion<>).MakeGenericType(type);
            var method = converter.GetMethod("TryConvert");

            if (null != method)
            {
                var parameters = new [] {source, null};
                var result = method.Invoke(null, parameters);
                if ((bool) result)
                {
                    return parameters[1];
                }
            }
            
            // At this point we can try our best to convert the value
            
            // Special handling of enum types
            if (type.IsEnum)
            {
                var s = source as string;
                return s != null ? Enum.Parse(type, s) : System.Convert.ChangeType(source, Enum.GetUnderlyingType(type));
            }

            if (source is IConvertible)
            {
                return System.Convert.ChangeType(source, type);
            }

            throw new Exception($"Failed to convert from '{source?.GetType()}' to '{type}'.");
        }
    }
    
    public static class TypeConversionExtensions
    {
        public static TValue GetValue<TValue>(this IPropertyContainer container, string name)
        {
            var value = (container.PropertyBag.FindProperty(name) as IValueProperty)?.GetObjectValue(container);
            return TypeConversion.Convert<TValue>(value);
        }
        
        public static TValue GetValueOrDefault<TValue>(this IPropertyContainer container, string name, TValue defaultValue = default(TValue))
        {
            var property = (container.PropertyBag.FindProperty(name) as IValueProperty);
            if (null == property)
            {
                return defaultValue;
            }
            
            var value = property?.GetObjectValue(container);
            TValue result;
            return TypeConversion.TryConvert(value, out result) ? result : defaultValue;
        }
        
        public static void SetValue<TValue>(this IPropertyContainer container, string name, TValue value)
        {
            (container.PropertyBag.FindProperty(name) as IValueClassProperty)?.SetObjectValue(container, value);
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)