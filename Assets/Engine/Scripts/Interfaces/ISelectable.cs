using UnityEngine;

public interface ISelectable {

    Vector3 GetGrabPoint();

    bool GetActive();

    void OnCursorInit(Cursor cursor);

    void OnCursorSelect();

    void OnCursorLeave();

    void OnOKPressed();

    void OnCancelPressed();

    void OnSideKeyPressed(Utils.EnumDirection direction);

}
