using System;
using UnityEngine;

namespace SavePort.Types {

    [Serializable]
    [CreateAssetMenu(fileName = "New Int Container", menuName = "Container Assets/Integer")]
    public class IntContainer : BaseDataContainer<int> {
        public IntReference minValue;
        public IntReference maxValue;

        public override int Validate(int input) {
            try {
                if (input <= maxValue && input >= minValue) {
                    return input;
                } else if (input > maxValue) {
                    return maxValue;
                } else if (input < minValue) {
                    return minValue;
                }

                return input;
            }
            catch (Exception) {
                Debug.LogError("Validation failed because the int reference of min or max value are set to Container mode, but don't contain a reference to a container!");
            }

            return input;
        }
    }

    [Serializable]
    public class IntReference : BaseDataReference<int, IntContainer> { }

}
