using UnityEngine;

public class NPCTalker : MonoBehaviour {

    [TextArea]
    public string[] text;
    public bool autoTalk;
    public TalkIcon talkIcon;
    public GameObject speechBubble;

    public AudioClip talkSound;
    public AudioClip skipSound;

    public Color bubbleTint;
    public Color textTint;

    public GameObject pageFinishedEventReciever;
    public GameObject bubbleCloseEventReciever;

    private RectTransform uiParent;
    private custom_inputs inputManager;
    private PlayerMachine player;
    private GameObject currentBubble;

    void Start () {
        uiParent = GameObject.FindGameObjectWithTag("UIParent").GetComponent<RectTransform>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<custom_inputs>();
    }

    void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            talkIcon.gameObject.SetActive(true);
            player.allowJumping = false;
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            talkIcon.playOutAnimation();
            player.allowJumping = true;
        }
    }

    void pageFinished(int page){
        if(pageFinishedEventReciever != null && pageFinishedEventReciever.GetComponent<ITriggerable>() != null){
            pageFinishedEventReciever.GetComponent<ITriggerable>().triggerObject(gameObject);
        }
    }

    void bubbleClosed() {
        if (pageFinishedEventReciever != null && pageFinishedEventReciever.GetComponent<ITriggerable>() != null) {
            pageFinishedEventReciever.GetComponent<ITriggerable>().triggerObject(gameObject);
        }
        player.allowMovement = true;
    }

    void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player") && currentBubble == null && (inputManager.isInput[4]  || autoTalk)) {
            GameObject bubble = Instantiate(speechBubble, uiParent);
            Typewriter writer = bubble.GetComponent<Typewriter>();
            writer.talkSound = talkSound;
            writer.skipSound = skipSound;
            writer.OnPageFinished += pageFinished;
            writer.OnBubbleClosed += bubbleClosed;
            writer.bubbleImage.color = bubbleTint;
            writer.bubbleText.color = textTint;
            writer.StartWriting(text);
            currentBubble = bubble;
            player.allowMovement = false;
        }
    }
}
