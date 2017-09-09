using UnityEngine;

public class TriggerAnimationSelectable : DescriptionSelectable {

    public Animator animator;
    public string selectTriggerName;
    public string leaveTriggerName;

    public override void onCursorSelect() {
        base.onCursorSelect();
        animator.SetTrigger(selectTriggerName);
    }

    public override void onCursorLeave() {
        base.onCursorLeave();
        animator.SetTrigger(leaveTriggerName);
    }

}
