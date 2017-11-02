//using UnityEngine;
//using System.Collections.Generic;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//public class CutsceneManager : MonoBehaviour {

//    public NodeMap nodeMap;

//}


//#if UNITY_EDITOR
//[CustomEditor(typeof(CutsceneManager))]
//public class CutsceneManagerEditor : Editor
//{
//    CutsceneManager manager;

//    public void OnEnable() {
//        manager = (CutsceneManager)target;
//    }

//    public override void OnInspectorGUI() {
//        DrawDefaultInspector();
//        GUILayout.BeginHorizontal();
        
//        if(GUILayout.Button("Open Cutscene Editor", GUILayout.Width(255))){
//            NodeBasedEditor window = (NodeBasedEditor)EditorWindow.GetWindow(typeof(NodeBasedEditor));
//            if(manager.nodeMap != null){
//                window.OpenWindow("Cutscene", manager.nodeMap);
//            }else{
//                NodeMap map = new NodeMap();
//                List<NodeMap.NodeInfo> nodeList = new List<NodeMap.NodeInfo>();
//                nodeList.Add(new NodeMap.NodeInfo(typeof(EntryPointNode), new Vector2(100, 100)));
//                nodeList.Add(new NodeMap.NodeInfo(typeof(EndPointNode), new Vector2(300, 100)));
//                map.nodes = nodeList;
//                window.OpenWindow("Cutscene", map);
//            }
//        }

//        GUILayout.EndHorizontal();
//    }

//}
//#endif