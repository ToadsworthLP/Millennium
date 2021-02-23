using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class IntersceneLoadingZone : MonoBehaviour {

    public GameManager gameManager;

    public bool disabled;
    public string targetSceneName;
    public int destinationLoadingZoneIndex;
    public float loadingDelay = 0.2f;
    public float controlDelay = 1;
    public GameObject currentZoneParent;

    //The exit direction is slightly rounded to counter floating point errors. Make this number higher to improve accuracy
    private const int exitDirectionRoundingConstant = 100;

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !disabled) {
            StartCoroutine(HandleLoadingZone());
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && disabled) {
            StartCoroutine(DelayPlayerControl());
        }
    }

    public void PlayerArrives(){
        gameManager.playerMachine.SetCutsceneMode(true);
        gameManager.playerMachine.disableAngledControls = true;
        StartCoroutine(HandlePlayerArrival());
    }

    IEnumerator HandleLoadingZone() {
        gameManager.playerMachine.SetCutsceneMode(true);
        gameManager.playerMachine.disableAngledControls = true;

        Vector3 exitDirection = GetExitDirection(this);
        gameManager.playerGamepad.direction = new Vector2(Mathf.Round(exitDirection.x * exitDirectionRoundingConstant) / exitDirectionRoundingConstant, Mathf.Round(exitDirection.z * exitDirectionRoundingConstant) / exitDirectionRoundingConstant);
        gameManager.blackOverlay.FadeIn();
        gameManager.GetBackpack().targetEntranceId = destinationLoadingZoneIndex;

        gameManager.mainCamera.GetComponent<MusicManager>().FadeMusicVolume(0.2f, 0);

        yield return new WaitForSeconds(loadingDelay);

        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);

        while(!operation.isDone){
            yield return null;
        }
    }

    IEnumerator HandlePlayerArrival() {
        disabled = true;
        currentZoneParent.SetActive(true);
        gameManager.playerMachine.transform.position = transform.position;

        yield return new WaitForSeconds(loadingDelay);

        gameManager.blackOverlay.FadeOut();

        Vector3 exitDirection = GetExitDirection(this) * -1;
        gameManager.playerGamepad.direction = new Vector2(Mathf.Round(exitDirection.x * exitDirectionRoundingConstant) / exitDirectionRoundingConstant, Mathf.Round(exitDirection.z * exitDirectionRoundingConstant) / exitDirectionRoundingConstant);
    }

    IEnumerator DelayPlayerControl() {
        yield return new WaitForSeconds(controlDelay);
        gameManager.playerMachine.disableAngledControls = false;
        gameManager.playerMachine.SetCutsceneMode(false);
        disabled = false;
    }

    private Vector3 GetExitDirection(IntersceneLoadingZone zone) {
        return Vector3.Normalize(zone.transform.forward);
    }

    private void OnDrawGizmos() {
        Debug.DrawRay(transform.position, GetExitDirection(this), Color.blue);
    }
}
