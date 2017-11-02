using UnityEngine;
using UnityEngine.UI;

public class UIImageScroller : MonoBehaviour
{
    public Vector2 uvAnimationRate = new Vector2(1.0f, 1.0f);
    private Image image;

    void Start() {
        image = GetComponent<Image>();
    }

    Vector2 uvOffset = Vector2.zero;
    void LateUpdate() {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        if (image.enabled) {
            image.materialForRendering.mainTextureOffset = uvOffset;
        }
    }

}