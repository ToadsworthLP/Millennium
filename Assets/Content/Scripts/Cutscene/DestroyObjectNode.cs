using UnityEngine;

public class DestroyObjectNode : BaseCutsceneNode
{

    public GameObject targetTransform;

    public override void CallNode() {
        Destroy(targetTransform);
        CallOutputSlot("Next Node");
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

}
