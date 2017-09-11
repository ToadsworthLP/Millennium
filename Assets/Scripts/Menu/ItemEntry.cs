using UnityEngine.UI;

public class ItemEntry : SelectableHelper {

    public InventoryItem item;

    private Image itemIcon;
    private Text itemName;
    private Text descriptionBox;

    public void setupEntry(Text descBox) {
        itemIcon = GetComponentInChildren<Image>();
        itemName = GetComponentInChildren<Text>();
        descriptionBox = descBox;

        itemIcon.sprite = item.icon;
        itemName.text = item.name;
    }

    public override void onCursorSelect() {
        base.onCursorSelect();
        descriptionBox.text = item.itemDescription;
    }

}
