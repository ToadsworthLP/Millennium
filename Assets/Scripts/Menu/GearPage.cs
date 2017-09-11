using UnityEngine;

public class GearPage : MonoBehaviour {

    public ItemListManager itemListManager;

    void OnEnable() {
        Backpack backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
        itemListManager.showItems(ItemListManager.EnumItemKind.NORMAL, backpack);
    }

}
