using UnityEngine;

public class TriggerAnimationSelectable : DescriptionSelectable {

    public Animator animator;

    [Header("Float animations")]
    public FloatAnimationSetter[] floatSelectTriggers;
    public FloatAnimationSetter[] floatDeselectTriggers;

    public override void onCursorSelect() {
        base.onCursorSelect();
        foreach (FloatAnimationSetter f in floatSelectTriggers) {
            animator.SetFloat(f.name, f.value);
        }
    }

    public override void onCursorLeave() {
        base.onCursorLeave();
        foreach (FloatAnimationSetter f in floatDeselectTriggers) {
            animator.SetFloat(f.name, f.value);
        }
    }

    public override void onCancelPressed() {
        base.onCancelPressed();
        foreach (FloatAnimationSetter f in floatDeselectTriggers) {
            animator.SetFloat(f.name, f.value);
        }
    }

    [System.Serializable]
    public class FloatAnimationSetter{
        public string name;
        public float value;
    }

}
