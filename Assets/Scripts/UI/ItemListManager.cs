using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListManager : MonoBehaviour {

    public GameObject entryParent;
    public GameObject itemEntryPrefab;
    public Cursor itemCursor;
    public Cursor listSwitchCursor;
    public Text descriptionBox;
    public MenuPageOption pageOption;

    [HideInInspector]
    public Backpack backpack;

    public void setBackpack(Backpack backpack){
        this.backpack = backpack;
    }

    public void showItems(EnumItemKind category){
        entryParent.transform.ClearChildren();
        itemCursor.optionObjects.Clear();

        List<InventoryItem> itemList = new List<InventoryItem>();
        if (category == EnumItemKind.IMPORTANT){
            itemList = backpack.importantItems;
        }else{
            itemList = backpack.normalItems;
        }

        if(itemList.Count > 0){
            int counter = 0;
            foreach (InventoryItem item in itemList) {
                GameObject entryObj = Instantiate(itemEntryPrefab, entryParent.transform);
                ItemEntry entry = entryObj.GetComponent<ItemEntry>();

                if (counter % 2 == 0){
                    entryObj.GetComponent<RectTransform>().localPosition += new Vector3(0, -50 * counter / 2, 0) ;
                }else{
                    entryObj.GetComponent<RectTransform>().localPosition += new Vector3(235, -50 * (counter - 1) / 2, 0);
                }
                entry.item = item;
                entry.listSwitchCursor = listSwitchCursor;
                itemCursor.optionObjects.Add(entryObj);
                entry.setupEntry(descriptionBox, pageOption);

                counter++;
            }
        }
    }    

    public enum EnumItemKind
    {
        NORMAL, IMPORTANT
    }
}