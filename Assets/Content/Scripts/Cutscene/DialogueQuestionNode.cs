using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueQuestionNode : BaseCutsceneNode {

    public GameObject speechBubblePrefab;
    public GameObject questionMenuPrefab;

    [TextArea]
    public string[] questionText;
    public PopupMenuSettings[] options;

    public AudioClip talkSound;
    public AudioClip skipSound;

    public Color bubbleTint;
    public Color textTint;

    public override void CallNode() {
        GameObject bubble = Instantiate(speechBubblePrefab, cutsceneManager.gameManager.uiParent);
        Typewriter writer = bubble.GetComponent<Typewriter>();

        writer.talkSound = talkSound;
        writer.skipSound = skipSound;
        writer.OnPageFinished += PageFinished;
        writer.bubbleImage.color = bubbleTint;
        writer.textComponent.color = textTint;
        writer.StartWriting(questionText);
    }

    private void PageFinished(int pageNumber) {
        if(pageNumber+1 >= questionText.Length){
            PopupMenu saveMenuObject = Instantiate(questionMenuPrefab, cutsceneManager.gameManager.uiParent).GetComponentInChildren<PopupMenu>();
            List<PopupMenuSettings> settings = new List<PopupMenuSettings>();
            foreach (PopupMenuSettings option in options) {
                settings.Add(new PopupMenuSettings(option.label, option.highlightColor, chooseOption));
            }
            saveMenuObject.SetupPopupMenu(settings);
        }
    }

    private void chooseOption(PopupMenuOption option){
        CallOutputSlot(option.label);
    }

    public override void DeclareOutputSlots() {
        if(options != null && options.Length > 0){
            foreach (PopupMenuSettings option in options) {
                SetOutputSlot(option.label);
            }
        }
    }
}
