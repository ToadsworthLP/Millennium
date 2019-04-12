#if (NET_4_6 || NET_STANDARD_2_0)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.Properties.Codegen.CSharp
{
    public class CSharpSchemaBuilder : ISchemaBuilder
    {
        private const string KInitializeCustomPropertiesMethodName = "InitializeCustomProperties";
        private const string KPropertyBagName = "s_PropertyBag";
        
        public void Build(string path, Schema schema)
        {
            var writer = new CodeWriter(CodeStyle.CSharp);

            writer.Line("#if (NET_4_6 || NET_STANDARD_2_0)");

            var usings = new List<string>
            {
                "System",
                "System.Collections.Generic",
                "Unity.Properties"
            };

            usings.AddRange(schema.UsingAssemblies);

            foreach (var @using in usings.OrderBy(s => s))
            {
                writer.Line($"using {@using};");
            }

            writer.Line();

            foreach (var type in schema.Types)
            {
                IScope namespaceScope = null;
                
                if (!string.IsNullOrEmpty(type.Namespace))
                {
                    namespaceScope = writer.Scope($"namespace {type.Namespace}");
                }
                
                WriteType(writer, type);
                
                namespaceScope?.Dispose();;
            }
            
            writer.Line("#endif // (NET_4_6 || NET_STANDARD_2_0)");

            System.IO.File.WriteAllText(path, writer.ToString());
        }

        private static void WriteType(CodeWriter writer, TypeNode type)
        {
            using (writer.Scope(GetTypeDeclaration(type)))
            {
                WritePropertyDeclarations(writer, type);
                WritePropertyBagDeclaration(writer, type);

                if (type.IncludePropertyBagAccessor)
                {
                    WritePropertyBagAccessor(writer, type);
                }
                
                if (type.IncludeVersionStorageAccessor)
                {
                    WriteVersionStorageAccessor(writer, type);
                }

                if (type.IncludePropertyBagAccessor || type.IncludeVersionStorageAccessor)
                {
                    writer.Line();
                }
                
                WriteInitializePropertiesMethod(writer, type);
                WriteInitializeCustomPropertiesMethod(writer, type);
                WriteInitializePropertyBagMethod(writer, type);

                if (type.IncludeStaticConstructor)
                {
                    WriteStaticConstructor(writer, type);
                }
                
                WriteBackingFields(writer, type);
                WritePropertyAccessors(writer, type);

                if (type.IsStruct)
                {
                    writer.Line();
                    WriteMakeRefMethod(writer, type);
                }

                foreach (var nested in type.NestedTypes)
                {
                    writer.Line();
                    WriteType(writer, nested);
                }

                writer.Length -= 1;
            }
        }

        private static void WritePropertyDeclarations(CodeWriter writer, TypeNode type)
        {
            if (!type.Properties.Any(p => p.IncludeDeclaration))
            {
                return;
            }

            foreach (var property in type.Properties)
            {
                if (!property.IncludeDeclaration)
                {
                    continue;
                }
                
                var propertyTypeName = GetPropertyTypeName(type, property);
                var propertyName = GetPropertyName(property);

                if (false == string.IsNullOrEmpty(property.Documentation))
                {
                    writer.Line("/// <summary>");
                    writer.Line($"/// <see cref=\"{type.Name}.{property.Name}\" /> property.");
                    writer.Line("/// </summary>");
                }
                writer.Line($"public static {propertyTypeName}<{type.Name}, {property.ValueType}> {propertyName} {{ get; private set; }}");
            }

            writer.Line();
        }
        
        private static void WritePropertyBagDeclaration(CodeWriter writer, TypeNode type)
        {
            var propertyBagType = GetPropertyBagTypeName(type);
            writer.Line($"private static {propertyBagType} {KPropertyBagName} {{ get; set; }}");
            writer.Line();
        }
        
        private static void WriteVersionStorageAccessor(CodeWriter writer, TypeNode type)
        {
            writer.Line("/// <inheritdoc cref=\"Unity.Properties.IPropertyContainer.VersionStorage\" />");
            writer.Line("public IVersionStorage VersionStorage => null;");
        }
        
        private static void WritePropertyBagAccessor(CodeWriter writer, TypeNode type)
        {
            writer.Line("/// <inheritdoc cref=\"Unity.Properties.IPropertyContainer.PropertyBag\" />");
            writer.Line($"public IPropertyBag PropertyBag => {KPropertyBagName};");
        }

        private static void WriteInitializePropertiesMethod(CodeWriter writer, TypeNode containerType)
        {
            using (writer.Scope("private static void InitializeProperties()"))
            {
                if (containerType.Properties.Any(p => p.IncludeInitializer))
                {
                    foreach (var property in containerType.Properties)
                    {
                        if (!property.IncludeInitializer)
                        {
                            continue;
                        }
                        
                        var propertyTypeName = GetPropertyTypeName(containerType, property);
                        var propertyName = GetPropertyName(property);
                        var backingFieldName = GetBackingFieldName(property);

                        writer.Line($"{propertyName} = new {propertyTypeName}<{containerType.Name}, {property.ValueType}>(");
                        writer.IncrementIndent();

                        writer.Line($"\"{property.Name}\"");

                        if (containerType.IsStruct)
                        {
                            if (property.IsList)
                            {
                                writer.Line($",(ref {containerType.Name} c) => c.{backingFieldName}"); // GetValue
                                if (property.PropertyType == PropertyType.ClassList)
                                {
                                    // CreateInstance: assumes that class-typed containers have default constructors
                                    writer.Line($",(ref {containerType.Name} c) => new {property.ValueType}()");
                                }
                            }
                            else
                            {
                                writer.Line($",(ref {containerType.Name} c) => c.{backingFieldName}"); // GetValue
                                writer.Line($",(ref {containerType.Name} c, {property.ValueType} v) => c.{backingFieldName} = v"); // SetValue
                                
                                if (property.PropertyType == PropertyType.StructValue)
                                {
                                    writer.Line($",(StructValueStructProperty<{containerType.Name},{property.ValueType}>.ByRef m, StructValueStructProperty<{containerType.Name},{property.ValueType}> p, ref {containerType.Name} c, IPropertyVisitor v) => m(p, ref c, ref c.{backingFieldName}, v)"); // GetValueRef
                                }
                            }
                        }
                        else
                        {
                            if (property.IsList)
                            {
                                writer.Line($",c => c.{backingFieldName}"); // GetValue
                                if (property.PropertyType == PropertyType.ClassList)
                                {
                                    // CreateInstance: assumes that class-typed containers have default constructors
                                    writer.Line($",c => new {property.ValueType}()");
                                }
                            }
                            else
                            {
                                writer.Line($",c => c.{backingFieldName}"); // GetValue
                                writer.Line($",(c, v) => c.{backingFieldName} = v"); // SetValue
                                
                                if (property.PropertyType == PropertyType.StructValue)
                                {
                                    writer.Line($",(m, p, c, v) => m(p, c, ref c.{backingFieldName}, v)"); // GetValueRef
                                }
                            }
                        }

                        writer.DecrementIndent();
                        writer.Line($");");
                        writer.Line();
                    }

                    writer.Length -= 1;
                }
            }

            writer.Line();
        }
        
        private static void WriteInitializeCustomPropertiesMethod(CodeWriter writer, TypeNode type)
        {
            writer.Line($"static partial void {KInitializeCustomPropertiesMethodName}();");
            writer.Line();
        }

        private static void WriteInitializePropertyBagMethod(CodeWriter writer, TypeNode type)
        {
            using (writer.Scope("private static void InitializePropertyBag()"))
            {
                writer.Line($"{KPropertyBagName} = new {GetPropertyBagTypeName(type)}(");

                if (type.Properties.Count == 0)
                {
                    writer.Length -= 1;
                    writer.Write(");").Line();
                }
                else
                {
                    writer.IncrementIndent();

                    foreach (var property in type.Properties)
                    {
                        writer.Line($"{GetPropertyName(property)},");
                    }

                    writer.Length -= 2;
                    writer.Line();

                    writer.DecrementIndent();
                    writer.Line(");");
                }
            }

            writer.Line();
        }

        private static void WriteStaticConstructor(CodeWriter writer, TypeNode type)
        {
            using (writer.Scope($"static {type.Name}()"))
            {
                writer.Line("InitializeProperties();");
                writer.Line($"{KInitializeCustomPropertiesMethodName}();");
                writer.Line("InitializePropertyBag();");
            }

            writer.Line();
        }

        private static void WriteBackingFields(CodeWriter writer, TypeNode type)
        {
            if (!type.Properties.Any(p => p.IncludeBackingField))
            {
                return;
            }

            foreach (var property in type.Properties)
            {
                if (!property.IncludeBackingField)
                {
                    continue;;
                }
                
                var backingFieldName = GetBackingFieldName(property);

                if (property.IsList)
                {
                    // Always initialize lists
                    writer.Line($"private readonly List<{property.ValueType}> {backingFieldName} = new List<{property.ValueType}>();");
                }
                else
                {
                    var fieldInitializer = string.Empty;
                    
                    if (!string.IsNullOrEmpty(property.DefaultValue))
                    {
                        fieldInitializer = $" = {property.DefaultValue}";
                    }
                    
                    writer.Line($"private {property.ValueType} {backingFieldName}{fieldInitializer};");
                }
            }

            writer.Line();
        }

        private static void WritePropertyAccessors(CodeWriter writer, TypeNode type)
        {
            foreach (var property in type.Properties)
            {
                if (!property.IncludeAccessor)
                {
                    continue;
                }

                if (false == string.IsNullOrEmpty(property.Documentation))
                {
                    writer.Line("/// <summary>");
                    writer.Line($"/// {property.Documentation}");
                    writer.Line("/// </summary>");
                }
                
                var propertyName = GetPropertyName(property);

                if (property.IsList)
                {
                    var propertyListType = $"PropertyList<{type.Name}, {property.ValueType}>";
                    writer.Line($"public {propertyListType} {property.Name} => new {propertyListType}({propertyName}, this);");
                    writer.Line();
                }
                else
                {
                    writer.Line($"public {property.ValueType} {property.Name}");
                    writer.Line("{");
                    writer.IncrementIndent();
                    
                    if (type.IsStruct)
                    {
                        writer.Line($"get {{ return {propertyName}.GetValue(ref this); }}");
                        writer.Line($"set {{ {propertyName}.SetValue(ref this, value); }}");
                    }
                    else
                    {
                        writer.Line($"get {{ return {propertyName}.GetValue(this); }}");
                        writer.Line($"set {{ {propertyName}.SetValue(this, value); }}");
                    }
                    
                    writer.DecrementIndent();
                    writer.Line("}");
                    writer.Line();
                }
            }
        }

        private static void WriteMakeRefMethod(CodeWriter writer, TypeNode type)
        {
            writer.Line("/// <summary>");
            writer.Line("/// Pass this object as a reference to the given handler.");
            writer.Line("/// </summary>");
            writer.Line("/// <param name=\"byRef\">Handler to invoke.</param>");
            writer.Line("/// <param name=\"context\">Context argument passed to the handler.</param>");
            using (writer.Scope($"public void MakeRef<TContext>(ByRef<{type.Name}, TContext> byRef, TContext context)"))
            {
                writer.Line("byRef(ref this, context);");
            }

            writer.Line();
            
            writer.Line("/// <summary>");
            writer.Line("/// Pass this object as a reference to the given handler, and return the result.");
            writer.Line("/// </summary>");
            writer.Line("/// <param name=\"byRef\">Handler to invoke.</param>");
            writer.Line("/// <param name=\"context\">Context argument passed to the handler.</param>");
            writer.Line("/// <returns>The handler's return value.</returns>");
            using (writer.Scope($"public TReturn MakeRef<TContext, TReturn>(ByRef<{type.Name}, TContext, TReturn> byRef, TContext context)"))
            {
                writer.Line("return byRef(ref this, context);");
            }

            writer.Line();
        }

        private static string GetTypeDeclaration(TypeNode type)
        {
            var str = new StringBuilder();

            switch (type.AccessModifier)
            {
                case AccessModifier.Public:
                    str.Append("public ");
                    break;
                case AccessModifier.Private:
                    str.Append("private ");
                    break;
                case AccessModifier.Internal:
                    str.Append("internal ");
                    break;
            }

            if (type.IsAbstract)
            {
                str.Append("abstract ");
            }

            str.Append("partial ");
            str.Append(type.IsStruct ? "struct " : "class ");
            str.Append($"{type.Name} ");

            if (type.IsStruct)
            {
                str.Append($": {typeof(IStructPropertyContainer).Name}<{type.Name}>");
            }
            else
            {
                str.Append($": {typeof(IPropertyContainer).Name}");
            }
            
            return str.ToString();
        }

        private static string GetPropertyTypeName(TypeNode type, PropertyNode property)
        {
            var str = new StringBuilder();

            switch (property.PropertyType)
            {
                case PropertyType.Value:
                    str.Append("Value");
                    break;
                case PropertyType.ClassValue:
                    str.Append("ClassValue");
                    break;
                case PropertyType.StructValue:
                    str.Append("StructValue");
                    break;
                case PropertyType.ValueList:
                    str.Append("ValueList");
                    break;
                case PropertyType.ClassList:
                    str.Append("ClassList");
                    break;
                case PropertyType.StructList:
                    str.Append("StructList");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            str.Append(type.IsStruct ? "Struct" : "Class");
            str.Append("Property");

            return str.ToString();
        }

        private static string GetPropertyBagTypeName(TypeNode type)
        {
            return type.IsStruct ? $"StructPropertyBag<{type.Name}>" : $"ClassPropertyBag<{type.Name}>";
        }

        private static string GetPropertyName(PropertyNode property)
        {
            return $"{property.Name}Property";
        }

        private static string GetBackingFieldName(PropertyNode property)
        {
            return string.IsNullOrEmpty(property.BackingField) ? $"m_{property.Name}" : property.BackingField;
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)