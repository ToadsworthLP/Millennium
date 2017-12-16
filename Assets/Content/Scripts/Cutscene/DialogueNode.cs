using UnityEngine;

public class DialogueNode : BaseCutsceneNode{

    public GameObject speechBubblePrefab;

    [TextArea]
    public string[] text;

    public AudioClip talkSound;
    public AudioClip skipSound;

    public Color bubbleTint;
    public Color textTint;

    public override void callNode() {
        GameObject bubble = Instantiate(speechBubblePrefab, cutsceneManager.gameManager.uiParent);
        Typewriter writer = bubble.GetComponent<Typewriter>();

        writer.talkSound = talkSound;
        writer.skipSound = skipSound;
        writer.OnBubbleClosed += finished;
        writer.bubbleImage.color = bubbleTint;
        writer.textComponent.color = textTint;
        writer.startWriting(text);
    }

    private void finished(){
        callOutputSlot("Next Node");
    }

    public override void declareOutputSlots() {
        setOutputSlot("Next Node");
    }
}
