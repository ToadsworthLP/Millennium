using UnityEngine;

public class GearPage : MonoBehaviour {

    public ItemListManager itemListManager;
    private Backpack backpack;

    void Awake() {
        backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
    }

    void OnEnable() {
        itemListManager.SetBackpack(backpack);
        itemListManager.ShowItems(ItemListManager.EnumItemKind.NORMAL);
    }

}
