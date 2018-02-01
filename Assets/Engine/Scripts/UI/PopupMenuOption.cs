using System;
using System.Collections;
using UnityEngine;

public class PopupMenuOption : SelectableHelper {
    public CanvasGroup parentCanvasRenderer;
    public Action<PopupMenuOption> action;
    public string label;

    private Cursor cursor;

    public override void OnOKPressed() {
        base.OnOKPressed();
        action.Invoke(this);
        cursor.SetActivityStatus(false);
        StartCoroutine(FadeOut());
    }

    public override void OnCursorInit(Cursor cursor) {
        base.OnCursorInit(cursor);
        this.cursor = cursor;
        textComponent.text = label;
    }

    IEnumerator FadeOut() {
        while (parentCanvasRenderer.alpha > 0) {
            parentCanvasRenderer.alpha = parentCanvasRenderer.alpha - 0.1f;
            yield return new WaitForEndOfFrame();
        }
        Destroy(parentCanvasRenderer.gameObject);
    }
}