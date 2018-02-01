using System;
using UnityEditor;
using UnityEngine;

public class EditorConnection
{
    public EditorConnectionPoint inPoint;
    public EditorConnectionPoint outPoint;
    public Action<EditorConnection> OnRemoveConnection;

    public EditorConnection(EditorConnectionPoint inPoint, EditorConnectionPoint outPoint, Action<EditorConnection> OnRemoveConnection) {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.OnRemoveConnection = OnRemoveConnection;
    }

    public void Draw() {
        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            Color.white,
            null,
            2f
        );

        if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap)) {
            ProcessContextMenu();
        }
    }

    private void ProcessContextMenu() {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove connection"), false, OnClickRemoveConnection);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveConnection() {
        if (OnRemoveConnection != null) {
            OnRemoveConnection(this);
        }
    }
}