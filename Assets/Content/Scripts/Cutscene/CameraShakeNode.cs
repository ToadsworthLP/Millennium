using System.Collections;
using UnityEngine;

public class CameraShakeNode : BaseCutsceneNode
{
    public CameraController camera;
    public float shakeDuration;
    public float shakeMagnitude;
    public bool waitForShakingFinished;

    public override void CallNode() {
        camera.Shake(shakeMagnitude, 10f, shakeDuration);
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
