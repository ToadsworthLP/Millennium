#if (NET_4_6 || NET_STANDARD_2_0)

using System.IO;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace Unity.Properties.Codegen
{
    [ScriptedImporter(3, new[] {".properties"})]
    public class SchemaImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // Generate a dummy object to satisfy the asset pipeline
            var asset = ScriptableObject.CreateInstance<SchemaObject>();
            ctx.AddObjectToAsset("asset", asset);

            asset.JsonSchema = File.ReadAllText(ctx.assetPath);

            // Asset importer expects ONE and ONLY ONE call to `SetMainObject`
            ctx.SetMainObject(asset);
        }
    }
}

#endif // (NET_4_6 || NET_STANDARD_2_0)