using UnityEngine;

public class ItemListSwitch : DescriptionSelectable {

    public Color overlayColor;

    private Color oldColor;
    private CanvasRenderer canvasRenderer;

    public void OnEnable() {
        canvasRenderer = GetComponent<CanvasRenderer>();

        if (oldColor == new Color())
            oldColor = canvasRenderer.GetColor();

        canvasRenderer.SetColor(overlayColor);
    }

    public override void onCursorSelect() {
        base.onCursorSelect();
        canvasRenderer.SetColor(oldColor);
    }

    public override void onCursorLeave() {
        base.onCursorLeave();
        canvasRenderer.SetColor(overlayColor);
    }

    public override void onCancelPressed() {
        canvasRenderer.SetColor(overlayColor);
        base.onCancelPressed();
    }

}
