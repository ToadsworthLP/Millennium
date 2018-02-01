using UnityEngine;

public class TriggerAnimationSelectable : DescriptionSelectable {

    public Animator animator;

    [Header("Float animations")]
    public FloatAnimationSetter[] floatSelectTriggers;
    public FloatAnimationSetter[] floatDeselectTriggers;

    public override void OnCursorSelect() {
        base.OnCursorSelect();
        foreach (FloatAnimationSetter f in floatSelectTriggers) {
            animator.SetFloat(f.name, f.value);
        }
    }

    public override void OnCursorLeave() {
        base.OnCursorLeave();
        foreach (FloatAnimationSetter f in floatDeselectTriggers) {
            animator.SetFloat(f.name, f.value);
        }
    }

    public override void OnCancelPressed() {
        base.OnCancelPressed();
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
