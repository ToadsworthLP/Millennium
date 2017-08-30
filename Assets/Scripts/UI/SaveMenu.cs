using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : SelectableHelper {

    [TextArea]
    public string[] saveText;

    public GameObject speechBubble;
    public GameObject menuParent;
    public float fadeoutConstant;
    public float jumpDelay;

    private Transform uiParent;
    private PlayerMachine player;

    private bool active;

    private void Awake() {
        active = true;
        textComponent.canvasRenderer.SetColor(defaultColor);
    }

    private void bubbleClose(){
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        player.allowJumping = false;
        player.allowMovement = true;
        StartCoroutine(waitAndDestroy());
    }

    public override void onOKPressed() {
        uiParent = GameObject.FindGameObjectWithTag("UIParent").GetComponent<RectTransform>();
        Backpack backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
        backpack.saveData();
        GameObject bubble = Instantiate(speechBubble, uiParent);
        Typewriter writer = bubble.GetComponent<Typewriter>();
        writer.OnBubbleClosed += bubbleClose;
        writer.StartWriting(saveText);
        active = false;
        StartCoroutine(fadeOut());
    }

    IEnumerator waitAndDestroy(){
        yield return new WaitForSeconds(jumpDelay);
        player.allowJumping = true;
        Destroy(menuParent);
    }

    IEnumerator fadeOut() {
        CanvasRenderer[] canvasRenderers = menuParent.GetComponentsInChildren<CanvasRenderer>();

        while (canvasRenderers[0].GetAlpha() > 0) {
            foreach (CanvasRenderer i in canvasRenderers) {
                i.SetAlpha(i.GetAlpha() - fadeoutConstant);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public override bool getActive() {
        return active;
    }
}
