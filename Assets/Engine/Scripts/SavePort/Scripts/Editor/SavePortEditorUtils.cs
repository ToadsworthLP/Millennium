using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SavePort.EditorOnly {
    public class SavePortEditorUtils {

        public static List<T> FindAllAssetsOfType<T>() where T : Object {
            List<T> assets = new List<T>();
            string[] ids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));

            for (int i = 0; i < ids.Length; i++) {
                string assetPath = AssetDatabase.GUIDToAssetPath(ids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) {
                    assets.Add(asset);
                }
            }

            return assets;
        }

    } 
}
