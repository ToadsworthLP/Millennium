using System.Collections;
using UnityEngine;

public class HealingCollectible : MonoBehaviour {

    public AudioClip collectSound;
    public Sprite[] blinkSprites;
    public float blinkAnimationLength;
    public SpriteRenderer art;
    public Utils.StatModifier modifier;

    private Backpack backpack;
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
        backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
        defaultSprite = art.sprite;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            switch(modifier.statToModify){
                case Utils.StatType.HP:
                    backpack.hp += modifier.value;
                    break;

                case Utils.StatType.FP:
                    backpack.fp += modifier.value;
                    break;

                case Utils.StatType.SP:
                    backpack.sp += modifier.value;
                    break;
            }
            other.gameObject.GetComponent<PlayerMachine>().audioSource.PlayOneShot(collectSound);
            Destroy(gameObject);
        }
    }
}
