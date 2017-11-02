using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NodeBasedEditor : EditorWindow {

    private string editorName;

    public List<EditorNode> nodes;
    public List<EditorConnection> connections;

    private GUIStyle nodeStyle;
    private GUIStyle headerStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private EditorFlowConnectionPoint selectedInPoint;
    private EditorFlowConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    public void OpenWindow(string editorName, NodeMap map) {
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        this.editorName = editorName;
        window.titleContent = new GUIContent(editorName + " Editor");

        if(map != null){
            LoadNodeMap(map);
        }
    }

    private void OnEnable() {
        headerStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box;
        headerStyle.border = new RectOffset(12, 12, 12, 12);

        nodeStyle = new GUIStyle();
        nodeStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

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
        editorName = null;
        selectedInPoint = null;
        selectedOutPoint = null;
        offset = Vector2.zero;
        drag = Vector2.zero;
        if(nodes != null) nodes.Clear();
        if(connections != null) connections.Clear();
    }

    private void OnGUI() {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.Button(new Rect(0, 0, 80, 20), "Save map"))
            SaveNodeMap();

        if (GUI.Button(new Rect(90, 0, 100, 20), "Go to origin")){
            OnDrag(-offset);
        }
            

        if (GUI.changed) Repaint();
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

    private void DrawNodes() {
        if (nodes != null) {
            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnections() {
        if (connections != null) {
            for (int i = 0; i < connections.Count; i++) {
                connections[i].Draw();
            }
        }
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
            for (int i = nodes.Count - 1; i >= 0; i--) {
                bool guiChanged = nodes[i].ProcessEvents(e);

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
        foreach(Type t in NodeTypes.GetNodeTypes(editorName)){
            genericMenu.AddItem(new GUIContent("Add " + t.Name), false, () => OnClickAddNode(mousePosition, t));
        }
        genericMenu.ShowAsContext();
    }

    private void OnDrag(Vector2 delta) {
        drag = delta;

        if (nodes != null) {
            for (int i = 0; i < nodes.Count; i++) {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition, Type type, Dictionary<string, object> data = null) {
        if (nodes == null) {
            nodes = new List<EditorNode>();
        }

        Type[] parameterTypes = { typeof(Vector2), typeof(GUIStyle), typeof(GUIStyle) };
        var constructor = type.GetConstructor(parameterTypes);

        object[] parameters = { mousePosition, headerStyle, nodeStyle };

        EditorNode node = (EditorNode)constructor.Invoke(parameters);

        node.OnRemoveNode = OnClickRemoveNode;

        if (data != null)
            node.data = data;

        node.prepareConnections(inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint);

        nodes.Add(node);
    }

    private void OnClickInPoint(EditorFlowConnectionPoint inPoint) {
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

    private void OnClickOutPoint(EditorFlowConnectionPoint outPoint) {
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

        nodes.Remove(node);
    }

    private void OnClickRemoveConnection(EditorConnection connection) {
        connection.inPoint.connection = null;
        connection.outPoint.connection = null;
        connections.Remove(connection);
    }

    private void CreateConnection() {
        if(selectedInPoint.connection == null && selectedOutPoint.connection == null){
            if (connections == null) {
                connections = new List<EditorConnection>();
            }

            EditorConnection conn = new EditorConnection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection);
            selectedInPoint.connection = conn;
            selectedOutPoint.connection = conn;
            connections.Add(conn);
        }
    }

    private void ClearConnectionSelection() {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void LoadNodeMap(NodeMap map){
        foreach (NodeMap.NodeInfo nodeInfo in map.nodes) {
            OnClickAddNode(nodeInfo.position, Type.GetType(nodeInfo.type), nodeInfo.getData());
        }
    }

    private void SaveNodeMap(){
        string path = EditorUtility.SaveFilePanel(
                "Save NodeMap",
                Application.dataPath,
                "New NodeMap.asset",
                "asset");

        path = path.Replace(Application.dataPath, "Assets");

        NodeMap map = CreateInstance<NodeMap>();
        map.createNodeMap(editorName, nodes, connections);

        AssetDatabase.CreateAsset(map, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
