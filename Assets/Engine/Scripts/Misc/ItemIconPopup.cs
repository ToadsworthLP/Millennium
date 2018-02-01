using UnityEngine;

public class ItemIconPopup : MonoBehaviour {

    public SpriteRenderer sprite;

    public void Show(Sprite icon){
        sprite.sprite = icon;
    }

}
