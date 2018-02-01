using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemListManager : MonoBehaviour {

    public GameObject entryParent;
    public GameObject itemEntryPrefab;
    public Cursor itemCursor;
    public Cursor listSwitchCursor;
    public TextMeshProUGUI descriptionBox;
    public MenuPageOption pageOption;
    public float animationLength;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 targetPosition;

    [HideInInspector]
    public Backpack backpack;
    [HideInInspector]
    public int pageCount;
    [HideInInspector]
    public int currentPage;

    private Vector2 currentAnimVelocity;

    void Update() {
        if (rectTransform.anchoredPosition != targetPosition) {
            rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition, targetPosition, ref currentAnimVelocity, animationLength, 10000, Time.deltaTime);
        }
    }

    public void SetBackpack(Backpack backpack){
        this.backpack = backpack;
    }

    public void ShowItems(EnumItemKind category){
        entryParent.transform.ClearChildren();
        itemCursor.optionObjects.Clear();
        rectTransform = entryParent.GetComponent<RectTransform>();

        if (startPosition == Vector2.zero)
            startPosition = rectTransform.anchoredPosition;
        ResetPosition();

        List<BaseItem> itemList = new List<BaseItem>();
        if (category == EnumItemKind.IMPORTANT){
            itemList = backpack.importantItems;
        }else{
            itemList = backpack.normalItems;
        }

        if(itemList.Count > 0){
            int counter = 0;
            pageCount = (itemList.Count + 9) / 10;
            foreach (BaseItem item in itemList) {
                GameObject entryObj = Instantiate(itemEntryPrefab, entryParent.transform);
                ItemEntry entry = entryObj.GetComponent<ItemEntry>();

                if (counter % 2 == 0){
                    entryObj.GetComponent<RectTransform>().localPosition += new Vector3(0, -50 * counter / 2, 0) ;
                }else{
                    entryObj.GetComponent<RectTransform>().localPosition += new Vector3(220, -50 * (counter - 1) / 2, 0);
                }
                entry.item = (UsableItem)item;
                entry.listSwitchCursor = listSwitchCursor;
                itemCursor.optionObjects.Add(entryObj);
                entry.SetupEntry(this);

                counter++;
            }
        }
    }

    public void ScrollItemList(int targetIndex) {
        if (GetPageOfIndex(targetIndex) - 1 > currentPage) {
            targetPosition = rectTransform.anchoredPosition + new Vector2(0, 250);
            currentPage++;
        } else if (GetPageOfIndex(targetIndex) - 1 < currentPage) {
            targetPosition = rectTransform.anchoredPosition - new Vector2(0, 250);
            currentPage--;
        }

    }

    public void ResetPosition(){
        targetPosition = startPosition;
        currentPage = 0;
    }

    public int GetPageOfIndex(int index) {
        return (index + 10) / 10;
    }

    public enum EnumItemKind
    {
        NORMAL, IMPORTANT
    }
}