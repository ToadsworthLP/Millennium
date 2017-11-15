using UnityEngine;

//TODO rewrite this at some point since this is an ancient script
public class SmoothCameraMovement : MonoBehaviour {
    // The target we are following
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 10.0f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    // the target angle on the X axis if lookAtTarget is disabled
    public float xTargetAngle;
    // How much we want to damp
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;
    // If we want to look at the target transform
    public bool lookAtTarget = false;

    void LateUpdate() {
        // Early out if we don't have a target
        if (!target)
            return;

        // Calculate the current rotation angles
        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;
        float currentRotationAngleY = transform.eulerAngles.y;
        float currentRotationAngleX = transform.eulerAngles.x;
        float currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngleY = Mathf.LerpAngle(currentRotationAngleY, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngleY, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = target.position;
        transform.position -= currentRotation * Vector3.forward * distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        if (lookAtTarget) {
            transform.LookAt(target);
        }else{
            float angle = Mathf.LerpAngle(currentRotationAngleX, xTargetAngle, rotationDamping * Time.deltaTime);
            transform.eulerAngles = new Vector3(angle, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
