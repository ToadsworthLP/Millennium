using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BranchNode : EditorNode {
    public BranchNode(Vector2 position, GUIStyle headerStyle, GUIStyle boxStyle) : base(position, headerStyle, boxStyle) {
    }

    public override void DrawNodeContent() {
        GUI.Label(GetRectForDescription(1), "Test Branch");
    }

    public override string getTitle() {
        return "Branch";
    }

    public override float getHeight() {
        return 100;
    }

    public override float getWidth() {
        return 150;
    }

    public override int getInputCount() {
        return 1;
    }

    public override int getOutputCount() {
        return 2;
    }
}
