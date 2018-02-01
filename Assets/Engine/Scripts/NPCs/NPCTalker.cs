using UnityEngine;

public class NPCTalker : InteractableHelper {

    [TextArea]
    public string[] text;
    public GameObject speechBubble;

    public AudioClip talkSound;
    public AudioClip skipSound;

    public Color bubbleTint;
    public Color textTint;

    public MilleniumEvent pageFinishedEvent;
    public MilleniumEvent bubbleClosedEvent;

    private RectTransform uiParent;
    private PlayerMachine player;
    private GameObject currentBubble;

    void PageFinished(int page){
        if(pageFinishedEvent != null){
            pageFinishedEvent.Invoke(gameObject, page.ToString());
        }
    }

    void BubbleClosed() {
        if (bubbleClosedEvent != null) {
            bubbleClosedEvent.Invoke(gameObject, null);
        }
        player.SetCutsceneMode(false);
    }

    public override void Interact(GameObject playerObject) {
        base.Interact(playerObject);
        if (currentBubble == null) {
            player = playerObject.GetComponent<PlayerMachine>();
            GameObject bubble = Instantiate(speechBubble, player.gameManager.uiParent);
            Typewriter writer = bubble.GetComponent<Typewriter>();
            
            writer.talkSound = talkSound;
            writer.skipSound = skipSound;
            writer.OnPageFinished += PageFinished;
            writer.OnBubbleClosed += BubbleClosed;
            writer.bubbleImage.color = bubbleTint;
            writer.textComponent.color = textTint;
            writer.StartWriting(text);
            currentBubble = bubble;
            player.SetCutsceneMode(true);
        }
    }
}
