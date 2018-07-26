using System.Collections.Generic;

namespace SavePort.Types {
    public class GenericListContainer<T> : BaseDataContainer<List<T>> {
        public IntReference maxCount;

        public override List<T> Validate(List<T> input) {
            if (input.Count > maxCount) {
                input.RemoveRange(maxCount, input.Count - maxCount);
            }

            return input;
        }
    } 
}