using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveBlock : MonoBehaviour
{

    [TextArea]
    public string[] genText;
    [TextArea]
    public string[] saveText;
    [TextArea]
    public string[] errorText;

    public GameObject speechBubble;
    public GameObject menuPrefab;

    public float jumpDelay;

    public AudioClip talkSound;
    public AudioClip skipSound;
    public AudioClip blockSound;

    public Color textTint;
    public float textDelay;

    public Color yesHighlightColor;
    public Color noHighlightColor;

    private GameManager gameManager;
    private RectTransform uiParent;
    private PlayerMachine player;
    private Backpack backpack;
    private GameObject currentBubble;
    private Animator animator;

    private bool blockAnimPlaying;

    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        uiParent = gameManager.uiParent;
        player = gameManager.playerMachine;
        backpack = gameManager.getBackpack();
        animator = gameObject.GetComponent<Animator>();
    }

    private void pageFinished(int page) {
        PopupMenu saveMenuObject = Instantiate(menuPrefab, uiParent).GetComponentInChildren<PopupMenu>();
        List<PopupMenuSettings> settings = new List<PopupMenuSettings>();
        settings.Add(new PopupMenuSettings("Yes", yesHighlightColor, yesSelected));
        settings.Add(new PopupMenuSettings("No", noHighlightColor, noSelected));
        saveMenuObject.setupPopupMenu(settings);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") && currentBubble == null && !blockAnimPlaying) {
            blockAnimPlaying = true;
            player.setCutsceneMode(true);
            StartCoroutine(waitForBlockAnimation());
        }
    }

    private void yesSelected(PopupMenuOption option){
        bool success = backpack.saveData();
        GameObject bubble = Instantiate(speechBubble, uiParent);
        Typewriter writer = bubble.GetComponent<Typewriter>();
        writer.OnBubbleClosed += resultBubbleClose;

        if (success) {
            writer.StartWriting(saveText);
        } else {
            writer.StartWriting(errorText);
        }
    }

    private void noSelected(PopupMenuOption option) {
        player.setCutsceneMode(false);
        player.setFrozenStatus(false);
        StartCoroutine(waitBeforeEnablingJump());
    }

    private void resultBubbleClose() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        player.setCutsceneMode(false);
        player.setFrozenStatus(false);
        StartCoroutine(waitBeforeEnablingJump());
    }

    IEnumerator waitBeforeEnablingJump() {
        player.allowJumping = false;
        yield return new WaitForSeconds(jumpDelay);
        player.allowJumping = true;
    }

    IEnumerator waitForBlockAnimation() {
        animator.SetTrigger("Hit");
        yield return new WaitForSeconds(textDelay);
        GameObject bubble = Instantiate(speechBubble, uiParent);
        Typewriter writer = bubble.GetComponent<Typewriter>();
        writer.talkSound = talkSound;
        writer.skipSound = skipSound;
        writer.bubbleText.color = textTint;
        writer.OnPageFinished += pageFinished;
        writer.StartWriting(genText);
        currentBubble = bubble;
        blockAnimPlaying = false;
    }
}

