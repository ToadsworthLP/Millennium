using System;
using UnityEngine;

namespace SavePort.Types {

    [Serializable]
    [CreateAssetMenu(fileName = "New Float Container", menuName = "Container Assets/Float")]
    public class FloatContainer : BaseDataContainer<float> {
        public FloatReference minValue;
        public FloatReference maxValue;

        public override float Validate(float input) {
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
            catch (NullReferenceException) {
                Debug.LogError("Validation failed because the float reference of min or max value are set to Container mode, but don't contain a reference to a container!");
            }

            return input;
        }
    }

    [Serializable]
    public class FloatReference : BaseDataReference<float, FloatContainer> { }

}
