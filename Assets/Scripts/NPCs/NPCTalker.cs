using System.Collections.Generic;
using UnityEngine;

public class NPCTalker : InteractableHelper {

    public DialogNodeCanvas dialogCanvas;
    public GameObject speechBubblePrefab;

    [SerializeField]
    private GameObject messageBoxPrefab;

    private RectTransform uiParent;
    private PlayerMachine player;
    private GameObject currentBubble;

    public override void interact(GameObject playerObject) {
        base.interact(playerObject);

        
    }

}

//    public void Awake() {
//        uiParent = GameObject.FindGameObjectWithTag("UIParent").GetComponent<RectTransform>();

//        _messageBoxes = new Dictionary<int, MessageBoxHud>();
//        _dialogIdTracker = new Dictionary<int, DialogNodeCanvas>();
//        _dialogIdTracker.Clear();

//        if (dialogCanvas) {
//            foreach (int id in dialogCanvas.GetAllDialogId()) {
//                _dialogIdTracker.Add(id, dialogCanvas);
//            }
//        } else {
//            foreach (DialogNodeCanvas nodeCanvas in Resources.LoadAll<DialogNodeCanvas>("Saves/")) {
//                foreach (int id in nodeCanvas.GetAllDialogId()) {
//                    _dialogIdTracker.Add(id, nodeCanvas);
//                }
//            }
//        }
//    }

//    public void ShowDialogWithId(int dialogIdToLoad, bool goBackToBeginning) {
//        if (_messageBoxes.ContainsKey(dialogIdToLoad)) {
//            return;
//        }

//        DialogNodeCanvas nodeCanvas;
//        if (_dialogIdTracker.TryGetValue(dialogIdToLoad, out nodeCanvas)) {
//            nodeCanvas.ActivateDialog(dialogIdToLoad, goBackToBeginning);
//        } else
//            Debug.LogError("ShowDialogWithId Not found Dialog with ID : " + dialogIdToLoad);

//        GameObject bubble = Instantiate(speechBubblePrefab, uiParent);
//        Typewriter writer = bubble.GetComponent<Typewriter>();
//        player = playerObject.GetComponent<PlayerMachine>();
//        writer.talkSound = talkSound;
//        writer.skipSound = skipSound;
//        writer.OnPageFinished += pageFinished;
//        writer.OnBubbleClosed += bubbleClosed;
//        writer.bubbleImage.color = bubbleTint;
//        writer.bubbleText.color = textTint;
//        writer.StartWriting(nodeCanvas.getDialogStartNode.);
//        currentBubble = bubble;
//        player.setCutsceneMode(true);

//    }

//    private BaseDialogNode GetNodeForId(int dialogIdToLoad) {
//        DialogNodeCanvas nodeCanvas;
//        if (_dialogIdTracker.TryGetValue(dialogIdToLoad, out nodeCanvas)) {
//            return nodeCanvas.GetDialog(dialogIdToLoad);
//        } else
//            Debug.LogError("GetNodeForId Not found Dialog with ID : " + dialogIdToLoad);
//        return null;
//    }

//    private void GiveInputToDialog(int dialogIdToLoad, int inputValue) {
//        DialogNodeCanvas nodeCanvas;
//        if (_dialogIdTracker.TryGetValue(dialogIdToLoad, out nodeCanvas)) {
//            nodeCanvas.InputToDialog(dialogIdToLoad, inputValue);
//        } else
//            Debug.LogError("GiveInputToDialog Not found Dialog with ID : " + dialogIdToLoad);
//    }

//    public void OkayPressed(int dialogId) {
//        GiveInputToDialog(dialogId, (int)EDialogInputValue.Next);
//        _messageBoxes[dialogId].SetData(GetNodeForId(dialogId));
//    }

//    public void BackPressed(int dialogId) {
//        GiveInputToDialog(dialogId, (int)EDialogInputValue.Back);
//        _messageBoxes[dialogId].SetData(GetNodeForId(dialogId));
//    }

//    public void RemoveMessageBox(int dialogId) {
//        _messageBoxes.Remove(dialogId);
//    }

//    public void OptionSelected(int dialogId, int optionSelected) {
//        GiveInputToDialog(dialogId, optionSelected);
//        _messageBoxes[dialogId].SetData(GetNodeForId(dialogId));
//    }
//}

//public enum EDialogInputValue
//{
//    Next = -2,
//    Back = -1,
//}

    //public DialogNodeCanvas dialogCanvas;
    //public GameObject speechBubble;

    //public AudioClip talkSound;
    //public AudioClip skipSound;

    //public Color bubbleTint;
    //public Color textTint;

    //public MilleniumEvent pageFinishedEvent;
    //public MilleniumEvent bubbleClosedEvent;

    //private RectTransform uiParent;
    //private PlayerMachine player;
    //private GameObject currentBubble;

    //void Start () {
    //    uiParent = GameObject.FindGameObjectWithTag("UIParent").GetComponent<RectTransform>();
    //}

    //void pageFinished(int page){
    //    if(pageFinishedEvent != null){
    //        pageFinishedEvent.Invoke(gameObject, page.ToString());
    //    }
    //}

    //void bubbleClosed() {
    //    if (bubbleClosedEvent != null) {
    //        bubbleClosedEvent.Invoke(gameObject, null);
    //    }
    //    player.setCutsceneMode(false);
    //}

    //public override void interact(GameObject playerObject) {
    //    base.interact(playerObject);
    //    if (currentBubble == null) {
    //        GameObject bubble = Instantiate(speechBubble, uiParent);
    //        Typewriter writer = bubble.GetComponent<Typewriter>();
    //        player = playerObject.GetComponent<PlayerMachine>();
    //        writer.talkSound = talkSound;
    //        writer.skipSound = skipSound;
    //        writer.OnPageFinished += pageFinished;
    //        writer.OnBubbleClosed += bubbleClosed;
    //        writer.bubbleImage.color = bubbleTint;
    //        writer.bubbleText.color = textTint;
    //        //writer.StartWriting(text);
    //        currentBubble = bubble;
    //        player.setCutsceneMode(true);
    //    }
    //}