using SavePort;
using SavePort.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Millennium.Containers {

    [Serializable]
    [CreateAssetMenu(fileName = "New ItemList Container", menuName = "Container Assets/List/BaseItem")]
    public class ItemListContainer : GenericListContainer<BaseItem> { }

    [Serializable]
    public class ItemListReference : BaseDataReference<List<BaseItem>, ItemListContainer> { } 
}