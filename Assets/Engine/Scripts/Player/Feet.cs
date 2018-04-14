using UnityEngine;

public class Feet : MonoBehaviour {

    public float sphereCastRadius = 0.5f;
	public float groundCheckDistance = 0.1f;
    public Vector3 offset;

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + offset, sphereCastRadius);
        Gizmos.DrawWireSphere(transform.position + offset + transform.up * -1 * groundCheckDistance, sphereCastRadius);
    }

    public bool CheckGroundStatus(){
        RaycastHit hitInfo;

        bool didSphereCastHit = Physics.SphereCast(transform.position + offset, sphereCastRadius, Vector3.down, out hitInfo, groundCheckDistance);

        if (didSphereCastHit && hitInfo.transform.CompareTag("Ground"))
            return true;

        return false;
	}
}
