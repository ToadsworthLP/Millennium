using UnityEngine;

public class InteractableHelper : MonoBehaviour, IInteractable {

    public Sprite interactionIcon;
    private GameObject playerObject;

    public Sprite getIcon(){
        return interactionIcon;
    }

    public virtual void approach(GameObject playerObject) {
        this.playerObject = playerObject;
    }

    public virtual void interact(GameObject playerObject) {}
}
