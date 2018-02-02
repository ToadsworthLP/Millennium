using System;
using System.Collections;
using UnityEngine;

public class CameraPanNode : BaseCutsceneNode
{

    public Camera camera;

    public float panSpeed;
    public float panDuration;

    private Vector3 cameraStartPos;
    private Quaternion cameraStartRot;
    private SmoothCameraMovement camMovementScript;

    private bool isOtherFinished;

    public override void CallNode() {
        Transform camTransform = camera.transform;
        camMovementScript = camera.gameObject.GetComponent<SmoothCameraMovement>();

        camMovementScript.enabled = false;

        cameraStartPos = camTransform.position;
        cameraStartRot = camTransform.rotation;

        StartCoroutine(MoveCamera(camTransform, transform.position, WaitForPanReached));
        StartCoroutine(RotateCamera(camTransform, transform.rotation, WaitForPanReached));
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

    private IEnumerator MoveCamera(Transform camTransform, Vector3 targetPosition, Action nextStep) {
        var t = 0f;
        Vector3 startPosition = camTransform.position;

        while(camTransform.position != targetPosition){
            t += Time.deltaTime / panSpeed;

            camTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return new WaitForEndOfFrame();
        }

        nextStep.Invoke();
    }

    private IEnumerator RotateCamera(Transform camTransform, Quaternion targetRotation, Action nextStep) {
        var t = 0f;
        Quaternion startRotation = camTransform.rotation;

        while (camTransform.rotation != targetRotation) {
            t += Time.deltaTime / panSpeed;

            camTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return new WaitForEndOfFrame();
        }

        nextStep.Invoke();
    }

    private void WaitForPanReached() {
        if(isOtherFinished){
            isOtherFinished = false;
            StartCoroutine(WaitBeforeReturning(panDuration));
        }else{
            isOtherFinished = true;
        }
    }

    private IEnumerator WaitBeforeReturning(float delay) {
        yield return new WaitForSeconds(delay);

        Transform camTransform = camera.transform;
        StartCoroutine(MoveCamera(camTransform, cameraStartPos, WaitForPanFinished));
        StartCoroutine(RotateCamera(camTransform, cameraStartRot, WaitForPanFinished));
    }

    private void WaitForPanFinished() {
        if (isOtherFinished) {
            isOtherFinished = false;
            camMovementScript.enabled = true;
            CallOutputSlot("Next Node");
        } else {
            isOtherFinished = true;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        base.OnDrawGizmos();
        Debug.DrawRay(transform.position, Vector3.Normalize(transform.forward), Color.blue);
    }
#endif

}
