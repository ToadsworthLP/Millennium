using UnityEngine;

//TODO rewrite this at some point since this is an ancient script
public class SmoothCameraMovement : MonoBehaviour {
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

    void LateUpdate() {
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
        return new Vector3(targetPosition.x, currentHeight, targetPosition.z);
    }

    public Quaternion GetTargetRotation(){
        return Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotationAngle), rotationDamping * Time.deltaTime);
    }
}
