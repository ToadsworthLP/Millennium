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

    void Start () {
        uiParent = GameObject.FindGameObjectWithTag("UIParent").GetComponent<RectTransform>();
    }

    void pageFinished(int page){
        if(pageFinishedEvent != null){
            pageFinishedEvent.Invoke(gameObject, page.ToString());
        }
    }

    void bubbleClosed() {
        if (bubbleClosedEvent != null) {
            bubbleClosedEvent.Invoke(gameObject, null);
        }
        player.setCutsceneMode(false);
    }

    public override void interact(GameObject playerObject) {
        base.interact(playerObject);
        if (currentBubble == null) {
            GameObject bubble = Instantiate(speechBubble, uiParent);
            Typewriter writer = bubble.GetComponent<Typewriter>();
            player = playerObject.GetComponent<PlayerMachine>();
            writer.talkSound = talkSound;
            writer.skipSound = skipSound;
            writer.OnPageFinished += pageFinished;
            writer.OnBubbleClosed += bubbleClosed;
            writer.bubbleImage.color = bubbleTint;
            writer.bubbleText.color = textTint;
            writer.StartWriting(text);
            currentBubble = bubble;
            player.setCutsceneMode(true);
        }
    }
}
