using UnityEngine;

public abstract class InteractableHelper : MonoBehaviour, IInteractable {

    public Sprite interactionIcon;
    private GameObject playerObject;

    public Sprite GetIcon(){
        return interactionIcon;
    }

    public virtual void Approach(GameObject playerObject) {
        this.playerObject = playerObject;
    }

    public virtual void Interact(GameObject playerObject) {}
}
