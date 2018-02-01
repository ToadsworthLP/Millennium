using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionSelectable : SelectableHelper {

    public Cursor pageSelectionCursor;
    public TextMeshProUGUI descriptionTextBox;

    [TextArea]
    public string descriptionText;

    private GameObject pageCursor;

    public override void OnCursorInit(Cursor cursor) {
        base.OnCursorInit(cursor);
        pageCursor = cursor.gameObject;
    }

    public override void OnCancelPressed() {
        base.OnCancelPressed();
        pageCursor.SetActive(false);
        pageSelectionCursor.SetActivityStatus(true);
    }

    public override void OnCursorSelect() {
        base.OnCursorSelect();
        descriptionTextBox.text = descriptionText;
    }

}
