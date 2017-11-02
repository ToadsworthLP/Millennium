using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class EditorNode{

    public Dictionary<string, object> data;

    public float width;
    public float height;

    public bool isDragged;
    public bool isSelected;

    public Action<EditorNode> OnRemoveNode;
    public Action<EditorNode> OnDrawNode;

    public Rect rect;

    public List<EditorFlowConnectionPoint> inPoints;
    public List<EditorFlowConnectionPoint> outPoints;

    private GUIStyle headerStyle;
    private GUIStyle boxStyle;

    private bool hasInput;
    private bool hasOutput;

    public EditorNode(Vector2 position, GUIStyle headerStyle, GUIStyle boxStyle) {
        rect = new Rect(position.x, position.y, getWidth(), getHeight());
        this.headerStyle = headerStyle;
        this.boxStyle = boxStyle;
    }

    public void prepareConnections(GUIStyle inPointStyle, GUIStyle outPointStyle, Action<EditorFlowConnectionPoint> OnClickInPoint, Action<EditorFlowConnectionPoint> OnClickOutPoint) {
        hasInput = (getInputCount() > 0);
        hasOutput = (getOutputCount() > 0);

        if (hasInput) {
            inPoints = new List<EditorFlowConnectionPoint>();
            for (int i = 1; i <= getInputCount(); i++) {
                inPoints.Add(new EditorFlowConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint, i));
            }
        }

        if (hasOutput) {
            outPoints = new List<EditorFlowConnectionPoint>();
            for (int i = 1; i <= getOutputCount(); i++) {
                outPoints.Add(new EditorFlowConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint, i));
            }
        }

    }

    public void Drag(Vector2 delta) {
        rect.position += delta;
    }

    public void Draw() {
        //Draw the BG box
        GUI.Box(rect, "", boxStyle);

        //Draw the header
        GUI.Box(new Rect(rect.x, rect.y, rect.width, 25), "", headerStyle);
        GUI.Label(new Rect(rect.x+5, rect.y+5, rect.width, 25), getTitle());

        //Remove button
        if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, 20), "X"))
            OnClickRemoveNode();

        //Inputs and Outputs
        if(hasInput){
            foreach (EditorFlowConnectionPoint connectionPoint in inPoints) {
                connectionPoint.Draw();
            }
        }

        if (hasOutput) {
            foreach (EditorFlowConnectionPoint connectionPoint in outPoints) {
                connectionPoint.Draw();
            }
        }

        DrawNodeContent();

        if (OnDrawNode != null){
            OnDrawNode(this);
        }
    }

    public abstract void DrawNodeContent();

    public abstract float getWidth();

    public abstract float getHeight();

    public abstract int getInputCount();

    public abstract int getOutputCount();

    public abstract string getTitle();

    public virtual bool ProcessEvents(Event e) {
        switch (e.type) {
            case EventType.MouseDown:
                if (e.button == 0) {
                    if (rect.Contains(e.mousePosition)) {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                    } else {
                        GUI.changed = true;
                        isSelected = false;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition)) {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged) {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu() {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode() {
        if (OnRemoveNode != null) {
            OnRemoveNode(this);
        }
    }


    public Rect GetRectForControl(int row, float lineHeight = 20, float padding = 5) {
        return new Rect(rect.x + rect.width/2, rect.y + 5 + row * (lineHeight + padding), rect.width/2 - padding, lineHeight);
    }

    public Rect GetRectForLargeControl(int row, float lineHeight = 20, float padding = 5, float heightMultiplier = 1) {
        return new Rect(rect.x + padding, rect.y + 5 + row * (lineHeight + padding), rect.width - 2*padding, lineHeight * heightMultiplier);
    }

    public Rect GetRectForLabel(int row, float lineHeight = 20, float padding = 5) {
        return new Rect(rect.x + padding, rect.y + 5 + row*(lineHeight+padding), rect.width/2 - padding, lineHeight);
    }

    public Rect GetRectForDescription(int row, float lineHeight = 20, float padding = 5) {
        return new Rect(rect.x + padding, rect.y + 5 + row * (lineHeight + padding), rect.width - padding, lineHeight);
    }

    public EditorNode GetNextNode(int connectionId){
        if (hasOutput){
            return outPoints[connectionId].connection.inPoint.node;
        }else{
            return null;
        }
    }

    public EditorNode GetPreviousNode(int connectionId) {
        if (hasInput) {
            return inPoints[connectionId].connection.outPoint.node;
        } else {
            return null;
        }
    }
}