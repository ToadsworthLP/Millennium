using System.Collections;
using UnityEngine;

public class BlinkingSprite : MonoBehaviour {

    public Sprite[] blinkSprites;
    public float blinkAnimationLength;
    public SpriteRenderer art;

    private Sprite defaultSprite;
    private WaitForSeconds wait;

    private void Awake(){
        defaultSprite = art.sprite;

        float frameLength = blinkAnimationLength / blinkSprites.Length;
        WaitForSeconds wait = new WaitForSeconds(frameLength);
    }

    public void Blink() {
        StartCoroutine(BlinkingAnimation());
    }

    IEnumerator BlinkingAnimation(){
        foreach(Sprite s in blinkSprites){
            art.sprite = s;
            yield return wait;
        }
        art.sprite = defaultSprite;
    }
}
