using UnityEngine;

public class SpinningTransform : MonoBehaviour {

    public Transform target;
    public float spinSpeed;

    private Vector3 upwards;

    private void Start() {
        upwards = transform.up;
    }

    private void Update() {
        target.rotation *= Quaternion.AngleAxis(spinSpeed * Time.deltaTime, upwards);
    }
}
