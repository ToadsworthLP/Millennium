using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemEntry : SelectableHelper {

    public UsableItem item;
    [HideInInspector]
    public Cursor listSwitchCursor;

    private ItemListManager manager;

    private Image itemIcon;
    private TextMeshProUGUI itemName;

    private GameObject entryParent;
    private TextMeshProUGUI descriptionBox;
    private MenuPageOption pageOption;

    private Cursor itemCursor;

    public void SetupEntry(ItemListManager manager) {
        itemIcon = GetComponentInChildren<Image>();
        itemName = GetComponentInChildren<TextMeshProUGUI>();

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

    public override void OnCursorInit(Cursor cursor) {
        base.OnCursorInit(cursor);
        itemCursor = cursor;
    }

    public override void OnSideKeyPressed(Utils.EnumDirection direction) {
        base.OnSideKeyPressed(direction);

        switch (direction){
            case Utils.EnumDirection.UP:
                if (itemCursor.selectedIndex - 2 >= 0)
                    itemCursor.CursorMoved(-2);
                    manager.ScrollItemList(itemCursor.selectedIndex);
                break;

            case Utils.EnumDirection.DOWN:
                if (itemCursor.selectedIndex + 2 < itemCursor.optionObjects.Count)
                    itemCursor.CursorMoved(2);
                    manager.ScrollItemList(itemCursor.selectedIndex);
                break;

            case Utils.EnumDirection.LEFT:
                manager.ScrollItemList(itemCursor.selectedIndex);
                break;

            case Utils.EnumDirection.RIGHT:
                manager.ScrollItemList(itemCursor.selectedIndex);
                break;
        }
    }

    public override void OnCursorSelect() {
        base.OnCursorSelect();
        descriptionBox.text = item.description;
    }

    public override void OnOKPressed() {
        base.OnOKPressed();
        if(item.usableOnOverworld){
            FindObjectOfType<ItemManager>().UseItemOnOverworld(item);
            OnCancelPressed();
            pageOption.OnCancelPressed();
        }
    }

    public override void OnCancelPressed() {
        base.OnCancelPressed();
        manager.ResetPosition();
        itemCursor.gameObject.SetActive(false);
        listSwitchCursor.SetActivityStatus(true);
    }

}
