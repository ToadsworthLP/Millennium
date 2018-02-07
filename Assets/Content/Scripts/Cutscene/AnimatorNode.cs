using UnityEngine;

public class AnimatorNode : BaseCutsceneNode {

    public Animator animator;
    public Utils.AnimationModifier modifier;

    public override void CallNode() {
        modifier.SetProperty(animator);

        CallOutputSlot("Next Node");
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

}
