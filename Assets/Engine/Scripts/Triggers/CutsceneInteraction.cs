using System.Collections;
using UnityEngine;

public class CutsceneInteraction : InteractableHelper {

    public CutsceneManager cutscene;
    public float interactionCooldown = 0.5f;
    public bool interactable = true;

    public override void interact(GameObject playerObject) {
        if(interactable && !cutscene.isPlaying){
            interactable = false;
            base.interact(playerObject);
            cutscene.OnCutsceneFinished.AddListener(cutsceneFinished);
            cutscene.play();
        }
    }

    public void cutsceneFinished(GameObject src, object args){
        StartCoroutine(delayEnablingInteractions(interactionCooldown));
    }

    private IEnumerator delayEnablingInteractions(float delay){
        yield return new WaitForSeconds(delay);
        interactable = true;
    }
}
