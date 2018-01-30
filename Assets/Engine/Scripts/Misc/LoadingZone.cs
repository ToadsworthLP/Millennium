using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class LoadingZone : MonoBehaviour {

    public GameManager gameManager;

    public bool disabled;
    public float loadingDelay = 0.3f;
    public float controlDelay = 0.2f;
    public GameObject currentZoneParent;
    public GameObject destinationZoneParent;
    public LoadingZone destinationLoadingZone;

    //The exit direction is slightly rounded to counter floating point errors. Make this number higher to improve accuracy
    private const int exitDirectionRoundingConstant = 100;

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && !disabled){
            StartCoroutine(handleLoadingZone());
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && disabled) {
            StartCoroutine(delayPlayerControl());
        }
    }

    IEnumerator handleLoadingZone() {
        gameManager.playerMachine.setCutsceneMode(true);
        gameManager.playerMachine.disableAngledControls = true;

        Vector3 exitDirection = getExitDirection(this);
        gameManager.controller.direction = new Vector2(Mathf.Round(exitDirection.x * exitDirectionRoundingConstant) / exitDirectionRoundingConstant, Mathf.Round(exitDirection.z * exitDirectionRoundingConstant) / exitDirectionRoundingConstant);
        gameManager.blackOverlay.FadeIn();

        yield return new WaitForSeconds(loadingDelay);
        currentZoneParent.SetActive(false);
        destinationZoneParent.SetActive(true);
        destinationLoadingZone.disabled = true;
        gameManager.playerMachine.transform.position = destinationLoadingZone.transform.position;

        exitDirection = getExitDirection(destinationLoadingZone)*-1;
        gameManager.controller.direction = new Vector2(Mathf.Round(exitDirection.x * exitDirectionRoundingConstant) / exitDirectionRoundingConstant, Mathf.Round(exitDirection.z * exitDirectionRoundingConstant) / exitDirectionRoundingConstant);
        gameManager.blackOverlay.FadeOut();
    }

    IEnumerator delayPlayerControl() {
        yield return new WaitForSeconds(controlDelay);
        gameManager.playerMachine.disableAngledControls = false;
        gameManager.playerMachine.setCutsceneMode(false);
        disabled = false;
    }

    private Vector3 getExitDirection(LoadingZone zone) {
        return Vector3.Normalize(zone.transform.forward);
    }

    private void OnDrawGizmos() {
        Debug.DrawRay(transform.position, getExitDirection(this), Color.blue);
    }

}
