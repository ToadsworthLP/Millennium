using UnityEngine;

public class GearPage : MonoBehaviour {

    public ItemListManager itemListManager;

    void OnEnable() {
        Backpack backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
        itemListManager.setBackpack(backpack);
        itemListManager.showItems(ItemListManager.EnumItemKind.NORMAL);
    }

}
