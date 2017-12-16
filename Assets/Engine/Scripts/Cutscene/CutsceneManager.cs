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
    public bool isPlaying;
    public MilleniumEvent OnCutsceneFinished;

    public void OnTransformChildrenChanged() {
        reloadNodes();
    }

    public void reloadNodes(){
        BaseCutsceneNode[] newNodes = GetComponentsInChildren<BaseCutsceneNode>();
        nodes = new List<BaseCutsceneNode>(newNodes);
    }

    public void play(){
        isPlaying = true;
        gameManager.playerMachine.setCutsceneMode(true);
        if(startNode != null){
            startNode.callNode();
        }else{
            Debug.LogError("No or invalid start node defined for cutscene "+gameObject.name);
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

    CutsceneManager cutsceneManager;

    public void OnEnable() {
        cutsceneManager = (CutsceneManager)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Nodes in this cutscene: " + cutsceneManager.nodes.Count);
        GUILayout.EndHorizontal();

        if(GUILayout.Button("Create new node")){
            BaseCutsceneNode node = new GameObject().AddComponent(typeof(DialogueNode)).GetComponent<BaseCutsceneNode>();
            node.cutsceneManager = cutsceneManager;
            node.transform.SetParent(cutsceneManager.transform);
            node.transform.position = cutsceneManager.transform.position;
        }
    }
}
#endif