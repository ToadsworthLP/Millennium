using UnityEngine;

public interface IInteractable {
    Sprite GetIcon();
    void Approach(GameObject playerObject);
    void Interact(GameObject playerObject);
}