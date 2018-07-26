using SavePort;
using System;
using UnityEngine;

namespace Millennium.Containers {

    [Serializable]
    [CreateAssetMenu(fileName = "New HammerAsset Container", menuName = "Container Assets/HammerAsset")]
    public class HammerAssetContainer : BaseDataContainer<HammerAsset> {
        public override HammerAsset Validate(HammerAsset input) {
            return input;
        }
    }

    [Serializable]
    public class HammerAssetReference : BaseDataReference<HammerAsset, HammerAssetContainer> { }
}