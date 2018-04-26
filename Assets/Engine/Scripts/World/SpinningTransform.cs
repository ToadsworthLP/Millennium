using UnityEngine;

public class SpinningTransform : MonoBehaviour {

    public Transform target;
    public float spinSpeed;

    void Update () {
        target.rotation *= Quaternion.AngleAxis(spinSpeed * Time.deltaTime, transform.up);
    }
}
