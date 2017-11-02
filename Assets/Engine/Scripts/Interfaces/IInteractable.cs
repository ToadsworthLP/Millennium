using UnityEngine;

public interface IInteractable {
    Sprite getIcon();
    void approach(GameObject playerObject);
    void interact(GameObject playerObject);
}