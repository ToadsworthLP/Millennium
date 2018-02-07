using System.Collections;
using UnityEngine;

//TODO rewrite this at some point since this is an ancient script
public class CameraController : MonoBehaviour {
    // The target we are following
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 10.0f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    // the target angle if lookAtTarget is disabled
    public Vector3 targetRotationAngle;
    // How much we want to damp
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;
    // If we want to look at the target transform
    public bool lookAtTarget = false;

    private float currentShakeMagnitude;
    private Vector3 cameraShakePosition;
    private Vector3 constantShakePosition;

    private Coroutine shakeCoroutine;

    private void Start() {
        StartCoroutine(ConstantShake());
    }

    private void LateUpdate() {
        // Early out if we don't have a target
        if (!target)
            return;

        // Set the position of the camera
        transform.position = GetTargetPosition();

        if (lookAtTarget) {
            transform.LookAt(target);
        }else{
            transform.rotation = GetTargetRotation();
        }

        currentShakeMagnitude = 0;
    }

    public Vector3 GetTargetPosition(){
        // Calculate the height values
        float wantedHeight = target.position.y + height;
        float currentHeight = transform.position.y;

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        Vector3 targetPosition;
        targetPosition = target.position;
        targetPosition -= Vector3.forward * distance;
        return new Vector3(targetPosition.x, currentHeight, targetPosition.z) + cameraShakePosition + constantShakePosition;
    }

    public Quaternion GetTargetRotation(){
        return Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotationAngle), rotationDamping * Time.deltaTime);
    }

    #region Camera Shaking
    //Found somewhere in my code snippets folder
    //I probably didn't write this

    public void ConstantShake(float magnitude) {
        currentShakeMagnitude = Mathf.Max(currentShakeMagnitude, magnitude);
    }

    public void Shake(float magnitude, float speed, float duration) {
        StopShake();
        shakeCoroutine = StartCoroutine(CameraShake(magnitude, speed, duration, true));
    }

    public void StopShake() {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
    }

    IEnumerator CameraShake(float magnitude, float speed, float duration, bool damp) {
        cameraShakePosition = Vector3.zero;

        Vector3 targetPosition = cameraShakePosition;

        float shakeStartTime = Time.time;

        while (!(Time.time > shakeStartTime + duration)) {
            cameraShakePosition = Vector3.MoveTowards(cameraShakePosition, targetPosition, speed * Time.deltaTime);

            float magModifier = 1.0f - Mathf.InverseLerp(shakeStartTime, shakeStartTime + duration, Time.time);

            if (cameraShakePosition == targetPosition) {
                targetPosition = Vector3.zero + Random.insideUnitSphere * magnitude * magModifier;
            }

            yield return 0;
        }

        while (cameraShakePosition != Vector3.zero) {
            cameraShakePosition = Vector3.MoveTowards(cameraShakePosition, Vector3.zero, speed * Time.deltaTime);

            yield return 0;
        }
    }

    IEnumerator ConstantShake() {
        constantShakePosition = Vector3.zero;

        Vector3 targetPosition = constantShakePosition;

        while (true) {
            if (currentShakeMagnitude != 0) {
                constantShakePosition = Vector3.MoveTowards(constantShakePosition, targetPosition, 10.0f * Time.deltaTime);

                if (constantShakePosition == targetPosition) {
                    targetPosition = Vector3.zero + Random.insideUnitSphere * currentShakeMagnitude;
                }
            } else {
                constantShakePosition = Vector3.MoveTowards(constantShakePosition, Vector3.zero, 10.0f * Time.deltaTime);
            }

            yield return 0;
        }
    }

    #endregion
}
