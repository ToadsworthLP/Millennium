using System.Collections;
using UnityEngine;

public class CameraShakeNode : BaseCutsceneNode
{
    public CameraController camera;
    public float shakeDuration = 1f;
    public float shakeSpeed = 15f;
    public float shakeMagnitude = 0.1f;
    public bool waitForShakingFinished;

    public override void CallNode() {
        camera.Shake(shakeMagnitude, shakeSpeed, shakeDuration);
        if (waitForShakingFinished){
            StartCoroutine(WaitUntilFinishedShaking());
        }else{
            CallOutputSlot("Next Node");
        }
    }

    public override void DeclareOutputSlots() {
        SetOutputSlot("Next Node");
    }

    private IEnumerator WaitUntilFinishedShaking() {
        yield return new WaitForSeconds(shakeDuration);
        CallOutputSlot("Next Node");
    }

}
