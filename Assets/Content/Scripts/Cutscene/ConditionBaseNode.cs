using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConditionBaseNode : BaseCutsceneNode {

    public override void CallNode() {
        if (Condition()) {
            CallOutputSlot("True");
        } else {
            CallOutputSlot("False");
        }
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("True");
        SetOutputSlot("False");
    }

    public abstract bool Condition();

}
