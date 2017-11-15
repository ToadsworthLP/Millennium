using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CameraTurn : MonoBehaviour {

    public float targetRotation;
    public float turnTime;
    public float playerControlDelay;

    private SmoothCameraMovement camera;
    private Transform playerObject;
    private PlayerMachine player;
    private VirtualController input;
    private float turnVelocity;
    private Vector3 startRotation;

    private bool isTurning;

    void OnDisable() {
        playerObject.eulerAngles = startRotation;
        camera.lookAtTarget = false;
        camera.transform.eulerAngles = new Vector3(camera.transform.eulerAngles.x, playerObject.eulerAngles.y, camera.transform.eulerAngles.z);
        isTurning = false;
    }

    void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player") && !isTurning) {
            playerObject = other.transform;
            player = playerObject.GetComponent<PlayerMachine>();
            camera = player.gameManager.mainCamera.GetComponent<SmoothCameraMovement>();
            input = player.gameManager.controller;
            startRotation = playerObject.rotation.eulerAngles;

            isTurning = true;
            StartCoroutine(turnPlayer(targetRotation, false));
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && !isTurning) {
            isTurning = true;
            StartCoroutine(turnPlayer(-targetRotation, true));
        }
    }

    IEnumerator delayPlayerControl(){
        player.setCutsceneMode(true);
        player.disableAngledControls = true;
        Rigidbody playerRigidbody = playerObject.GetComponent<Rigidbody>();

        input.direction = Vector2.ClampMagnitude(new Vector2(playerRigidbody.velocity.x, playerRigidbody.velocity.z), 1);

        yield return new WaitForSeconds(playerControlDelay);

        player.disableAngledControls = false;
        player.setCutsceneMode(false);
    }

    IEnumerator turnPlayer(float targetAngle, bool turnBack){
        float absoluteAngle = Mathf.Round(playerObject.eulerAngles.y + targetAngle);
        camera.lookAtTarget = true;

        StartCoroutine(delayPlayerControl());

        while (Mathf.Round(playerObject.eulerAngles.y) != absoluteAngle) {
            /*First line: Uses smooth damp, second line: lerp
              Uncomment the one you like the most*/
            float currentAngle = Mathf.SmoothDamp(playerObject.eulerAngles.y, absoluteAngle, ref turnVelocity, turnTime);
            //float currentAngle = Mathf.Lerp(playerObject.eulerAngles.y, absoluteAngle, turnTime);

            playerObject.eulerAngles = new Vector3(0, currentAngle);
            yield return new WaitForEndOfFrame();
        }

        if(turnBack){
            playerObject.eulerAngles = startRotation;
        }else{
            playerObject.eulerAngles = startRotation + new Vector3(0, absoluteAngle, 0);
        }

        StartCoroutine(waitForCamera(turnBack));
        isTurning = false;
    }

    IEnumerator waitForCamera(bool turnBack){
        //Values multiplied by 100 before rounding to improve accuracy, make it higher to make it even more accurate, but exact comparison will always fail
        while (Mathf.Round(Mathf.Abs(camera.transform.rotation.eulerAngles.y) * 100) != Mathf.Round(Mathf.Abs(playerObject.eulerAngles.y) * 100)) {
            yield return new WaitForEndOfFrame();
        }

        camera.lookAtTarget = false;
    }
}
