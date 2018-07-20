using System;
using UnityEngine;

namespace SavePort.Types {

    [Serializable]
    [CreateAssetMenu(fileName = "New Vector3 Container", menuName = "Container Assets/Vector3")]
    public class Vector3Container : BaseDataContainer<Vector3> {
        public override Vector3 Validate(Vector3 input) {
            return input; //TODO implement proper validation for this type
        }
    }

    [Serializable]
    public class Vector3Reference : BaseDataReference<Vector3, Vector3Container> { }

}