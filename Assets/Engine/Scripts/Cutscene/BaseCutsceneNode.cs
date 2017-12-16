#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public abstract class BaseCutsceneNode : MonoBehaviour {

    public abstract void callNode();
    public abstract void declareOutputSlots();
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

        for (int i = 0; i < outputNodeLabels.Count; i++) {
            if (outputNodeLabels.Count > i && outputNodeLabelsBackup.Contains(outputNodeLabels[i])) {
                int indexInBackup = outputNodeLabelsBackup.IndexOf(outputNodeLabels[i]);

                outputNodes[i] = outputNodesBackup[indexInBackup];
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BaseCutsceneNode), true)]
public class BaseCutsceneNodeEditor : Editor
{

    bool showOutputs;
    bool showNodeProperties;

    BaseCutsceneNode node;
    SerializedProperty[] outputNodeProperties;

    public void OnEnable() {
        node = (BaseCutsceneNode)target;
    }

    public override void OnInspectorGUI() {

        EditorGUILayout.BeginHorizontal();
        showOutputs = EditorGUILayout.Foldout(showOutputs, "Outputs");
        if (GUILayout.Button("Reload slots"))
            node.reloadOutputs();
        EditorGUILayout.EndHorizontal();

        if (showOutputs){
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
                    EditorGUILayout.PropertyField(sp, new GUIContent(node.outputNodeLabels[i]), null);
                    if (i < lastIndex) sp.Next(false); // advance without drilling into children
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        showNodeProperties = EditorGUILayout.Foldout(showNodeProperties, "Node properties");
        if(showNodeProperties) {
            DrawDefaultInspector();
        }

    }
}
#endif