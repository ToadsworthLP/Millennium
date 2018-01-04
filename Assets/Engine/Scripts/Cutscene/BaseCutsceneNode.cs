#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class BaseCutsceneNode : MonoBehaviour {

    public abstract void declareOutputSlots();
    public abstract void callNode();
    [HideInInspector]
    public CutsceneManager cutsceneManager;

    //I have to use lists for this because a dictionary isn't serialized and thus inaccessible using serializedObjects.FindProperty()
    [HideInInspector]
    public List<BaseCutsceneNode> outputNodes;
    [HideInInspector]
    public List<string> outputNodeLabels;

    public void Awake() {
        reloadOutputs();
    }

    public void OnDrawGizmos() {
        if(outputNodes != null && outputNodes.Count > 0){
            float width = HandleUtility.GetHandleSize(Vector3.zero) * 0.1f;
            foreach (BaseCutsceneNode node in outputNodes) {
                if(node != null)
                    Handles.DrawLine(transform.position, node.transform.position);
            }
        }
    }

    public void OnTransformParentChanged() {
        transform.parent = cutsceneManager.transform;
    }

    public void callOutputSlot(string label){
        BaseCutsceneNode node;
        if(outputNodeLabels.Contains(label)){
            int targetIndex = outputNodeLabels.IndexOf(label);
            if(outputNodes.Count > targetIndex){
                node = outputNodes[targetIndex];
                if(node != null){
                    node.callNode();
                    return;
                }
            }
        }
        cutsceneManager.stop();
    }

    public void setOutputSlot(string label) {
        if(!outputNodeLabels.Contains(label)){
            outputNodeLabels.Add(label);
            outputNodes.Add(null);
        }
    }

    private BaseCutsceneNode getOutputSlot(string label) {
        BaseCutsceneNode node = null;
        if (outputNodeLabels.Contains(label)) {
            int targetIndex = outputNodeLabels.IndexOf(label);
            if (outputNodes.Count > targetIndex) {
                node = outputNodes[targetIndex];
            }
        }
        return node;
    }

    public void resetOutputs() {
        outputNodes = new List<BaseCutsceneNode>();
        outputNodeLabels = new List<string>();
        declareOutputSlots();
    }

    public void reloadOutputs() {
        List<BaseCutsceneNode> outputNodesBackup = outputNodes;
        List<string> outputNodeLabelsBackup = outputNodeLabels;

        resetOutputs();

        if(outputNodeLabels != null && outputNodeLabelsBackup != null){
            for (int i = 0; i < outputNodeLabels.Count; i++) {
                if (outputNodeLabels.Count > i && outputNodeLabelsBackup.Contains(outputNodeLabels[i])) {
                    int indexInBackup = outputNodeLabelsBackup.IndexOf(outputNodeLabels[i]);
                    outputNodes[i] = outputNodesBackup[indexInBackup];
                }
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BaseCutsceneNode), true)]
public class BaseCutsceneNodeEditor : Editor
{

    private bool showOutputs;
    private bool outputLinkMode;
    private bool showNodeProperties;

    BaseCutsceneNode node;

    private int linkableOutputSlotIndex;
    private BaseCutsceneNode linkableOutputSlotNodeBackup;

    private void OnEnable() {
        node = (BaseCutsceneNode)target;
    }

    private void OnDisable() {
        if(outputLinkMode){
            node.outputNodes[linkableOutputSlotIndex] = linkableOutputSlotNodeBackup;
            outputLinkMode = false;
            SceneView.onSceneGUIDelegate = null;
        }
    }

    public override void OnInspectorGUI() {
        if(outputLinkMode){
            GUILayout.Label("Please press the button on the node in the scene view you want to link to " + node.outputNodeLabels[linkableOutputSlotIndex] + ".", EditorStyles.wordWrappedLabel);
        }else{
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Options");
            if (GUILayout.Button("Set as start node"))
                node.cutsceneManager.startNode = node;
            if (GUILayout.Button("Select cutscene manager"))
                Selection.SetActiveObjectWithContext(node.cutsceneManager.gameObject, null);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            showOutputs = EditorGUILayout.Foldout(showOutputs, "Outputs");
            if (GUILayout.Button("Refresh slots"))
                node.reloadOutputs();
            EditorGUILayout.EndHorizontal();

            if (showOutputs) {
                SerializedProperty sp = serializedObject.FindProperty("outputNodes");

                if (sp.isArray) {
                    int arrayLength = 0;

                    sp.Next(true); // skip generic field
                    sp.Next(true); // advance to array size field

                    arrayLength = sp.intValue;

                    sp.Next(true);

                    // loop through all serialized properties and create a property field for each one
                    int lastIndex = arrayLength - 1;
                    for (int i = 0; i < arrayLength; i++) {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(sp, new GUIContent(node.outputNodeLabels[i]), null);

                        if (GUILayout.Button("Link")) {
                            outputLinkMode = true;
                            linkableOutputSlotNodeBackup = node.outputNodes[i];
                            SceneView.onSceneGUIDelegate += drawFloatingButtons;
                            if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();
                            linkableOutputSlotIndex = i;
                        }

                        EditorGUILayout.EndHorizontal();
                        if (i < lastIndex) sp.Next(false); // advance without drilling into children
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }


            DrawDefaultInspector();
        }

    }

    private void drawFloatingButtons(SceneView scene) {
        Handles.BeginGUI();

        float screenHeight = SceneView.currentDrawingSceneView.position.size.y;
        Vector2 buttonSize = new Vector2(20, 20);

        List<BaseCutsceneNode> otherNodes = new List<BaseCutsceneNode>(node.cutsceneManager.nodes);
        otherNodes.Remove(node);

        foreach (BaseCutsceneNode n in otherNodes) {
            Vector3 pos = n.gameObject.transform.position;
            Vector3 screenPoint = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(pos);

            // this prevents the GUI control from being drawn if you aren't looking at it
            if (screenPoint.z > 0) {
                Vector2 buttonPos = new Vector2(screenPoint.x - buttonSize.x * 0.5f, screenHeight - screenPoint.y - buttonSize.y);
                if (GUI.Button(new Rect(buttonPos, buttonSize), " ")){
                    node.outputNodes[linkableOutputSlotIndex] = n;
                    SceneView.onSceneGUIDelegate = null;
                    outputLinkMode = false;
                }
            }
        }

        Handles.EndGUI();
    }
}
#endif