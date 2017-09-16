using UnityEngine;

public class ItemIconPopup : MonoBehaviour {

    public SpriteRenderer sprite;

    public void show(Sprite icon){
        sprite.sprite = icon;
    }

}
