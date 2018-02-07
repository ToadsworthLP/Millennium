using UnityEngine;

public class CameraResetNode : CameraBaseNode
{
    private CameraController camMovementScript;
    private bool isOtherFinished;

    public override void CallNode() {
        Transform camTransform = camera.transform;
        camMovementScript = camera.gameObject.GetComponent<CameraController>();

        camMovementScript.isControllerActive = false;

        StartCoroutine(MoveCamera(camTransform, camMovementScript.GetTargetPosition(), WaitForPanFinished));
        StartCoroutine(RotateCamera(camTransform, camMovementScript.GetTargetRotation(), WaitForPanFinished));

    }

    private void WaitForPanFinished() {
        if (isOtherFinished) {
            isOtherFinished = false;
            camMovementScript.isControllerActive = true;

            CallOutputSlot("Next Node");
        } else {
            isOtherFinished = true;
        }
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }
}
