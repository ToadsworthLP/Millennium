using System;
using System.Collections;
using UnityEngine;

public class Hammer : MonoBehaviour{

    public HammerAsset hammer;
    public GameManager gameManager;
    public SpriteRenderer hammerSpriteRenderer;
    private PlayerMachine playerMachine;
    private Animator playerAnimator;

    private bool alreadyHit;

    public void swingHammer(){
        alreadyHit = false;
        playerMachine = gameManager.playerMachine;
        playerAnimator = gameManager.playerAnimator;

        playerMachine.hammering = true;
        playerMachine.art.hammerSwing();
        playerMachine.art.animator.SetFloat("Hammer", 1);
        playerMachine.allowMovement = false;
    }

    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player") && gameObject.activeSelf && !other.isTrigger && !alreadyHit){
            StartCoroutine(hammerHit(other.gameObject));
        }
    }

    IEnumerator hammerHit(GameObject hitObject){
        alreadyHit = true;
        playerAnimator.SetFloat("Hammer", 0);
        playerMachine.art.hammerHit();

        IHammerable hammerable = hitObject.GetComponent<IHammerable>();
        if (hammerable != null)
            hammerable.hammer();

        yield return new WaitForSeconds(gameManager.playerMachine.hammerDuration);

        playerAnimator.SetFloat("Hammer", -1);
        playerMachine.allowMovement = true;
        playerMachine.hammering = false;
        alreadyHit = false;
    }

}

[Serializable]
[CreateAssetMenu(fileName = "New Hammer", menuName = "Hammer")]
public class HammerAsset : ScriptableObject{

    public string hammerName;
    public Sprite hammerIcon;
    public Sprite hammerSprite;
    public int level;

}
