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
    private SaveManager saveManager;
    private RectTransform uiParent;
    private PlayerMachine player;
    private GameObject currentBubble;
    private Animator animator;

    private bool blockAnimPlaying;

    void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        uiParent = gameManager.uiParent;
        player = gameManager.playerMachine;
        saveManager = gameManager.GetSaveManager();
        animator = gameObject.GetComponent<Animator>();
    }

    private void PageFinished(int page) {
        PopupMenu saveMenuObject = Instantiate(menuPrefab, uiParent).GetComponentInChildren<PopupMenu>();
        List<PopupMenuSettings> settings = new List<PopupMenuSettings> {
            new PopupMenuSettings("Yes", yesHighlightColor, YesSelected),
            new PopupMenuSettings("No", noHighlightColor, NoSelected)
        };
        saveMenuObject.SetupPopupMenu(settings);
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") && currentBubble == null && !blockAnimPlaying) {
            blockAnimPlaying = true;
            player.SetCutsceneMode(true);
            StartCoroutine(WaitForBlockAnimation());
        }
    }

    private void YesSelected(PopupMenuOption option){
        bool success = saveManager.SaveGame();
        GameObject bubble = Instantiate(speechBubble, uiParent);
        Typewriter writer = bubble.GetComponent<Typewriter>();
        writer.OnBubbleClosed += ResultBubbleClose;

        if (success) {
            writer.StartWriting(saveText);
        } else {
            writer.StartWriting(errorText);
        }
    }

    private void NoSelected(PopupMenuOption option) {
        player.SetCutsceneMode(false);
        player.SetFrozenStatus(false);
        StartCoroutine(WaitBeforeEnablingJump());
    }

    private void ResultBubbleClose() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        player.SetCutsceneMode(false);
        player.SetFrozenStatus(false);
        StartCoroutine(WaitBeforeEnablingJump());
    }

    IEnumerator WaitBeforeEnablingJump() {
        player.allowJumping = false;
        yield return new WaitForSeconds(jumpDelay);
        player.allowJumping = true;
    }

    IEnumerator WaitForBlockAnimation() {
        animator.SetTrigger("Hit");
        yield return new WaitForSeconds(textDelay);
        GameObject bubble = Instantiate(speechBubble, uiParent);
        Typewriter writer = bubble.GetComponent<Typewriter>();
        writer.talkSound = talkSound;
        writer.skipSound = skipSound;
        writer.textComponent.color = textTint;
        writer.OnPageFinished += PageFinished;
        writer.StartWriting(genText);
        currentBubble = bubble;
        blockAnimPlaying = false;
    }
}

