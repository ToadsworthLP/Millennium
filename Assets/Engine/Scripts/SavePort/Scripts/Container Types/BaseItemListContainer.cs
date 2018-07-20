using System;
using System.Collections.Generic;
using UnityEngine;

namespace SavePort.Types {

    [Serializable]
    [CreateAssetMenu(fileName = "New BaseItem List Container", menuName = "Container Assets/List/BaseItem")]
    public class BaseItemListContainer : BaseDataContainer<List<BaseItem>> {

        public override List<BaseItem> Validate(List<BaseItem> input) {
            if (input == null) return input = new List<BaseItem>();

            return input;
        }
    }

    [Serializable]
    public class BaseItemListReference : BaseDataReference<List<BaseItem>, BaseItemListContainer> { }

}