using UnityEngine;

public class SaveMenu : MonoBehaviour, ISelectable {

    [TextArea]
    public string[] yesText;
    [TextArea]
    public string[] noText;

    public RectTransform grabPoint;
    public GameObject speechBubble;
    public GameObject menuParent;
    public bool saveOption;

    private Transform uiParent;

    public Vector3 getGrabPoint() {
        return grabPoint.position;
    }

    public void onCancelPressed() {}

    public void onCursorLeave() {}

    public void onCursorSelect() {}

    private void bubbleClose(){
        PlayerMachine player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        player.allowMovement = true;
    }

    public void onOKPressed() {
        uiParent = GameObject.FindGameObjectWithTag("UIParent").GetComponent<RectTransform>();
        if (saveOption){
            Backpack backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
            backpack.saveData();
            GameObject bubble = Instantiate(speechBubble, uiParent);
            Typewriter writer = bubble.GetComponent<Typewriter>();
            writer.OnBubbleClosed += bubbleClose;
            writer.StartWriting(yesText);
        } else{
            GameObject bubble = Instantiate(speechBubble, uiParent);
            Typewriter writer = bubble.GetComponent<Typewriter>();
            writer.OnBubbleClosed += bubbleClose;
            writer.StartWriting(noText);
        }
        Destroy(menuParent);
    }

}
