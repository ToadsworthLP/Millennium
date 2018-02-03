using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CutsceneManager))]
public class CutsceneManagerEditor : Editor
{

    [MenuItem("GameObject/Cutscene", false, 0)]
    private static void CreateCutsceneGameObject() {
        var selected = Selection.activeTransform;
        GameObject gameObject = new GameObject() {
            name = "New cutscene"
        };
        gameObject.AddComponent<CutsceneManager>();

        if (selected != null)
            gameObject.transform.SetParent(selected);

        gameObject.transform.localPosition = Vector3.zero;
        Selection.SetActiveObjectWithContext(gameObject, null);
    }

    CutsceneManager cutsceneManager;
    Type[] nodeTypes;
    bool showNodeTypes;

    public void OnEnable() {
        cutsceneManager = (CutsceneManager)target;
    }

    private void RefreshNodeTypes() {
        nodeTypes = Utils.GetAllSubclasses(typeof(BaseCutsceneNode));
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Nodes in this cutscene: " + cutsceneManager.nodes.Count);
        GUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        if (nodeTypes == null)
            RefreshNodeTypes();

        if (GUILayout.Button("Open in Sequence Editor", GUILayout.Width(255))) {
            NodeBasedEditor window = (NodeBasedEditor)EditorWindow.GetWindow(typeof(NodeBasedEditor));
            window.OpenWindow(cutsceneManager);
        }

        EditorGUILayout.BeginHorizontal();
        showNodeTypes = EditorGUILayout.Foldout(showNodeTypes, "Create nodes");
        if (GUILayout.Button("Refesh node types"))
            RefreshNodeTypes();
        EditorGUILayout.EndHorizontal();

        if (showNodeTypes) {
            foreach (Type type in nodeTypes) {
                if (GUILayout.Button("Create new " + type.Name)) {
                    BaseCutsceneNode node = new GameObject().AddComponent(type).GetComponent<BaseCutsceneNode>();
                    node.cutsceneManager = cutsceneManager;
                    node.transform.SetParent(cutsceneManager.transform);
                    node.transform.position = cutsceneManager.transform.position;
                    node.name = "New " + type.Name;
                }
            }
        }
    }
}
