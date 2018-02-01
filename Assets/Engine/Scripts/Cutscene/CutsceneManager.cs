using System.Collections.Generic;
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
        ReloadNodes();
    }

    public void OnTransformChildrenChanged() {
        ReloadNodes();
    }

    public void ReloadNodes(){
        BaseCutsceneNode[] newNodes = GetComponentsInChildren<BaseCutsceneNode>();
        nodes = new List<BaseCutsceneNode>(newNodes);
    }

    public void Play(){
        if(!isPlaying){
            isPlaying = true;
            gameManager.playerMachine.SetCutsceneMode(true);
            if (startNode != null) {
                startNode.CallNode();
            } else {
                Debug.LogError("No or invalid start node defined for cutscene " + gameObject.name);
            }
        }
    }

    public void Stop(){
        gameManager.playerMachine.SetCutsceneMode(false);
        isPlaying = false;
        if(OnCutsceneFinished != null){
            OnCutsceneFinished.Invoke(gameObject, null);
        }
    }

}