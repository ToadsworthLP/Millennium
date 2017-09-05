using UnityEngine;
using UnityEngine.UI;

public class MenuPageOption : SelectableHelper {

    [TextArea]
    public string descriptionText;
    public Color overlayColor;

    private Color oldColor;
    private Text descriptionBox;
    private CanvasRenderer canvasRenderer;

    public override void onCursorInit() {
        base.onCursorInit();
        descriptionBox = GetComponentInParent<MenuManager>().descriptionBox;
        canvasRenderer = GetComponent<CanvasRenderer>();
        oldColor = canvasRenderer.GetColor();

        canvasRenderer.SetColor(overlayColor);
    }

    public override void onCursorSelect() {
        base.onCursorSelect();
        if(descriptionBox == null){
            descriptionBox = GetComponentInParent<MenuManager>().descriptionBox;
        }
        descriptionBox.text = descriptionText;

        canvasRenderer.SetColor(oldColor);
    }

    public override void onCursorLeave() {
        base.onCursorLeave();
        canvasRenderer.SetColor(overlayColor);
    }

}
