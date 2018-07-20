using System;
using UnityEngine;

namespace SavePort.Types {

    [Serializable]
    [CreateAssetMenu(fileName = "New String Container", menuName = "Container Assets/String")]
    public class StringContainer : BaseDataContainer<string> {
        public IntReference maxLength;

        public override string Validate(string input) {
            try {
                if (input.Length <= maxLength) {
                    return input;
                } else {
                    return input.Substring(0, maxLength);
                }
            }
            catch (Exception) {
                Debug.LogError("Validation failed because the int reference of maxLength is set to Container mode, but doesn't contain a reference to a container!");
            }

            return input;
        }
    }

    [Serializable]
    public class StringReference : BaseDataReference<string, StringContainer> { }

}
