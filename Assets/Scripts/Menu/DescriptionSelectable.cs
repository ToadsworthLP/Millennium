using UnityEngine;
using UnityEngine.UI;

public class DescriptionSelectable : SelectableHelper {

    public Cursor pageSelectionCursor;
    public Text descriptionTextBox;

    [TextArea]
    public string descriptionText;

    private GameObject pageCursor;

    public override void onCursorInit(Cursor cursor) {
        base.onCursorInit(cursor);
        pageCursor = cursor.gameObject;
    }

    public override void onCancelPressed() {
        base.onCancelPressed();
        pageCursor.SetActive(false);
        pageSelectionCursor.setActivityStatus(true);
    }

    public override void onCursorSelect() {
        base.onCursorSelect();
        descriptionTextBox.text = descriptionText;
    }

}
