using System;
using UnityEngine;

namespace SavePort.Types {

    [Serializable]
    [CreateAssetMenu(fileName = "New HammerAsset Container", menuName = "Container Assets/HammerAsset")]
    public class HammerAssetContainer : BaseDataContainer<HammerAsset> {
        public override HammerAsset Validate(HammerAsset input) {
            return input; //implement proper validation for this type
        }
    }

    [Serializable]
    public class HammerAssetReference : BaseDataReference<HammerAsset, HammerAssetContainer> { }

}