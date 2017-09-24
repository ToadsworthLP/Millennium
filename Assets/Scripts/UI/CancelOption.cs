using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CancelOption : SelectableHelper {

    public GameObject menuParent;
    public float fadeoutConstant;
    public bool enablePlayerMovement;

    private PlayerMachine player;

    public override void onOKPressed() {
        if(enablePlayerMovement) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMachine>();
        }
        StartCoroutine(fadeOutMenu());
    }

    IEnumerator fadeOutMenu() {
        CanvasRenderer[] canvasRenderers = menuParent.GetComponentsInChildren<CanvasRenderer>();

        while (canvasRenderers[0].GetAlpha() > 0) {
            foreach (CanvasRenderer i in canvasRenderers) {
                i.SetAlpha(i.GetAlpha() - fadeoutConstant);
            }
            yield return new WaitForEndOfFrame();
        }

        player.setCutsceneMode(false);
        player.toggleFrozenStatus();
        Destroy(menuParent);
    }

}
