using System;
using System.IO;
using Unity.Properties.Codegen.CSharp;
using Unity.Properties.Serialization;
using UnityEditor;
using UnityEngine;

namespace Unity.Properties.Codegen
{
    [CustomEditor(typeof(SchemaObject))]
    public class SchemaObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var wasEnabled = GUI.enabled;
            GUI.enabled = true;
            try
            {
                if (GUILayout.Button("Generate"))
                {
                    var schemaObject = (SchemaObject) target;
                    Generate(schemaObject.JsonSchema);
                }
            }
            finally
            {
                GUI.enabled = wasEnabled;
            }
        }
        
        private void Generate(string json)
        {
            // Deserialize to a generic object tree
            var obj = JsonSerializer.Deserialize(json);

            // Unpack the fully migrated object to the current schema version
            var schema = new Schema();
            PropertyContainer.Transfer(obj, schema);
            
            // Construct the destination path
            var assetPath = AssetDatabase.GetAssetPath(target);
            var directory = Path.GetDirectoryName(assetPath);
            var fileName = Path.GetFileNameWithoutExtension(assetPath);
            var path = Path.Combine(directory, $"{fileName}.Properties.cs");
            
            // Generate the code and write to the file
            var builder = new CSharpSchemaBuilder();
            builder.Build(path, schema);
            
            // Re-import to force recompilation
            AssetDatabase.ImportAsset(path);
        }
    }
}