using System;
using UnityEngine;

public class EndPointNode : EditorNode
{
    public EndPointNode(Vector2 position, GUIStyle headerStyle, GUIStyle boxStyle) : base(position, headerStyle, boxStyle) {
    }

    public override void DrawNodeContent() {
        GUI.Label(GetRectForDescription(1), "End here.");
    }

    public override string getTitle() {
        return "End Point";
    }

    public override float getHeight() {
        return 50;
    }

    public override float getWidth() {
        return 150;
    }

    public override int getInputCount() {
        return 1;
    }

    public override int getOutputCount() {
        return 0;
    }
}
