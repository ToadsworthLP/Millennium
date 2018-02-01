using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]

public class TiledIconDisplay : MonoBehaviour {

    private RectTransform rectTransform;
    private Image image;

    public void UpdateIcons(int amount){
        if(rectTransform == null){
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }
        rectTransform.sizeDelta = new Vector2(image.mainTexture.width * amount, image.mainTexture.height);
    }

}
