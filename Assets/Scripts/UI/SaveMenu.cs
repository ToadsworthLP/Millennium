using System.Collections;
using UnityEngine;

public class SaveMenu : SelectableHelper {

    [TextArea]
    public string[] saveText;
    [TextArea]
    public string[] errorText;

    public GameObject speechBubble;
    public GameObject menuParent;
    public float fadeoutConstant;
    public float jumpDelay;

    private Transform uiParent;
    private PlayerMachine player;

    private bool active;

    public override void onCursorInit(Cursor cursor) {
        base.onCursorInit(cursor);
        active = true;
        textComponent.canvasRenderer.SetColor(defaultColor);
    }

    private void bubbleClose(){
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        player.setCutsceneMode(false);
        player.allowJumping = false;
        player.setFrozenStatus(false);
        StartCoroutine(waitAndDestroy());
    }

    public override void onOKPressed() {
        uiParent = GameObject.FindGameObjectWithTag("UIParent").GetComponent<RectTransform>();
        Backpack backpack = GameObject.FindGameObjectWithTag("Backpack").GetComponent<Backpack>();
        bool success = backpack.saveData();
        GameObject bubble = Instantiate(speechBubble, uiParent);
        Typewriter writer = bubble.GetComponent<Typewriter>();
        writer.OnBubbleClosed += bubbleClose;

        if (success){
            writer.StartWriting(saveText);
        } else{
            writer.StartWriting(errorText);
        }
        
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
