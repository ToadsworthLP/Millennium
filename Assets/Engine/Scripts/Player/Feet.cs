using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour {

	public float groundCheckDistance = 0.1f;
	public float sideOffset = 0.2f;

	public bool CheckGroundStatus()
	{
		RaycastHit hitInfo1;
        RaycastHit hitInfo2;

		#if UNITY_EDITOR
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f) + new Vector3(sideOffset,0,0), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance) + new Vector3(sideOffset,0,0));
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f) - new Vector3(sideOffset,0,0), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance) - new Vector3(sideOffset,0,0));
        #endif

        bool didRay1Hit = Physics.Raycast(transform.position + (Vector3.up * 0.1f) + new Vector3(sideOffset, 0, 0), Vector3.down, out hitInfo1, groundCheckDistance);
        bool didRay2Hit = Physics.Raycast(transform.position + (Vector3.up * 0.1f) - new Vector3(sideOffset, 0, 0), Vector3.down, out hitInfo2, groundCheckDistance);

        if ((didRay1Hit && hitInfo1.transform.CompareTag("Ground") || (didRay2Hit && hitInfo2.transform.CompareTag("Ground")))){
			return true;
		}
		else{
			return false;
		}
	}
}
