using UnityEngine;
using UnityEngine.Events;

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

    public MilleniumEvent pageFinishedEvent;
    public MilleniumEvent bubbleClosedEvent;

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
        if(pageFinishedEvent != null){
            pageFinishedEvent.Invoke(gameObject, page.ToString());
        }
    }

    void bubbleClosed() {
        if (bubbleClosedEvent != null) {
            bubbleClosedEvent.Invoke(gameObject, null);
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
