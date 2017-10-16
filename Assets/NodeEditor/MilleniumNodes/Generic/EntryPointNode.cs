using System;
using UnityEngine;

public class EntryPointNode : EditorNode
{
    public EntryPointNode(Vector2 position, GUIStyle headerStyle, GUIStyle boxStyle) : base(position, headerStyle, boxStyle) {
    }

    public override void DrawNodeContent() {
        GUI.Label(GetRectForDescription(1), "Build from here.");
    }

    public override string getTitle() {
        return "Entry Point";
    }

    public override float getHeight() {
        return 50;
    }

    public override float getWidth() {
        return 150;
    }

    public override int getInputCount() {
        return 0;
    }

    public override int getOutputCount() {
        return 1;
    }
}
