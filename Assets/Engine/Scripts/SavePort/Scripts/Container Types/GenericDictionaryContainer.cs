using System.Collections.Generic;

namespace SavePort.Types {
    public class GenericDictionaryContainer<TKey, TValue> : BaseDataContainer<Dictionary<TKey, TValue>> {
        public override Dictionary<TKey, TValue> Validate(Dictionary<TKey, TValue> input) {
            return input;
        }
    }
}
