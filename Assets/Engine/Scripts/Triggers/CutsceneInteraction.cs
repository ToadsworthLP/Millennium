using System.Collections;
using UnityEngine;

public class CutsceneInteraction : InteractableHelper {

    public CutsceneManager cutscene;
    public float interactionCooldown = 0.5f;
    public bool interactable = true;
    private Coroutine inputEnableCoroutine;

    public override void Interact(GameObject playerObject) {
        base.Interact(playerObject);
        if(interactable && !cutscene.isPlaying){
            interactable = false;
            base.Interact(playerObject);
            cutscene.OnCutsceneFinished.AddListener(CutsceneFinished);
            cutscene.Play();
        }
    }

    public void CutsceneFinished(GameObject src, object args){
        if(inputEnableCoroutine == null){
            inputEnableCoroutine = StartCoroutine(DelayEnablingInteractions(interactionCooldown));
        }
    }

    private IEnumerator DelayEnablingInteractions(float delay){
        yield return new WaitForSeconds(delay);
        interactable = true;
        inputEnableCoroutine = null;
    }
}
