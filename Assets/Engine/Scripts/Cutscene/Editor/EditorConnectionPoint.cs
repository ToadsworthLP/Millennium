using System;
using UnityEditor;
using UnityEngine;

public enum ConnectionPointType { In, Out }

public class EditorConnectionPoint
{
    public string name;
    public Rect rect;
    public ConnectionPointType type;
    public EditorNode node;
    public GUIStyle style;

    public int id;

    public EditorConnection connection;

    public Action<EditorConnectionPoint> OnClickConnectionPoint;

    public EditorConnectionPoint(EditorNode node, ConnectionPointType type, GUIStyle style, Action<EditorConnectionPoint> OnClickConnectionPoint, int id, string name) {
        this.node = node;
        this.type = type;
        this.style = style;
        this.OnClickConnectionPoint = OnClickConnectionPoint;
        this.name = name;
        this.id = id;

        rect = new Rect(0, 0, 10f, 20f);
    }

    public void Draw() {
        rect.y = node.GetRectForLine(id).y - rect.height * 0.5f;

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

        if (name != null)
            //GUI.Label(new Rect(rect.x + 150, rect.y, rect.width - 100, rect.height), name, EditorStyles.label);
            GUI.Label(new Rect(rect.x-120, rect.y, rect.width+120, rect.height), name);

    }
}