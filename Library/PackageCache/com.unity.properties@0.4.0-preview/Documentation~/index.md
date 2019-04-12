# Unity Properties

Unity Properties are a set of interfaces and runtime implementation that offer a very fexible and efficient framework for data container description and visitation services.

# About the Property API

Use the Property API package to include the `Unity.Properties` namespace in your project. This namespace contains interfaces and classes you can use to:
 * describe or wrap .Net types as property containers
 * resolve properties in nested property containers
 * implement visitors to traverse property containers and perform tasks (e.g. schema generation, serialization, UI generation, etc.)

The Property API differs from the available .Net RTTI in multiple aspects:
 * it was designed with performance in mind
 * it supports dynamic type trees - you can create properties at runtime
 * it can be extended for application-specific use cases

# Installing the Property API

To install this package, follow the instructions in the [Package Manager documentation](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html).

# Code Generation

## Property Schema

You can use the `Unity.Properties.Codegen.Schema` API to build the representation of a property container type. This `Schema` object implements the `IPropertyContaner` interface, and can be serialized using the Property API.

By serializing it to JSON and saving it to a file ending with the `.properties` extension, you can use the `Unity.Properties.Codegen.SchemaImporter` to inspect the schema and generate C# code in the Unity Editor on demand.

Here's a simple example:

```csharp
using Unity.Properties.Codegen;
using Unity.Properties.Codegen.CSharp;
using Unity.Properties.Serialization;
using UnityEditor;

namespace Sample
{
    internal static class SchemaCodeGeneration
    {
        [MenuItem("Properties/Generate")]
        static void Generate()
        {
            var schema = new Schema();
            var myClass = new TypeNode()
            {
                Name = "MyClass",
                Namespace = "Sample"
            };
            myClass.Properties.Add(new PropertyNode()
            {
                Name = "IntValue",
                PropertyType = PropertyType.Value,
                ValueType = "int",
                Documentation = "This is an int value"
            });
            schema.Types.Add(myClass);

            // Optional:
            // save the schema using the `.properties` extension
            // useful if you don't want to programmatically generate the schema object
            File.WriteAllText("Assets/MyClass.properties", JsonSerializer.Serialize(schema));

            // generate the C# code
            // you can do the same thing by pressing the "Generate" button on the properties asset inspector
            var builder = new CSharpSchemaBuilder();
            builder.Build("Assets/MyClass.Properties.cs", schema);

            AssetDatabase.Refresh();
        }
    }
}
```

The serialized shema object looks like this:
```json
{
    "Types": [{
        "Namespace": "Sample",
        "Name": "MyClass",
        "Properties": [{
            "Name": "IntValue",
            "ValueType": "int",
            "PropertyType": "Value",
            "Documentation": "This is an int value"
        }]
    }]
}
```

And the generated C# code looks like this. By default, the generated classes or structs are `public` and `partial`. This allows you to define your custom logic in a separate C# file.

There are many options to control the C# code generation output. See the `Unity.Properties.Codegen.Schema` API documentation for details.

```csharp
#if (NET_4_6 || NET_STANDARD_2_0)
using System;
using System.Collections.Generic;
using Unity.Properties;

namespace Sample
{
    public partial class MyClass : IPropertyContainer
    {
        /// <summary>
        /// <see cref="MyClass.IntValue" /> property.
        /// </summary>
        public static ValueClassProperty<MyClass, int> IntValueProperty { get; private set; }

        private static ClassPropertyBag<MyClass> s_PropertyBag { get; set; }

        /// <inheritdoc cref="Unity.Properties.IPropertyContainer.PropertyBag" />
        public IPropertyBag PropertyBag => s_PropertyBag;
        /// <inheritdoc cref="Unity.Properties.IPropertyContainer.VersionStorage" />
        public IVersionStorage VersionStorage => null;

        private static void InitializeProperties()
        {
            IntValueProperty = new ValueClassProperty<MyClass, int>(
                "IntValue"
                ,c => c.m_IntValue
                ,(c, v) => c.m_IntValue = v
            );
        }

        static partial void InitializeCustomProperties();

        private static void InitializePropertyBag()
        {
            s_PropertyBag = new ClassPropertyBag<MyClass>(
                IntValueProperty
            );
        }

        static MyClass()
        {
            InitializeProperties();
            InitializeCustomProperties();
            InitializePropertyBag();
        }

        private int m_IntValue;

        /// <summary>
        /// This is an int value
        /// </summary>
        public int IntValue
        {
            get { return IntValueProperty.GetValue(this); }
            set { IntValueProperty.SetValue(this, value); }
        }
    }
}
#endif // (NET_4_6 || NET_STANDARD_2_0)
```

# Creating Property Containers

To work with Properties, you need to instrument your .Net types with the `Unity.Properties.IPropertyContainer` interface.

This interface declares a `Unity.Properties.IPropertyBag`, essentially exposing a list of properties for this particular type to consumers of the API.

This manual instrumentation can be tedious (and error-prone!). Refer to the [Code Generation](#CodeGeneration) section for alternative code instrumentation methods.

## Class Containers

Property types available to `class` containers all have the `ClassProperty` suffix.

```csharp
// Class container example instrumented with static properties
class MyClass : IPropertyContainer
{
    // Property declarations
    // properties themselves can have any access modifier (public, internal, protected, private).
    // They'll be available as long as they're added to the type's property bag (see below).

    // Value property types
    public static ValueClassProperty<MyClass, int> SimpleValueProperty { get; private set; }
    public static ClassValueClassProperty<MyClass, MyOtherClass> ClassContainerProperty { get; private set; }
    public static StructValueClassProperty<MyClass, MyOtherStruct> StructContainerProperty { get; private set; }

    // List property types
    public static ValueListClassProperty<MyClass, int> SimpleValueListProperty { get; private set; }
    public static ClassListClassProperty<MyClass, MyOtherClass> ListOfClassContainersProperty { get; private set; }
    public static StructListClassProperty<MyClass, MyOtherStruct> ListOfStructContainersProperty { get; private set; }

    // Backing Storage
    // here we're using .Net fields hosted on the instance itself, but the data can come from anywhere
    // For example, the data could come from a native (unmanaged) data store, or could live on another .Net object.

    private int m_SimpleValue;
    private MyOtherClass m_ClassContainer;
    private MyOtherStruct m_StructContainer;
    private readonly List<int> m_SimpleValueList = new List<int>();
    private readonly List<MyOtherClass> m_ListOfClassContainers = new List<MyOtherClass>();
    private readonly List<MyOtherStruct> m_ListOfStructContainers = new List<MyOtherStruct>();

    // Setup

    private static void InitializeProperties()
    {
        // All property types use delegates to access backing storage while preserving encapsulation
        // The delegates here are anonymous and static, and will not allocate.

        // You can put anything in a Value property, as long as the algorithms support the value type.
        // Algorithms built on top of Properties are encouraged to support all .Net primitive types, including
        // enum types.

        SimpleValueProperty = new ValueClassProperty<MyClass, int>(
            "SimpleValue"
            , c => c.m_SimpleValue                 // read instance data (get)
            , (c, v) => c.m_SimpleValue = v        // write instance data (set)
        );

        // Use ClassContainer or StructContainer properties to recurse into complex types while visiting
        // Note that ClassContainerProperty does not handle cyclic references automatically.

        ClassContainerProperty = new ClassValueClassProperty<MyClass, MyOtherClass>(
            "ClassContainer"
            , c => c.m_ClassContainer
            , (c, v) => c.m_ClassContainer = v
        );

        StructContainerProperty = new StructValueClassProperty<MyClass, MyOtherStruct>(
            "StructContainer"
            , c => c.m_StructContainer
            , (c, v) => c.m_StructContainer = v
        );

        // List properties can be used to represent ordered collections of items
        // Lists must be backed by a type that implements the `System.Collections.Generic.IList<TValue>` interface.

        SimpleValueListProperty = new ValueListClassProperty<MyClass, int>(
            "SimpleValueList"
            , c => c.m_SimpleValueList            // get list
        );

        // The 2nd delegate is optional, and defaults to use `default(TValue)` when adding a new item.
        // You usually want to provide an item creation delegate when working with `class` value, otherwise new
        // items will default to `null`.

        ListOfClassContainersProperty = new ClassListClassProperty<MyClass, MyOtherClass>(
            "ListOfClassContainers"
            , c => c.m_ListOfClassContainers
            , c => new MyOtherClass()             // create list item
        );

        ListOfStructContainersProperty = new StructListClassProperty<MyClass, MyOtherStruct>(
            "ListOfStructContainers"
            , c => c.m_ListOfStructContainers
        );
    }

    private static ClassPropertyBag<ScriptMetadata> s_PropertyBag { get; set; }

    private static void InitializePropertyBag()
    {
        // A property bag is simply an ordered list of properties

        s_PropertyBag = new ClassPropertyBag<MyClass>(
            SimpleValueProperty
            , ClassContainerProperty
            , StructContainerProperty
            , SimpleValueListProperty
            , ListOfClassContainersProperty
            , ListOfStructContainersProperty
        );
    }

    static MyClass()
    {
        InitializeProperties();
        InitializePropertyBag();
    }

    // IPropertyContainer interface implementation

    public IPropertyBag PropertyBag => s_PropertyBag;
    public IVersionStorage VersionStorage => null;
}

class MyOtherClass : IPropertyContainer {  }
struct MyOtherStruct : IStructPropertyContainer<MyOtherStruct> {  }
```

## Struct Containers

Property types available to `struct` containers all end with the `StructProperty` suffix.

Struct containers require a bit more code to instrument, since they must be passed by reference in order to support data mutation during visits while preserving performance (no boxing, less copies).

`struct` containers should therefore use the extended `IStructPropertyContainer<T>` interface.

```csharp
public struct MyStruct : IStructPropertyContainer<MyStruct>
{
    public static ValueStructProperty<MyStruct, string> NameProperty { get; private set; }

    private static StructPropertyBag<MyStruct> s_PropertyBag { get; set; }

    public IPropertyBag PropertyBag => s_PropertyBag;
    public IVersionStorage VersionStorage => null;

    private static void InitializeProperties()
    {
    	// note that the delegates now get the container by reference

        NameProperty = new ValueStructProperty<MyStruct, string>(
            "Name"
            ,(ref MyStruct c) => c.m_Name
            ,(ref MyStruct c, string v) => c.m_Name = v
        );
    }

    private static void InitializePropertyBag()
    {
        s_PropertyBag = new StructPropertyBag<MyStruct>(
            NameProperty
        );
    }

    static MyStruct()
    {
        InitializeProperties();
        InitializePropertyBag();
    }

    private string m_Name;

    public string Name
    {
        get { return NameProperty.GetValue(ref this); }
        set { NameProperty.SetValue(ref this, value); }
    }

    // IStructPropertyContainer implementation

    public void MakeRef<TContext>(ByRef<Reference, TContext> byRef, TContext context)
    {
        byRef(ref this, context);
    }

    public TReturn MakeRef<TContext, TReturn>(ByRef<Reference, TContext, TReturn> byRef, TContext context)
    {
        return byRef(ref this, context);
    }
}
```

# Document revision history

|Date|Reason|
|---|---|
|Nov 12, 2018|Removed Roslyn-base code generation path.|
|Oct 27, 2018|Added documentation around instrumentation and code generation.|
|Feb 14, 2018|Document created. Matches package version 0.1.0|
