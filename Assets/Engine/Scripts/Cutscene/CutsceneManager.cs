using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class CutsceneManager : MonoBehaviour {

    public GameManager gameManager;
    [HideInInspector]
    public List<BaseCutsceneNode> nodes;
    [HideInInspector]
    public BaseCutsceneNode startNode;
    [HideInInspector]
    public bool isPlaying;
    public MilleniumEvent OnCutsceneFinished;

    public void OnEnable() {
        reloadNodes();
    }

    public void OnTransformChildrenChanged() {
        reloadNodes();
    }

    public void reloadNodes(){
        BaseCutsceneNode[] newNodes = GetComponentsInChildren<BaseCutsceneNode>();
        nodes = new List<BaseCutsceneNode>(newNodes);
    }

    public void play(){
        if(!isPlaying){
            isPlaying = true;
            gameManager.playerMachine.setCutsceneMode(true);
            if (startNode != null) {
                startNode.callNode();
            } else {
                Debug.LogError("No or invalid start node defined for cutscene " + gameObject.name);
            }
        }
    }

    public void stop(){
        gameManager.playerMachine.setCutsceneMode(false);
        isPlaying = false;
        if(OnCutsceneFinished != null){
            OnCutsceneFinished.Invoke(gameObject, null);
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(CutsceneManager))]
public class CutsceneManagerEditor : Editor {

    [MenuItem("GameObject/Cutscene", false, 0)]
    private static void createCutsceneGameObject() {
        var selected = Selection.activeTransform;
        GameObject gameObject = new GameObject();
        gameObject.name = "New cutscene";
        gameObject.AddComponent<CutsceneManager>();

        if (selected != null)
            gameObject.transform.SetParent(selected);

        Selection.SetActiveObjectWithContext(gameObject, null);
    }

    CutsceneManager cutsceneManager;
    Type[] nodeTypes;
    bool showNodeTypes;

    public void OnEnable() {
        cutsceneManager = (CutsceneManager)target;
    }

    private void refreshNodeTypes(){
        nodeTypes = Utils.GetAllSubclasses(typeof(BaseCutsceneNode));
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Nodes in this cutscene: " + cutsceneManager.nodes.Count);
        GUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        if (nodeTypes == null)
            refreshNodeTypes();

        if (GUILayout.Button("Open in Sequence Editor", GUILayout.Width(255))) {
            NodeBasedEditor window = (NodeBasedEditor)EditorWindow.GetWindow(typeof(NodeBasedEditor));
            window.OpenWindow(cutsceneManager);
        }

        EditorGUILayout.BeginHorizontal();
        showNodeTypes = EditorGUILayout.Foldout(showNodeTypes, "Create nodes");
        if (GUILayout.Button("Refesh node types"))
            refreshNodeTypes();
        EditorGUILayout.EndHorizontal();

        if(showNodeTypes){
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
#endif