using SavePort.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Millennium.Containers {

    [Serializable]
    [CreateAssetMenu(fileName = "New Shelf Container", menuName = "Container Assets/Shelf")]
    public class ShelfContainer : GenericDictionaryContainer<string, object> {

        public void SetShelfData(string key, object value) {
            if(Value == null) { Value = new Dictionary<string, object>(); }

            Value[key] = value;
        }

        public T GetShelfData<T>(string key, T defaultValue) {
            if (Value == null) { Value = new Dictionary<string, object>(); }

            object value;
            Value.TryGetValue(key, out value);

            if(value == null) {
                return defaultValue;
            }

            return (T)value;
        }

    }

}
