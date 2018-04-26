using System.Collections;
using UnityEngine;

public class BlinkingSprite : MonoBehaviour {

    public Sprite[] blinkSprites;
    public float blinkAnimationLength;
    public SpriteRenderer art;
    private Sprite defaultSprite;

    public void Blink() {
        StartCoroutine(BlinkingAnimation());
    }

    IEnumerator BlinkingAnimation(){
        float frameLength = blinkAnimationLength / blinkSprites.Length;
        foreach(Sprite s in blinkSprites){
            art.sprite = s;
            yield return new WaitForSeconds(frameLength);
        }
        art.sprite = defaultSprite;
    }

    void Awake() {
        defaultSprite = art.sprite;
    }
}
