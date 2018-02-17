using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeBasedEditor : EditorWindow
{
    public CutsceneManager cutsceneManager;
    public Dictionary<BaseCutsceneNode, EditorNode> nodes;
    public List<EditorConnection> connections;

    private GUIStyle nodeStyle;
    private GUIStyle headerStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private EditorConnectionPoint selectedInPoint;
    private EditorConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    private float menuBarHeight = 20f;
    private Rect menuBar;

    public enum ProjectionPlaneType{ XY, XZ, YZ }
    public ProjectionPlaneType projectionPlane = ProjectionPlaneType.XY;

    public static float offsetScaleFactor = -120;

    public void OpenWindow(CutsceneManager manager) {
        this.cutsceneManager = manager;
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Cutscene Sequence Editor");

        nodes = new Dictionary<BaseCutsceneNode, EditorNode>();
        AddNodesFromCutscene();
    }

    private void OnEnable() {
        headerStyle = new GUIStyle();
        headerStyle = EditorStyles.helpBox;

        nodeStyle = new GUIStyle();
        nodeStyle = EditorStyles.helpBox;

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);
    }

    private void OnDestroy() {
        selectedInPoint = null;
        selectedOutPoint = null;
        offset = Vector2.zero;
        drag = Vector2.zero;
        if (nodes != null) nodes.Clear();
        if (connections != null) connections.Clear();
    }

    private void OnGUI() {
        if(cutsceneManager == null){
            GUILayout.Label("The cutscene manager this editor is linked to has been deleted. Please close this window.");
        }else{

            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            DrawNodes();
            DrawConnections();

            DrawConnectionLine(Event.current);

            DrawMenuBar();

            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

        }

        if (GUI.changed) Repaint();
    }

    private void AddNodesFromCutscene(){
        selectedInPoint = null;
        selectedOutPoint = null;
        if (nodes != null) nodes.Clear();
        if (connections != null) connections.Clear();

        foreach (BaseCutsceneNode actualNode in cutsceneManager.nodes) {
            Vector2 targetPosition;
            switch (projectionPlane) {
                case ProjectionPlaneType.XY:
                    targetPosition = new Vector2(actualNode.transform.localPosition.x * -offsetScaleFactor, actualNode.transform.localPosition.y * offsetScaleFactor);
                    break;
                case ProjectionPlaneType.XZ:
                    targetPosition = new Vector2(actualNode.transform.localPosition.x * offsetScaleFactor, actualNode.transform.localPosition.z * offsetScaleFactor);
                    break;
                case ProjectionPlaneType.YZ:
                    targetPosition = new Vector2(actualNode.transform.localPosition.z * offsetScaleFactor, actualNode.transform.localPosition.y * offsetScaleFactor);
                    break;
                default:
                    targetPosition = new Vector2(actualNode.transform.localPosition.x * -offsetScaleFactor, actualNode.transform.localPosition.y * offsetScaleFactor);
                    break;
            }

            EditorNode node = new EditorNode(this, actualNode, targetPosition, headerStyle, nodeStyle) {
                OnRemoveNode = OnClickRemoveNode
            };
            node.PrepareConnections(inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint);

            nodes.Add(actualNode, node);
        }

        foreach (BaseCutsceneNode actualNode in cutsceneManager.nodes) {
            EditorNode node;
            nodes.TryGetValue(actualNode, out node);

            if(node != null){
                for (int i = 0; i < actualNode.outputNodes.Count; i++) {
                    if(actualNode.outputNodes[i] != null){
                        EditorNode targetNode;
                        nodes.TryGetValue(actualNode.outputNodes[i], out targetNode);

                        OnClickInPoint(targetNode.inPoints[0]);
                        OnClickOutPoint(node.outPoints[i]);
                    }
                }
            }
        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor) {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++) {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++) {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawMenuBar() {
        menuBar = new Rect(0, 0, position.width, menuBarHeight);

        GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Go to origin", EditorStyles.toolbarButton, GUILayout.Width(80))){
            ResetView();
        }

        GUILayout.Space(30);

        GUILayout.Label("View plane:", GUILayout.Width(70));
        GUILayout.Space(10);
        if(GUILayout.Button("XY", EditorStyles.toolbarButton, GUILayout.Width(30))){
            UpdateProjectionPlane(ProjectionPlaneType.XY);
        }
        GUILayout.Space(5);
        if (GUILayout.Button("ZX", EditorStyles.toolbarButton, GUILayout.Width(30))) {
            UpdateProjectionPlane(ProjectionPlaneType.XZ);
        }
        GUILayout.Space(5);
        if (GUILayout.Button("ZY", EditorStyles.toolbarButton, GUILayout.Width(30))) {
            UpdateProjectionPlane(ProjectionPlaneType.YZ);
        }

        GUILayout.Space(30);
        GUILayout.Label("Offset: "+offset, GUILayout.Width(200));

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void DrawNodes() {
        if (nodes != null) {
            try {
                foreach (EditorNode node in nodes.Values) {
                    node.Draw();
                }
            }catch(Exception){}
        }
    }

    private void DrawConnections() {
        if (connections != null) {
            for (int i = 0; i < connections.Count; i++) {
                connections[i].Draw();
            }
        }
    }

    private void UpdateProjectionPlane(ProjectionPlaneType newPlane) {
        offset = Vector2.zero;
        projectionPlane = newPlane;
        AddNodesFromCutscene();

        ResetView();
    }

    private void ResetView() {
        OnDrag(-offset);
        offset = new Vector2(position.width / 2, position.height / 2);
        OnDrag(offset);
    }

    private void ProcessEvents(Event e) {
        drag = Vector2.zero;

        switch (e.type) {
            case EventType.MouseDown:
                if (e.button == 0) {
                    ClearConnectionSelection();
                }

                if (e.button == 1) {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0) {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void ProcessNodeEvents(Event e) {
        if (nodes != null) {
            foreach (EditorNode node in nodes.Values) {
                bool guiChanged = node.ProcessEvents(e);

                if (guiChanged) {
                    GUI.changed = true;
                }
            }
        }
    }

    private void DrawConnectionLine(Event e) {
        if (selectedInPoint != null && selectedOutPoint == null) {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null) {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition) {
        GenericMenu genericMenu = new GenericMenu();
        Type[] nodeTypes = Utils.GetAllSubclasses(typeof(BaseCutsceneNode));
        foreach (Type type in nodeTypes) {
            genericMenu.AddItem(new GUIContent("Add " + type.Name), false, () => OnClickAddNode(mousePosition, type));
        }
        genericMenu.ShowAsContext();
    }

    private void OnDrag(Vector2 delta) {
        drag = delta;

        if (nodes != null) {
            foreach (EditorNode node in nodes.Values) {
                node.Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition, Type type, Dictionary<string, object> data = null) {
        //Actually adding it to the scene
        GameObject actualObject = new GameObject();
        Undo.RegisterCreatedObjectUndo(actualObject, "create cutscene node");
        BaseCutsceneNode actualNode = Undo.AddComponent(actualObject, type).GetComponent<BaseCutsceneNode>();
        
        actualNode.cutsceneManager = cutsceneManager;
        actualNode.transform.SetParent(cutsceneManager.transform);
        actualNode.transform.position = cutsceneManager.transform.position;
        actualNode.name = "New " + type.Name;

        EditorNode node = new EditorNode(this, actualNode, mousePosition, headerStyle, nodeStyle) {
            OnRemoveNode = OnClickRemoveNode
        };
        node.PrepareConnections(inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint);

        nodes.Add(actualNode, node);
    }

    private void OnClickInPoint(EditorConnectionPoint inPoint) {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null) {
            if (selectedOutPoint.node != selectedInPoint.node) {
                CreateConnection();
                ClearConnectionSelection();
            } else {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(EditorConnectionPoint outPoint) {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null) {
            if (selectedOutPoint.node != selectedInPoint.node) {
                CreateConnection();
                ClearConnectionSelection();
            } else {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(EditorNode node) {
        if (connections != null) {
            List<EditorConnection> connectionsToRemove = new List<EditorConnection>();

            for (int i = 0; i < connections.Count; i++) {
                if ((node.inPoints != null && node.inPoints.Contains(connections[i].inPoint)) || (node.outPoints != null && node.outPoints.Contains(connections[i].outPoint))) {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++) {
                connectionsToRemove[i].inPoint.connection = null;
                connectionsToRemove[i].outPoint.connection = null;
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        if(node.actualNode != null){
            Undo.DestroyObjectImmediate(node.actualNode.gameObject);
        }

        nodes.Remove(node.actualNode);
    }

    private void OnClickRemoveConnection(EditorConnection connection) {
        connection.inPoint.connection = null;
        connection.outPoint.connection = null;
        connections.Remove(connection);

        Undo.RecordObject(selectedOutPoint.node.actualNode.gameObject, "remove node connections");
        selectedOutPoint.node.actualNode.outputNodes[selectedOutPoint.id-1] = null;
    }

    private void CreateConnection() {
        if (selectedOutPoint.connection == null) {
            if (connections == null) {
                connections = new List<EditorConnection>();
            }

            EditorConnection conn = new EditorConnection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection);
            selectedInPoint.connection = conn;
            selectedOutPoint.connection = conn;
            connections.Add(conn);

            Undo.RecordObject(selectedOutPoint.node.actualNode.gameObject, "create node connection");
            selectedOutPoint.node.actualNode.outputNodes[selectedOutPoint.id-1] = selectedInPoint.node.actualNode;
        }
    }

    private void ClearConnectionSelection() {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    public Vector2 GetOffset(){
        return offset;
    }
}
