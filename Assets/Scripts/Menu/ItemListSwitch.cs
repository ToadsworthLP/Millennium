using UnityEngine;

public class ItemListSwitch : DescriptionSelectable {

    public Cursor itemCursor;
    public Color overlayColor;
    public ItemListManager itemListManager;
    public ItemListManager.EnumItemKind itemType;

    private Cursor switchCursor;
    private Color oldColor;
    private CanvasRenderer canvasRenderer;

    public void OnEnable() {
        canvasRenderer = GetComponent<CanvasRenderer>();

        if (oldColor == new Color())
            oldColor = canvasRenderer.GetColor();

        canvasRenderer.SetColor(overlayColor);
    }

    public override void onCursorInit(Cursor cursor) {
        base.onCursorInit(cursor);
        switchCursor = cursor;
    }

    public override void onCursorSelect() {
        base.onCursorSelect();
        canvasRenderer.SetColor(oldColor);
        itemListManager.showItems(itemType);
    }

    public override void onCursorLeave() {
        base.onCursorLeave();
        canvasRenderer.SetColor(overlayColor);
    }

    public override void onOKPressed() {
        base.onOKPressed();
        if((itemType == ItemListManager.EnumItemKind.NORMAL && itemListManager.backpack.normalItems.Count > 0)
            || (itemType == ItemListManager.EnumItemKind.IMPORTANT && itemListManager.backpack.importantItems.Count > 0)){
            switchCursor.setActivityStatus(false);
            itemCursor.gameObject.SetActive(true);
        }
    }

    public override void onCancelPressed() {
        canvasRenderer.SetColor(overlayColor);
        base.onCancelPressed();
    }

}
