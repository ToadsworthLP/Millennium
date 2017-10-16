using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NodeMap : ScriptableObject{

    public string type;
    public List<NodeInfo> nodes;

    public void createNodeMap(string type, List<EditorNode> editorNodes, List<EditorConnection> connections) {
        this.type = type;
        nodes = new List<NodeInfo>();
        foreach (EditorNode editorNode in editorNodes) {
            NodeInfo node = new NodeInfo(editorNode.GetType(), editorNode.rect.position, editorNode.data);
            nodes.Add(node);
        }
    }

    [Serializable]
    public class NodeInfo{
        public string type;
        public Vector2 position;
        [SerializeField]
        private byte[] data;

        public NodeInfo(Type type, Vector2 position, Dictionary<string, object> data){
            this.type = type.FullName;
            this.position = position;

            if(data != null)
                this.data = data.SerializeDict();
        }

        public Dictionary<string, object> getData(){
            if(data != null && data.Length > 0){
               return data.DeSerializeDict();
            }
            return null;
        }
    }

}