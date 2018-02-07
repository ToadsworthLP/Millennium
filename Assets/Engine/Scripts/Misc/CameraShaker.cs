using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class CameraShaker : MonoBehaviour {

    public float maximumMagnitude = 0.2f;
    public CameraController mainCamera;
    [HideInInspector]
    public SphereCollider sphereCollider;


    private float maximumShakeDistance;

    private void Awake() {
        sphereCollider = GetComponent<SphereCollider>();

        if(!sphereCollider.isTrigger){
            Debug.LogError("The sphere collider used for marking camera shaker boundaries must be marked as trigger!");
        }

        maximumShakeDistance = sphereCollider.radius * 2;
    }

    private void OnTriggerStay(Collider other) {
        if(other.transform.Equals(mainCamera.target)){
            float distance = Vector3.Distance(mainCamera.target.position, transform.TransformPoint(sphereCollider.center));

            float magnitude = Mathf.Lerp(maximumMagnitude, 0, Mathf.InverseLerp(0, maximumShakeDistance, distance));
            mainCamera.ConstantShake(magnitude);
        }
    }

}