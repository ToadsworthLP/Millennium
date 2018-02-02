using UnityEngine;

public class DialogueNode : BaseCutsceneNode{

    public GameObject speechBubblePrefab;

    [TextArea]
    public string[] text;

    public AudioClip talkSound;
    public AudioClip skipSound;

    public Color bubbleTint = Color.white;
    public Color textTint = new Color32(60, 60, 60, 255);

    public override void CallNode() {
        GameObject bubble = Instantiate(speechBubblePrefab, cutsceneManager.gameManager.uiParent);
        Typewriter writer = bubble.GetComponent<Typewriter>();

        writer.talkSound = talkSound;
        writer.skipSound = skipSound;
        writer.OnBubbleClosed += Finished;
        writer.bubbleImage.color = bubbleTint;
        writer.textComponent.color = textTint;
        writer.StartWriting(text);
    }

    private void Finished(){
        CallOutputSlot("Next Node");
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }
}
