using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]

public class TiledIconDisplay : MonoBehaviour {

    public int test;

    private RectTransform rectTransform;
    private Image image;

    public void updateIcons(int amount){
        if(rectTransform == null){
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
        }
        rectTransform.sizeDelta = new Vector2(image.mainTexture.width * amount, image.mainTexture.height);
    }

}
