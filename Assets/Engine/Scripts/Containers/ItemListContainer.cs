using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New BaseItem List Container", menuName = "Container Assets/BaseItem List")]
public class ItemListContainer : BaseVariableContainer<List<BaseItem>>
{
    public override List<BaseItem> Validate(List<BaseItem> input) {
        return input;
    }

    public void Add(BaseItem item){
        Value.Add(item);
    }

    public void Remove(BaseItem item){
        Value.Remove(item);
    }
}

[Serializable]
public class ItemListReference : BaseVariableReference<List<BaseItem>, ItemListContainer>{ }
