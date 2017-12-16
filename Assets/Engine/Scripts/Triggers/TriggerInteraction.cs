using UnityEngine;

public class TriggerInteraction : InteractableHelper {

    [SerializeField]
    public MilleniumEvent testEvent;

    public override void interact(GameObject playerObject) {
        base.interact(playerObject);
        if (testEvent != null) {
            testEvent.Invoke(gameObject, "Test");
        }
    }

}
