//using OdinSerializer;
//using System;
//using System.Collections.Generic;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
//using UnityEngine;

//namespace SavePort.Types {

//    [Serializable]
//    [CreateAssetMenu(fileName = "New Shelf Container", menuName = "Container Assets/Shelf")]
//    public class ShelfContainer : BaseDataContainer<Dictionary<string, object>>, ISerializationCallbackReceiver {

//        private void OnEnable() {
//            serializedValue = new Dictionary<string, object>();
//#if UNITY_EDITOR
//            EditorUtility.SetDirty(this);
//#endif
//        }

//        public override Dictionary<string, object> Validate(Dictionary<string, object> input) {
//            if (input == null) return input = new Dictionary<string, object>();

//            return input;
//        }

//        public T GetShelfData<T>(string key, T defaultValue) {
//            object data;
//            if (Value.TryGetValue(key, out data)) {
//                try {
//                    return (T)data;
//                }
//                catch (Exception e) {
//                    Debug.LogError("Failed to cast entry " + key + " to requested type: " + e.Message + e.StackTrace);
//                }
//            };
//            return defaultValue;
//        }

//        public void SetShelfData(string key, object value) {
//            if (Value.ContainsKey(key)) {
//                Value[key] = value;
//            } else {
//                Value.Add(key, value);
//            }
//        }

//    }

//    [Serializable]
//    public class ShelfReference : BaseDataReference<Dictionary<string, object>, ShelfContainer> { }

//}