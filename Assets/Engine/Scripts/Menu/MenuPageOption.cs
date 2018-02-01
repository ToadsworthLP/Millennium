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

    public override void OnCursorInit(Cursor cursor) {
        base.OnCursorInit(cursor);
        oldColor = canvasRenderer.GetColor();
        pageSelectCursor = cursor;

        canvasRenderer.SetColor(overlayColor);
    }

    public override void OnCursorSelect() {
        base.OnCursorSelect();

        int selectedIndex = pageSelectCursor.selectedIndex;
        MenuPage selectedPage = menuManager.GetPageOfIndex(selectedIndex);

        menuManager.descriptionBox.text = descriptionText;
        canvasRenderer.SetColor(oldColor);
        selectedPage.gameObject.SetActive(true);
        selectedPage.FocusPage();
        selectedPage.transform.SetSiblingIndex(menuManager.pages.Length);
        if(pageSelectCursor.previousSelectedIndex > pageSelectCursor.selectedIndex){
            selectedPage.animator.SetTrigger("TurnRight");
        }else if(pageSelectCursor.previousSelectedIndex < pageSelectCursor.selectedIndex) {
            selectedPage.animator.SetTrigger("TurnLeft");
        }
    }

    public override void OnCursorLeave() {
        base.OnCursorLeave();
        canvasRenderer.SetColor(overlayColor);
        if(pageCursor != null){
            pageCursor.gameObject.SetActive(false);
        }
        menuManager.GetPageOfIndex(pageSelectCursor.previousSelectedIndex).UnfocusPage(menuManager.animationLength);
    }

    public override void OnOKPressed() {
        base.OnOKPressed();
        if (pageCursor != null) {
            pageSelectCursor.SetActivityStatus(false);
            pageCursor.gameObject.SetActive(true);
        }
    }

    public override void OnCancelPressed() {
        base.OnCancelPressed();
        if (pageCursor != null) {
            pageCursor.gameObject.SetActive(false);
        }
        menuManager.CloseMenu();
    }

}
