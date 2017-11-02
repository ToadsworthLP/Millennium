using UnityEngine;

public class ItemManager : ItemCoroutines {

    public void useItem(GameMode mode, InventoryItem item){
        Backpack backpack = getBackpack();

        if(item.callCoroutineOnUse){
            StartCoroutine(item.coroutineName, new ItemParameter(mode, item.coroutineArgs));
        }

        if(item.statModifiers.Length > 0){
            foreach (StatModifier mod in item.statModifiers) {
                switch(mod.statToModify){
                    case StatType.HP:
                        backpack.hp += mod.value;
                        break;

                    case StatType.FP:
                        backpack.fp += mod.value;
                        break;

                    case StatType.SP:
                        backpack.sp += mod.value;
                        break;
                }
            }
        }

        backpack.items.Remove(item);
    }
}
