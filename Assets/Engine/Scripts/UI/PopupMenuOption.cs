using System;
using System.Collections;
using UnityEngine;

public class PopupMenuOption : SelectableHelper {
    public CanvasGroup parentCanvasRenderer;
    public Action<PopupMenuOption> action;
    public string label;

    private Cursor cursor;

    public override void onOKPressed() {
        base.onOKPressed();
        action.Invoke(this);
        cursor.setActivityStatus(false);
        StartCoroutine(fadeOut());
    }

    public override void onCursorInit(Cursor cursor) {
        base.onCursorInit(cursor);
        this.cursor = cursor;
        textComponent.text = label;
    }

    IEnumerator fadeOut() {
        while (parentCanvasRenderer.alpha > 0) {
            parentCanvasRenderer.alpha = parentCanvasRenderer.alpha - 0.1f;
            yield return new WaitForEndOfFrame();
        }
        Destroy(parentCanvasRenderer.gameObject);
    }
}