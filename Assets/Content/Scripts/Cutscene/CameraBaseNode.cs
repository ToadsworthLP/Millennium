using System;
using System.Collections;
using UnityEngine;

public abstract class CameraBaseNode : BaseCutsceneNode {

    public Camera camera;

    public InterpolationMode interpolationMode;
    public float panTime;

    public enum InterpolationMode { LERP, SMOOTH_DAMP, CURVE }

    [ShowIf("interpolationMode", InterpolationMode.CURVE)]
    [Tooltip("The curve to interpolate the camera position by. It should always end with a value of 1.")]
    public AnimationCurve customCurve;

    public IEnumerator MoveCamera(Transform camTransform, Vector3 targetPosition, Action nextStep) {
        var t = 0f; //Used by Lerp
        Vector3 startPosition = camTransform.position; //Used by Lerp
        Vector3 currentVelocity = new Vector3(); //Used by SmoothDamp

        if(interpolationMode == InterpolationMode.CURVE && customCurve.keys[customCurve.length-1].value != 1){
            interpolationMode = InterpolationMode.LERP;
            Debug.LogWarning("The last keyframe of your curve didn't have a value of 1, LERP has been forced as interpolation mode instead. Please correct it!");
        }

        while (camTransform.position != targetPosition) {
            switch (interpolationMode) {
                case InterpolationMode.LERP:
                    t += Time.deltaTime / panTime;
                    camTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
                    break;
                case InterpolationMode.SMOOTH_DAMP:
                    if (Vector3.Distance(camTransform.position, targetPosition) < 0.01f) {
                        camTransform.position = targetPosition;
                    } else {
                        camTransform.position = Vector3.SmoothDamp(camTransform.position, targetPosition, ref currentVelocity, panTime);
                    }
                    break;
                case InterpolationMode.CURVE:
                    t += Time.deltaTime / panTime;
                    camTransform.position = Utils.InterpolateByCurve(startPosition, targetPosition, t, customCurve);
                    break;
            }
            yield return new WaitForEndOfFrame();
        }

        nextStep.Invoke();
    }

    public IEnumerator RotateCamera(Transform camTransform, Quaternion targetRotation, Action nextStep) {
        var t = 0f; //Used by Lerp
        Quaternion startRotation = camTransform.rotation; //Used by Lerp
        Vector3 rotationVelocity = new Vector3(); //Used by SmoothDamp
        Vector3 targetRotationEuler = targetRotation.eulerAngles; //Used by SmoothDamp

        while (camTransform.rotation != targetRotation) {
            switch (interpolationMode) {
                case InterpolationMode.LERP:
                    t += Time.deltaTime / panTime;
                    camTransform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
                    break;
                case InterpolationMode.SMOOTH_DAMP:
                    Vector3 eulerAngles = camTransform.rotation.eulerAngles;
                    if (Vector3.Distance(eulerAngles, targetRotationEuler) < 0.01f) {
                        camTransform.rotation = Quaternion.Euler(targetRotationEuler);
                    } else {
                        eulerAngles.x = Mathf.SmoothDampAngle(eulerAngles.x, targetRotationEuler.x, ref rotationVelocity.x, panTime);
                        eulerAngles.y = Mathf.SmoothDampAngle(eulerAngles.y, targetRotationEuler.y, ref rotationVelocity.y, panTime);
                        eulerAngles.z = Mathf.SmoothDampAngle(eulerAngles.z, targetRotationEuler.z, ref rotationVelocity.z, panTime);

                        camTransform.rotation = Quaternion.Euler(eulerAngles);
                    }
                    break;
                case InterpolationMode.CURVE:
                    t += Time.deltaTime / panTime;
                    camTransform.rotation = Utils.InterpolateByCurve(startRotation, targetRotation, t, customCurve);
                    break;
            }
            yield return new WaitForEndOfFrame();
        }

        nextStep.Invoke();
    }

}
