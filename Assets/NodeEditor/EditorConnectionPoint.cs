using System;
using UnityEngine;

public enum ConnectionPointType { In, Out }

public class EditorFlowConnectionPoint
{
    public Rect rect;
    public ConnectionPointType type;
    public EditorNode node;
    public GUIStyle style;

    public int id;

    public EditorConnection connection;

    public Action<EditorFlowConnectionPoint> OnClickConnectionPoint;

    public EditorFlowConnectionPoint(EditorNode node, ConnectionPointType type, GUIStyle style, Action<EditorFlowConnectionPoint> OnClickConnectionPoint, int id) {
        this.node = node;
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;

        this.id = id;

        rect = new Rect(0, 0, 10f, 20f);
    }

    public void Draw() {
        rect.y = node.GetRectForControl(id).y - rect.height * 0.5f;

        switch (type) {
            case ConnectionPointType.In:
                rect.x = node.rect.x - rect.width;
                break;

            case ConnectionPointType.Out:
                rect.x = node.rect.x + node.rect.width;
                break;
        }

        if (GUI.Button(rect, "", style)) {
            if (OnClickConnectionPoint != null) {
                OnClickConnectionPoint(this);
            }
        }
    }
}