using UnityEngine;

public interface ISelectable {

    Vector3 getGrabPoint();

    void onCursorSelect();

    void onCursorLeave();

    void onOKPressed();

    void onCancelPressed();

}
