using UnityEngine;

public class CameraResetNode : CameraBaseNode
{
    private SmoothCameraMovement camMovementScript;
    private bool isOtherFinished;

    public override void CallNode() {
        Transform camTransform = camera.transform;
        camMovementScript = camera.gameObject.GetComponent<SmoothCameraMovement>();

        camMovementScript.enabled = false;

        StartCoroutine(MoveCamera(camTransform, camMovementScript.GetTargetPosition(), WaitForPanFinished));
        StartCoroutine(RotateCamera(camTransform, camMovementScript.GetTargetRotation(), WaitForPanFinished));

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

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }
}
