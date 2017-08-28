using UnityEngine;

public class TestSelectable : MonoBehaviour, ISelectable {

    public int id;
    public RectTransform grabPoint;

    public Vector3 getGrabPoint() {
        return grabPoint.position;
    }

    public void onCancelPressed() {
        print("Cancel pressed on "+id);
    }

    public void onCursorLeave() {
        print("Cursor left " + id);
    }

    public void onCursorSelect() {
        print("Cursor selected " + id);
    }

    public void onOKPressed() {
        print("OK pressed on " + id);
    }
}
