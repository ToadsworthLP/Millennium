using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CutsceneManager : MonoBehaviour {

    public NodeMap nodeMap;

}


#if UNITY_EDITOR
[CustomEditor(typeof(CutsceneManager))]
public class CutsceneManagerEditor : Editor
{
    CutsceneManager manager;

    public void OnEnable() {
        manager = (CutsceneManager)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        
        if(GUILayout.Button("Open Cutscene Editor", GUILayout.Width(255))){
            NodeBasedEditor window = (NodeBasedEditor)EditorWindow.GetWindow(typeof(NodeBasedEditor));
            window.OpenWindow("Cutscene", manager.nodeMap);
        }

        GUILayout.EndHorizontal();
    }

}
#endif