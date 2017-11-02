//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[Serializable]
//public class NodeMap : ScriptableObject{

//    public string type;
//    public List<NodeInfo> nodes;

//    public void createNodeMap(string type, List<EditorNode> editorNodes, List<EditorConnection> connections) {
//        this.type = type;
//        nodes = new List<NodeInfo>();

//        //Dictionary<EditorNode, int> nodeTable = new Dictionary<EditorNode, int>();

//        LinkedList<NodeInfo> nodeList = new LinkedList<NodeInfo>();
//        LinkedListNode<NodeInfo> entryNode = new LinkedListNode<NodeInfo>(new NodeInfo(editorNodes[0].GetType(), editorNodes[0].rect.position, editorNodes[0].data));
//        nodeList.AddFirst(entryNode);

//        EditorNode currentNode = editorNodes[0];
//        while(currentNode.GetType() != typeof(EndPointNode)){
//            foreach (EditorFlowConnectionPoint conn in currentNode.outPoints) {
//                EditorNode nextNode = conn.connection.outPoint.node;
//                nodeList.AddAfter(entryNode, new NodeInfo(nextNode.GetType(), nextNode.rect.position, nextNode.data));
//                currentNode = nextNode;
//            }
//        }

//        //Index all the nodes into the ID table
//        int counter = 0;
//        foreach (EditorNode node in editorNodes) {
//            //Vector2 connectorRange = new Vector2(counter, 0);
//            //int i = 0;
//            //foreach (EditorFlowConnectionPoint point in node.inPoints) {

//            //}
//            //nodeTable.Add(node, counter);
//            nodes.Add(new NodeInfo(node.GetType(), node.rect.position, node.data));
//            counter++;
//        }

//        //Set the I/O IDs
//        //counter = 0;
//        //foreach (EditorNode node in editorNodes) {
//        //    Debug.Log("Node " + counter);

//        //    if(node.inPoints != null){
//        //        int[] inputIds = new int[node.getInputCount()];
//        //        Debug.Log("Inputs " + node.inPoints.Count);
//        //        for (int i = 0; i < node.inPoints.Count; i++) {
//        //            if (node.inPoints[i].connection != null)
//        //                inputIds[i] = nodeTable[node.inPoints[i].connection.outPoint.node];
//        //        }
//        //        nodes[counter].inputIds = inputIds;
//        //    }

//        //    if (node.outPoints != null) {
//        //        int[] outputIds = new int[node.getOutputCount()];
//        //        Debug.Log("Inputs " + node.outPoints.Count);
//        //        for (int i = 0; i < node.outPoints.Count; i++) {
//        //            if(node.outPoints[i].connection != null)
//        //                outputIds[i] = nodeTable[node.outPoints[i].connection.inPoint.node];
//        //        }
//        //        nodes[counter].outputIds = outputIds;
//        //    }

//        //    counter++;
//        //}
//    }

//    [Serializable]
//    public class NodeInfo{
//        public string type;
//        public Vector2 position;
//        [SerializeField]
//        private byte[] data;

//        public int[] inputIds;
//        public int[] outputIds;

//        public NodeInfo(Type type, Vector2 position, Dictionary<string, object> data = null){
//            this.type = type.FullName;
//            this.position = position;

//            if(data != null)
//                this.data = data.SerializeDict();
//        }

//        public Dictionary<string, object> getData(){
//            if(data != null && data.Length > 0){
//               return data.DeSerializeDict();
//            }
//            return null;
//        }
//    }

//}