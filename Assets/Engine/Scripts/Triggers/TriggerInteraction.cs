using UnityEngine;

public class TriggerInteraction : InteractableHelper {

    [SerializeField]
    public MilleniumEvent testEvent;

    public override void Interact(GameObject playerObject) {
        base.Interact(playerObject);
        if (testEvent != null) {
            testEvent.Invoke(gameObject, "Test");
        }
    }

}
