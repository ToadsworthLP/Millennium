using UnityEngine;
using UnityEngine.UI;

public class ItemEntry : SelectableHelper {

    public InventoryItem item;
    [HideInInspector]
    public Cursor listSwitchCursor;

    private Image itemIcon;
    private Text itemName;
    private Text descriptionBox;
    private MenuPageOption pageOption;

    private Cursor itemCursor;

    public void setupEntry(Text descBox, MenuPageOption pageOption) {
        itemIcon = GetComponentInChildren<Image>();
        itemName = GetComponentInChildren<Text>();
        descriptionBox = descBox;
        this.pageOption = pageOption;

        itemIcon.sprite = item.icon;
        itemName.text = item.itemName;

        if(!item.usableOnOverworld && !item.isImportantItem){
            GetComponent<CanvasGroup>().alpha = 0.5f;
        }
    }

    public override void onCursorInit(Cursor cursor) {
        base.onCursorInit(cursor);
        itemCursor = cursor;
    }

    public override void onSideKeyPressed(Utils.EnumDirection direction) {
        base.onSideKeyPressed(direction);
        if(direction == Utils.EnumDirection.DOWN && itemCursor.selectedIndex+2 < itemCursor.optionObjects.Count){
            itemCursor.cursorMoved(2);
        }else if(direction == Utils.EnumDirection.UP && itemCursor.selectedIndex - 2 >= 0) {
            itemCursor.cursorMoved(-2);
        }
    }

    public override void onCursorSelect() {
        base.onCursorSelect();
        descriptionBox.text = item.itemDescription;
    }

    public override void onOKPressed() {
        base.onOKPressed();
        if(item.usableOnOverworld){
            FindObjectOfType<ItemManager>().useItem(GameMode.FIELD, item);
            onCancelPressed();
            pageOption.onCancelPressed();
        }
    }

    public override void onCancelPressed() {
        base.onCancelPressed();
        itemCursor.gameObject.SetActive(false);
        listSwitchCursor.setActivityStatus(true);
    }

}
