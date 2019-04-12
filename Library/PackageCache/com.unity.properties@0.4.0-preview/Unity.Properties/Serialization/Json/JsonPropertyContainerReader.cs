#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Unity.Properties.Serialization
{
    public static class TypeFactory
    {
        public delegate IPropertyContainer TypeConstructor();

        private static readonly Dictionary<Type, TypeConstructor> s_Constructors = new Dictionary<Type, TypeConstructor>();

        public static void Register(Type type, TypeConstructor constructor)
        {
            s_Constructors[type] = constructor;
        }

        public static IPropertyContainer CreateInstance(Type type)
        {
            if (s_Constructors.ContainsKey(type))
            {
                return s_Constructors[type].Invoke();
            }

            return Activator.CreateInstance(type) as IPropertyContainer;
        }
    }
    
    public static class JsonPropertyContainerReader
    {
        /// <summary>
        /// Parses the given string and returns a typed IPropertyContainer
        /// </summary>
        /// <param name="json"></param>
        /// <typeparam name="TContainer"></typeparam>
        /// <returns></returns>
        public static TContainer Read<TContainer>(string json)
            where TContainer : class, IPropertyContainer
        {
            object obj;
            return !Json.TryDeserializeObject(json, out obj) ? default(TContainer) : Read<TContainer>(obj);
        }
        
        /// <summary>
        /// Parses a .NET object tree and creates containers
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="TContainer"></typeparam>
        /// <returns></returns>
        private static TContainer Read<TContainer>(object obj)
            where TContainer : class, IPropertyContainer
        {
            return ParseContainer(typeof(TContainer), obj as IDictionary<string, object>) as TContainer;
        }

        /// <summary>
        /// Parses the given dictionary into a new instance of an IPropertyContainer
        /// </summary>
        private static IPropertyContainer ParseContainer(Type type, IDictionary<string, object> obj)
        {
            var container = TypeFactory.CreateInstance(type);
            ParseContainer(ref container, obj);
            return container;
        }

        /// <summary> 
        /// Parses the given dictionary into the given instance of IPropertyContainer
        /// </summary>
        private static void ParseContainer(ref IPropertyContainer container, IDictionary<string, object> obj)
        {
            Assert.IsNotNull(obj);

            foreach (var property in container.PropertyBag.Properties)
            {
                object value;
                
                if (!obj.TryGetValue(property.Name, out value))
                {
                    // @TODO warning?
                    continue;
                }

                ParseProperty(ref container, property, value);
            }
        }

        private static void ParseProperty(ref IPropertyContainer container, IProperty property, object value)
        {
            var valueStructProperty = property as IValueStructProperty;
            valueStructProperty?.SetObjectValue(ref container, ParseValue(valueStructProperty.ValueType, value));

            var valueClassProperty = property as IValueClassProperty;
            valueClassProperty?.SetObjectValue(container, ParseValue(valueClassProperty.ValueType, value));
            
            /*
            if (listStructProperty != null)
            {
                if (container.GetType().IsValueType)
                {
                    throw new NotSupportedException("Generic deserialization for list properties on structs is not supported!");
                }

                if (listStructProperty.GetObjectValue(container) == null)
                {
                    throw new NotSupportedException("Generic list construction is not supported, lists must be created in the object constructor");
                }

                var valueList = value as IList<object>;

                if (valueList == null)
                {
                    throw new Exception("Expected object value to be a list");
                }

                foreach (var v in valueList)
                {
                    listStructProperty.AddObject(container, ParseValue(listStructProperty.ItemType, v));
                }
            }
            else if (typeof(IPropertyContainer).IsAssignableFrom(property.ValueType))
            {
                var c = property.GetObjectValue(container) as IPropertyContainer;

                if (c == null)
                {
                    throw new NotSupportedException("Generic object construction is not supported, objects must be created in the object constructor");
                }

                var valueDictionary = value as IDictionary<string, object>;

                if (valueDictionary == null)
                {
                    throw new Exception("Expected object value to be a dictionary");
                }

                ParseContainer(ref c, valueDictionary);
            }
            else
            {
                property.SetObjectValue(container, ParseValue(property.ValueType, value));
            }
            */
        }

        private static object ParseValue(Type type, object value)
        {
            var valueDictionary = value as IDictionary<string, object>;
            if (valueDictionary != null)
            {
                Type valueType;
                if (!TryGetType(valueDictionary, out valueType))
                {
                    valueType = type;
                }

                Assert.IsTrue(type.IsAssignableFrom(valueType), $"{type}.IsAssignableFrom({valueType}) == false");
                return ParseContainer(valueType, valueDictionary);
            }

            return type.IsEnum ? Enum.ToObject(type, value) : value;
        }

        private static bool TryGetType(IDictionary<string, object> obj, out Type type)
        {
            object typeObj;
            if (!obj.TryGetValue("$Type", out typeObj))
            {
                type = null;
                return false;
            }

            var typeName = typeObj as string;

            if (string.IsNullOrEmpty(typeName))
            {
                type = null;
                return false;
            }

            type = Type.GetType(typeName);
            return type != null;
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)