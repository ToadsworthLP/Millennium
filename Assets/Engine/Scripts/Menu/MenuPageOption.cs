using UnityEngine;

public class MenuPageOption : SelectableHelper {

    public Cursor pageCursor;

    [TextArea]
    public string descriptionText;
    public Color overlayColor;
    public CanvasRenderer canvasRenderer;
    public MenuManager menuManager;

    private Cursor pageSelectCursor;
    private Color oldColor;

    public override void onCursorInit(Cursor cursor) {
        base.onCursorInit(cursor);
        oldColor = canvasRenderer.GetColor();
        pageSelectCursor = cursor;

        canvasRenderer.SetColor(overlayColor);
    }

    public override void onCursorSelect() {
        base.onCursorSelect();

        int selectedIndex = pageSelectCursor.selectedIndex;
        MenuPage selectedPage = menuManager.getPageOfIndex(selectedIndex);

        menuManager.descriptionBox.text = descriptionText;
        canvasRenderer.SetColor(oldColor);
        selectedPage.gameObject.SetActive(true);
        selectedPage.focusPage();
        selectedPage.transform.SetSiblingIndex(menuManager.pages.Length);
        if(pageSelectCursor.previousSelectedIndex > pageSelectCursor.selectedIndex){
            selectedPage.animator.SetTrigger("TurnRight");
        }else if(pageSelectCursor.previousSelectedIndex < pageSelectCursor.selectedIndex) {
            selectedPage.animator.SetTrigger("TurnLeft");
        }
    }

    public override void onCursorLeave() {
        base.onCursorLeave();
        canvasRenderer.SetColor(overlayColor);
        if(pageCursor != null){
            pageCursor.gameObject.SetActive(false);
        }
        menuManager.getPageOfIndex(pageSelectCursor.previousSelectedIndex).unfocusPage(menuManager.animationLength);
    }

    public override void onOKPressed() {
        base.onOKPressed();
        if (pageCursor != null) {
            pageSelectCursor.setActivityStatus(false);
            pageCursor.gameObject.SetActive(true);
        }
    }

    public override void onCancelPressed() {
        base.onCancelPressed();
        if (pageCursor != null) {
            pageCursor.gameObject.SetActive(false);
        }
        menuManager.closeMenu();
    }

}
