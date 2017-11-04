using UnityEngine;
using UnityEngine.UI;

public class ItemEntry : SelectableHelper {

    public UsableItem item;
    [HideInInspector]
    public Cursor listSwitchCursor;

    private ItemListManager manager;

    private Image itemIcon;
    private Text itemName;

    private GameObject entryParent;
    private Text descriptionBox;
    private MenuPageOption pageOption;

    private Cursor itemCursor;

    public void setupEntry(ItemListManager manager) {
        itemIcon = GetComponentInChildren<Image>();
        itemName = GetComponentInChildren<Text>();

        this.manager = manager;
        descriptionBox = manager.descriptionBox;
        entryParent = manager.entryParent;
        pageOption = manager.pageOption;

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

        switch (direction){
            case Utils.EnumDirection.UP:
                if (itemCursor.selectedIndex - 2 >= 0)
                    itemCursor.cursorMoved(-2);
                    manager.scrollItemList(itemCursor.selectedIndex);
                break;

            case Utils.EnumDirection.DOWN:
                if (itemCursor.selectedIndex + 2 < itemCursor.optionObjects.Count)
                    itemCursor.cursorMoved(2);
                    manager.scrollItemList(itemCursor.selectedIndex);
                break;

            case Utils.EnumDirection.LEFT:
                manager.scrollItemList(itemCursor.selectedIndex);
                break;

            case Utils.EnumDirection.RIGHT:
                manager.scrollItemList(itemCursor.selectedIndex);
                break;
        }
    }

    public override void onCursorSelect() {
        base.onCursorSelect();
        descriptionBox.text = item.description;
    }

    public override void onOKPressed() {
        base.onOKPressed();
        if(item.usableOnOverworld){
            FindObjectOfType<ItemManager>().useItemOnOverworld(item);
            onCancelPressed();
            pageOption.onCancelPressed();
        }
    }

    public override void onCancelPressed() {
        base.onCancelPressed();
        manager.resetPosition();
        itemCursor.gameObject.SetActive(false);
        listSwitchCursor.setActivityStatus(true);
    }

}
