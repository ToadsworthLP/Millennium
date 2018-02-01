using UnityEngine;

public class ItemListSwitch : DescriptionSelectable {

    public Cursor itemCursor;
    public Color overlayColor;
    public ItemListManager itemListManager;
    public ItemListManager.EnumItemKind itemType;
    public CanvasRenderer canvasRenderer;

    private Cursor switchCursor;
    private Color oldColor;

    public void OnEnable() {
        if (oldColor == new Color())
            oldColor = canvasRenderer.GetColor();

        canvasRenderer.SetColor(overlayColor);
    }

    public override void OnCursorInit(Cursor cursor) {
        base.OnCursorInit(cursor);
        switchCursor = cursor;
    }

    public override void OnCursorSelect() {
        base.OnCursorSelect();
        canvasRenderer.SetColor(oldColor);
        itemListManager.ShowItems(itemType);
    }

    public override void OnCursorLeave() {
        base.OnCursorLeave();
        canvasRenderer.SetColor(overlayColor);
    }

    public override void OnOKPressed() {
        base.OnOKPressed();
        if((itemType == ItemListManager.EnumItemKind.NORMAL && itemListManager.backpack.normalItems.Count > 0)
            || (itemType == ItemListManager.EnumItemKind.IMPORTANT && itemListManager.backpack.importantItems.Count > 0)){
            switchCursor.SetActivityStatus(false);
            itemCursor.gameObject.SetActive(true);
        }
    }

    public override void OnCancelPressed() {
        canvasRenderer.SetColor(overlayColor);
        base.OnCancelPressed();
    }

}
