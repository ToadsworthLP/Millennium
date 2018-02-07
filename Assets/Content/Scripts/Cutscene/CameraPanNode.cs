using System.Collections;
using UnityEngine;

public class CameraPanNode : CameraBaseNode
{
    public float stayDuration;
    public bool goBack = true;

    private Vector3 cameraStartPos;
    private Quaternion cameraStartRot;
    private CameraController camMovementScript;

    private bool isOtherFinished;

    public override void CallNode() {
        Transform camTransform = camera.transform;
        camMovementScript = camera.gameObject.GetComponent<CameraController>();

        camMovementScript.enabled = false;

        cameraStartPos = camTransform.position;
        cameraStartRot = camTransform.rotation;

        if(goBack){
            StartCoroutine(MoveCamera(camTransform, transform.position, WaitForPanReached));
            StartCoroutine(RotateCamera(camTransform, transform.rotation, WaitForPanReached));
        } else{
            StartCoroutine(MoveCamera(camTransform, transform.position, WaitForPanFinished));
            StartCoroutine(RotateCamera(camTransform, transform.rotation, WaitForPanFinished));
        }
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

    private void WaitForPanReached() {
        if(isOtherFinished){
            isOtherFinished = false;
            StartCoroutine(WaitBeforeReturning(stayDuration));
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
            
            if(goBack)
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
