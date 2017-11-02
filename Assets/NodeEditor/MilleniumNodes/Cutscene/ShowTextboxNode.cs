using System;
using System.Collections.Generic;
using UnityEngine;

public class ShowTextboxNode : EditorNode
{
    public ShowTextboxNode(Vector2 position, GUIStyle headerStyle, GUIStyle boxStyle) : base(position, headerStyle, boxStyle) {
        data = new Dictionary<string, object>();
        data.Add("text", "Enter text...");
    }

    public override void DrawNodeContent() {
        data["text"] = GUI.TextArea(GetRectForLargeControl(1, 20, 5, 3), data.Get<string>("text"));
    }

    public override float getHeight() {
        return 100;
    }

    public override int getInputCount() {
        return 1;
    }

    public override int getOutputCount() {
        return 1;
    }

    public override string getTitle() {
        return "Show Text Box";
    }

    public override float getWidth() {
        return 200;
    }
}
