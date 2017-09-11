using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListManager : MonoBehaviour {

    public GameObject entryParent;
    public GameObject itemEntryPrefab;
    public Text descriptionBox;

    public void showItems(EnumItemKind category, Backpack backpack){
        entryParent.transform.ClearChildren();

        List<InventoryItem> items = new List<InventoryItem>();
        if(category == EnumItemKind.IMPORTANT){
            items = backpack.items.FindAll(isImportant);
        }else{
            items = backpack.items.FindAll(isNormal);
        }

        int counter = 0;
        foreach (InventoryItem item in items) {
            GameObject entryObj = Instantiate(itemEntryPrefab, entryParent.transform);
            ItemEntry entry = entryObj.GetComponent<ItemEntry>();

            if(counter % 2 == 0){
                entryObj.GetComponent<RectTransform>().localPosition += new Vector3(0, -50 * counter / 2, 0) ;
            }else{
                entryObj.GetComponent<RectTransform>().localPosition += new Vector3(235, -50 * (counter - 1) / 2, 0);
            }
            entry.item = item;
            entry.setupEntry(descriptionBox);
            counter++;
        }
    }

    //Come on don't be so lazy!
    private static bool isImportant(InventoryItem item){
        if(item.isImportantItem){
            return true;
        }else{
            return false;
        }
    }

    private static bool isNormal(InventoryItem item) {
        if (item.isImportantItem) {
            return false;
        } else {
            return true;
        }
    }

    public enum EnumItemKind
    {
        NORMAL, IMPORTANT
    }
}