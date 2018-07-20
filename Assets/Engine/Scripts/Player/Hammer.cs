using SavePort.Types;
using System.Collections;
using UnityEngine;

public class Hammer : MonoBehaviour{

    public HammerAssetReference hammer;
    public GameManager gameManager;
    public SpriteRenderer hammerSpriteRenderer;
    private PlayerMachine playerMachine;
    private Animator playerAnimator;

    private bool alreadyHit;

    public void Start() {
        hammer.AddUpdateListener(UpdateHammer);
    }

    public void OnDestroy() {
        hammer.RemoveUpdateListener(UpdateHammer);
    }

    public void SwingHammer(){
        if (hammer == null)
            return;

        alreadyHit = false;
        playerMachine = gameManager.playerMachine;
        playerAnimator = gameManager.playerAnimator;

        playerMachine.hammering = true;
        playerMachine.art.HammerSwing();
        playerMachine.art.animator.SetFloat("Hammer", 1);
        playerMachine.allowMovement = false;
    }

    public void UpdateHammer(){
        hammerSpriteRenderer.sprite = hammer.Value.hammerSprite;
    }

    void OnTriggerEnter(Collider other) {
        if(!other.CompareTag("Player") && gameObject.activeSelf && !other.isTrigger && !alreadyHit){
            StartCoroutine(HammerHit(other.gameObject));
        }
    }

    IEnumerator HammerHit(GameObject hitObject){
        alreadyHit = true;
        playerAnimator.SetFloat("Hammer", 0);
        playerMachine.art.HammerHit();

        IHammerable hammerable = hitObject.GetComponent<IHammerable>();
        if (hammerable != null)
            hammerable.Hammer();

        yield return new WaitForSeconds(gameManager.playerMachine.hammerDuration);

        playerAnimator.SetFloat("Hammer", -1);
        playerMachine.allowMovement = true;
        playerMachine.hammering = false;
        alreadyHit = false;
    }

}
