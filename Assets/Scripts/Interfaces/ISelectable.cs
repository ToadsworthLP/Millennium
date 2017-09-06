using UnityEngine;

public interface ISelectable {

    Vector3 getGrabPoint();

    bool getActive();

    void onCursorInit(Cursor cursor);

    void onCursorSelect();

    void onCursorLeave();

    void onOKPressed();

    void onCancelPressed();

}
