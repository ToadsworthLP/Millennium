using UnityEngine;

public class Feet : MonoBehaviour {

    public float sphereCastRadius = 0.5f;
	public float groundCheckDistance = 0.1f;
    public float yOffset;

	public bool CheckGroundStatus(){
        RaycastHit hitInfo;

        bool didSphereCastHit = Physics.SphereCast(transform.position + new Vector3(0f, yOffset, 0f), sphereCastRadius, Vector3.down, out hitInfo, groundCheckDistance);

        if (didSphereCastHit && hitInfo.transform.CompareTag("Ground"))
            return true;

        return false;
	}
}
