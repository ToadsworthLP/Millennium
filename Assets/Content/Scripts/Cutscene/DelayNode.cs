using System;
using System.Collections;
using UnityEngine;

public class DelayNode : BaseCutsceneNode
{
    public float delay;

    public override void CallNode() {
        StartCoroutine(WaitBeforeNextNode(delay));
    }

    private IEnumerator WaitBeforeNextNode(float delay){
        yield return new WaitForSeconds(delay);
        CallOutputSlot("Next Node");
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }
}
