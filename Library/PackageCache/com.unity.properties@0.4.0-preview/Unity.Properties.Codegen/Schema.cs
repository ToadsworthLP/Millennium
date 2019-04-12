#if (NET_4_6 || NET_STANDARD_2_0)

using System.Collections.Generic;

namespace Unity.Properties.Codegen
{
    public enum AccessModifier
    {
        None,
        Public,
        Private,
        Internal
    }

    public enum PropertyType
    {
        /// <summary>
        /// Value property (primitive or custom handled types)
        /// </summary>
        Value,
        
        /// <summary>
        /// A nested class `IPropertyContainer`
        /// </summary>
        ClassValue,
        
        /// <summary>
        /// A nested struct `IPropertyContainer`
        /// </summary>
        StructValue,
        
        /// <summary>
        /// A list of values (primitive or custom handled types)
        /// </summary>
        ValueList,
        
        /// <summary>
        /// A list of class `IPropertyContainer`
        /// </summary>
        ClassList,
        
        /// <summary>
        /// A list of struct `IPropertyContainer`
        /// </summary>
        StructList
    }
    
    /// <summary>
    /// Property API schema to describe a collection of types and thier properties
    ///
    /// This class is consumed during code-generation
    /// </summary>
    public class Schema : IPropertyContainer
    {
        public static class Property
        {
            public static readonly ValueClassProperty<Schema, int> Version = new ValueClassProperty<Schema, int>(
                "Version",
                c => c.m_Version,
                (c, v) => c.m_Version = v
            );

            public static readonly ValueListClassProperty<Schema, string> UsingAssemblies = new ValueListClassProperty<Schema, string>(
                "UsingAssemblies",
                c => c.m_UsingAssemblies
            );

            public static readonly ClassListClassProperty<Schema, TypeNode> Types = new ClassListClassProperty<Schema, TypeNode>(
                "Types",
                c => c.m_Types,
                c => new TypeNode()
            );

            public static readonly ClassPropertyBag<Schema> PropertyBag = new ClassPropertyBag<Schema>(
                Version,
                UsingAssemblies,
                Types
            );
        }

        public IPropertyBag PropertyBag => Property.PropertyBag;
        public IVersionStorage VersionStorage => null;

        private int m_Version;
        private readonly IList<string> m_UsingAssemblies = new List<string>();
        private readonly IList<TypeNode> m_Types = new List<TypeNode>();

        /// <summary>
        /// Property API version for this schema; used for migration and backwards compatibility
        /// </summary>
        public int Version
        {
            get { return Property.Version.GetValue(this); }
            set { Property.Version.SetValue(this, value); }
        }

        /// <summary>
        /// List of assembly names that should be included in the generated code
        /// </summary>
        public PropertyList<Schema, string> UsingAssemblies => new PropertyList<Schema, string>(Property.UsingAssemblies, this);
        
        /// <summary>
        /// Type definitions for this schema
        /// </summary>
        public PropertyList<Schema, TypeNode> Types => new PropertyList<Schema, TypeNode>(Property.Types, this);
    }

    public class TypeNode : IPropertyContainer
    {
        private static class Property
        {
            public static readonly ValueClassProperty<TypeNode, string> Namespace = new ValueClassProperty<TypeNode, string>(
                "Namespace",
                c => c.m_Namespace,
                (c, v) => c.m_Namespace = v
            );
            
            public static readonly ValueClassProperty<TypeNode, string> Name = new ValueClassProperty<TypeNode, string>(
                "Name",
                c => c.m_Name,
                (c, v) => c.m_Name = v
            );
            
            public static readonly ValueClassProperty<TypeNode, AccessModifier> AccessModifier = new ValueClassProperty<TypeNode, AccessModifier>(
                "AccessModifier",
                c => c.m_AccessModifier,
                (c, v) => c.m_AccessModifier = v
            );
            
            public static readonly ValueClassProperty<TypeNode, bool> IsAbstract = new ValueClassProperty<TypeNode, bool>(
                "IsAbstract",
                c => c.m_IsAbstract,
                (c, v) => c.m_IsAbstract = v
            );
            
            public static readonly ValueClassProperty<TypeNode, bool> IsStruct = new ValueClassProperty<TypeNode, bool>(
                "IsStruct",
                c => c.m_IsStruct,
                (c, v) => c.m_IsStruct = v
            );
            
            public static readonly ValueClassProperty<TypeNode, bool> IncludeStaticConstructor = new ValueClassProperty<TypeNode, bool>(
                "IncludeStaticConstructor", 
                c => c.m_IncludeStaticConstructor,
                (c, v) => c.m_IncludeStaticConstructor = v
            );
            
            public static readonly ValueClassProperty<TypeNode, bool> IncludePropertyBagAccessor = new ValueClassProperty<TypeNode, bool>(
                "IncludePropertyBagAccessor", 
                c => c.m_IncludePropertyBagAccessor,
                (c, v) => c.m_IncludePropertyBagAccessor = v
            );
            
            public static readonly ValueClassProperty<TypeNode, bool> IncludeVersionStorageAccessor = new ValueClassProperty<TypeNode, bool>(
                "IncludeVersionStorageAccessor", 
                c => c.m_IncludeVersionStorageAccessor,
                (c, v) => c.m_IncludeVersionStorageAccessor = v
            );

            public static readonly ClassListClassProperty<TypeNode, PropertyNode> Properties = new ClassListClassProperty<TypeNode, PropertyNode>(
                "Properties",
                c => c.m_Properties,
                c => new PropertyNode()
            );
            
            public static readonly ClassListClassProperty<TypeNode, TypeNode> NestedTypes = new ClassListClassProperty<TypeNode, TypeNode>(
                "NestedTypes",
                c => c.m_NestedTypes,
                c => new TypeNode()
            );

            public static readonly ClassPropertyBag<TypeNode> PropertyBag = new ClassPropertyBag<TypeNode>(
                Namespace,
                Name,
                AccessModifier,
                IsAbstract,
                IsStruct,
                IncludeStaticConstructor,
                IncludePropertyBagAccessor,
                IncludeVersionStorageAccessor,
                Properties,
                NestedTypes
            );
        }

        public IPropertyBag PropertyBag => Property.PropertyBag;
        public IVersionStorage VersionStorage => null;

        private string m_Name;
        private string m_Namespace;
        private AccessModifier m_AccessModifier = AccessModifier.Public;
        private bool m_IsAbstract;
        private bool m_IsStruct;
        private bool m_IncludeStaticConstructor = true;
        private bool m_IncludePropertyBagAccessor = true;
        private bool m_IncludeVersionStorageAccessor = true;
        private readonly IList<PropertyNode> m_Properties = new List<PropertyNode>();
        private readonly IList<TypeNode> m_NestedTypes = new List<TypeNode>();

        /// <summary>
        /// TypeName for this type
        /// </summary>
        public string Name
        {
            get { return Property.Name.GetValue(this); }
            set { Property.Name.SetValue(this, value); }
        }

        /// <summary>
        /// Namespace for this type
        /// </summary>
        public string Namespace
        {
            get { return Property.Namespace.GetValue(this); }
            set { Property.Namespace.SetValue(this, value); }
        }
        
        /// <summary>
        /// .NET access modifier for this type <see cref="AccessModifier"/>
        /// </summary>
        public AccessModifier AccessModifier
        {
            get { return Property.AccessModifier.GetValue(this); }
            set { Property.AccessModifier.SetValue(this, value); }
        }

        /// <summary>
        /// Should this type be generated as abstract
        /// </summary>
        public bool IsAbstract
        {
            get { return Property.IsAbstract.GetValue(this); }
            set { Property.IsAbstract.SetValue(this, value); }
        }

        /// <summary>
        /// Should this type be generated as a ValueType
        /// </summary>
        public bool IsStruct
        {
            get { return Property.IsStruct.GetValue(this); }
            set { Property.IsStruct.SetValue(this, value); }
        }
        
        /// <summary>
        /// Should the static constructor be generated for this type
        ///
        /// if false; the implementor is responsible for calling `InitializeProperties()` and `InitializePropertyBag()` at the appropriate time
        ///
        /// defaults to `true`
        /// </summary>
        public bool IncludeStaticConstructor
        {
            get { return Property.IncludeStaticConstructor.GetValue(this); }
            set { Property.IncludeStaticConstructor.SetValue(this, value); }
        }
        
        /// <summary>
        /// Should the PropertyBag accessor be generated for this type
        ///
        /// if false; the implementor is responsible for implmenting the `PropertyBag` interface accessor
        ///
        /// defaults to `true`
        /// </summary>
        public bool IncludePropertyBagAccessor
        {
            get { return Property.IncludePropertyBagAccessor.GetValue(this); }
            set { Property.IncludePropertyBagAccessor.SetValue(this, value); }
        }
        
        /// <summary>
        /// Should the VersionStorage accessor be generated for this type
        ///
        /// if false; the implementor is responsible for implmenting the `VersionStorage` interface accessor
        ///
        /// defaults to `true`
        /// </summary>
        public bool IncludeVersionStorageAccessor
        {
            get { return Property.IncludeVersionStorageAccessor.GetValue(this); }
            set { Property.IncludeVersionStorageAccessor.SetValue(this, value); }
        }

        public PropertyList<TypeNode, PropertyNode> Properties => new PropertyList<TypeNode, PropertyNode>(Property.Properties, this);
        public PropertyList<TypeNode, TypeNode> NestedTypes => new PropertyList<TypeNode, TypeNode>(Property.NestedTypes, this);
    }

    public class PropertyNode : IPropertyContainer
    {
        private static class Property
        {
            public static readonly ValueClassProperty<PropertyNode, string> Name = new ValueClassProperty<PropertyNode, string>(
                "Name",
                c => c.m_Name,
                (c, v) => c.m_Name = v
            );

            public static readonly ValueClassProperty<PropertyNode, string> ValueType = new ValueClassProperty<PropertyNode, string>(
                "ValueType",
                c => c.m_ValueType,
                (c, v) => c.m_ValueType = v
            );
            
            public static readonly ValueClassProperty<PropertyNode, PropertyType> PropertyType = new ValueClassProperty<PropertyNode, PropertyType>(
                "PropertyType",
                c => c.m_PropertyType,
                (c, v) => c.m_PropertyType = v
            );
            
            public static readonly ValueClassProperty<PropertyNode, string> Documentation = new ValueClassProperty<PropertyNode, string>(
                "Documentation",
                c => c.m_Documentation,
                (c, v) => c.m_Documentation = v
            );
            
            public static readonly ValueClassProperty<PropertyNode, bool> IncludeDeclaration = new ValueClassProperty<PropertyNode, bool>(
                "IncludeDeclaration",
                c => c.m_IncludeDeclaration,
                (c, v) => c.m_IncludeDeclaration = v
            );
            
            public static readonly ValueClassProperty<PropertyNode, bool> IncludeInitializer = new ValueClassProperty<PropertyNode, bool>(
                "IncludeInitializer",
                c => c.m_IncludeInitializer,
                (c, v) => c.m_IncludeInitializer = v
            );
            
            public static readonly ValueClassProperty<PropertyNode, bool> IncludeBackingField = new ValueClassProperty<PropertyNode, bool>(
                "IncludeBackingField",
                c => c.m_IncludeBackingField,
                (c, v) => c.m_IncludeBackingField = v
            );

            public static readonly ValueClassProperty<PropertyNode, bool> IncludeAccessor = new ValueClassProperty<PropertyNode, bool>(
                "IncludeAccessor",
                c => c.m_IncludeAccessor,
                (c, v) => c.m_IncludeAccessor = v
            );
            
            public static readonly ValueClassProperty<PropertyNode, string> BackingField = new ValueClassProperty<PropertyNode, string>(
                "BackingField",
                c => c.m_BackingField,
                (c, v) => c.m_BackingField = v
            );
            
            public static readonly ValueClassProperty<PropertyNode, string> DefaultValue = new ValueClassProperty<PropertyNode, string>(
                "DefaultValue",
                c => c.m_DefaultValue,
                (c, v) => c.m_DefaultValue = v
            );
            
            public static readonly ClassPropertyBag<PropertyNode> PropertyBag = new ClassPropertyBag<PropertyNode>(
                Name,
                ValueType,
                PropertyType,
                Documentation,
                IncludeDeclaration,
                IncludeInitializer,
                IncludeBackingField,
                IncludeAccessor,
                BackingField,
                DefaultValue
            );
        }

        public IPropertyBag PropertyBag => Property.PropertyBag;
        public IVersionStorage VersionStorage => null;

        private string m_Name;
        private string m_ValueType;
        private PropertyType m_PropertyType = PropertyType.Value;
        private string m_Documentation;
        private bool m_IncludeDeclaration = true;
        private bool m_IncludeInitializer = true;
        private bool m_IncludeBackingField = true;
        private bool m_IncludeAccessor = true;
        private string m_BackingField;
        private string m_DefaultValue;

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name
        {
            get { return Property.Name.GetValue(this); }
            set { Property.Name.SetValue(this, value); }
        }

        /// <summary>
        /// The value type for this property (e.g. int, float, string)
        /// 
        /// @NOTE When dealing with collections this is the item type
        /// </summary>
        public string ValueType
        {
            get { return Property.ValueType.GetValue(this); }
            set { Property.ValueType.SetValue(this, value); }
        }

        /// <summary>
        /// The property type <see cref="PropertyType"/>
        ///
        /// defaults to `Value`
        /// </summary>
        public PropertyType PropertyType
        {
            get { return Property.PropertyType.GetValue(this); }
            set { Property.PropertyType.SetValue(this, value); }
        }
        
        /// <summary>
        /// Documentation for this property.
        /// </summary>
        public string Documentation
        {
            get { return Property.Documentation.GetValue(this); }
            set { Property.Documentation.SetValue(this, value); }
        }
        
        /// <summary>
        /// Should the declaration be generated for this property
        ///
        /// defaults to `true`
        /// </summary>
        public bool IncludeDeclaration
        {
            get { return Property.IncludeDeclaration.GetValue(this); }
            set { Property.IncludeDeclaration.SetValue(this, value); }
        }
        
        /// <summary>
        /// Should the initializer be generated for this property; (initializers are added to the `InitializeProperties` method)
        ///
        /// defaults to `true`
        /// </summary>
        public bool IncludeInitializer
        {
            get { return Property.IncludeInitializer.GetValue(this); }
            set { Property.IncludeInitializer.SetValue(this, value); }
        }
        
        /// <summary>
        /// Should the backing field be generated for this property
        ///
        /// defaults to `true`
        /// </summary>
        public bool IncludeBackingField
        {
            get { return Property.IncludeBackingField.GetValue(this); }
            set { Property.IncludeBackingField.SetValue(this, value); }
        }

        /// <summary>
        /// Should the .NET property accessors be generated for this property
        ///
        /// defaults to `true`
        /// </summary>
        public bool IncludeAccessor
        {
            get { return Property.IncludeAccessor.GetValue(this); }
            set { Property.IncludeAccessor.SetValue(this, value); }
        }

        /// <summary>
        /// Backing field name;
        ///
        /// If none is provided `m_{Name}` will be used
        /// </summary>
        public string BackingField
        {
            get { return Property.BackingField.GetValue(this); }
            set { Property.BackingField.SetValue(this, value); }
        }
        
        /// <summary>
        /// Default value for the backing field
        ///
        /// @NOTE This is assigned in the field initializer
        /// </summary>
        public string DefaultValue
        {
            get { return Property.DefaultValue.GetValue(this); }
            set { Property.DefaultValue.SetValue(this, value); }
        }

        public bool IsList => m_PropertyType == PropertyType.ValueList || m_PropertyType == PropertyType.ClassList || m_PropertyType == PropertyType.StructList;
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)