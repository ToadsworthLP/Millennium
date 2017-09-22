using UnityEngine;

public class TriggerAnimationSelectable : DescriptionSelectable {

    public Animator animator;
    public FloatAnimationSetter[] selectTriggers;
    public FloatAnimationSetter[] deselectTriggers;

    public override void onCursorSelect() {
        base.onCursorSelect();
        foreach (FloatAnimationSetter f in selectTriggers) {
            animator.SetFloat(f.name, f.value);
        }
    }

    public override void onCursorLeave() {
        base.onCursorLeave();
        foreach (FloatAnimationSetter f in deselectTriggers) {
            animator.SetFloat(f.name, f.value);
        }
    }

    public override void onCancelPressed() {
        base.onCancelPressed();
        foreach (FloatAnimationSetter f in deselectTriggers) {
            animator.SetFloat(f.name, f.value);
        }
    }

    [System.Serializable]
    public class FloatAnimationSetter{
        public string name;
        public float value;
    }

}
