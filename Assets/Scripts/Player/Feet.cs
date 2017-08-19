using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour {

	public float groundCheckDistance = 0.1f;
	public float sideOffset = 0.2f;

	public bool CheckGroundStatus()
	{
		RaycastHit hitInfo;
		#if UNITY_EDITOR
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f) + new Vector3(sideOffset,0,0), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance) + new Vector3(sideOffset,0,0));
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f) - new Vector3(sideOffset,0,0), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance) - new Vector3(sideOffset,0,0));
		#endif
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f) + new Vector3(sideOffset,0,0), Vector3.down, out hitInfo, groundCheckDistance) ||
			Physics.Raycast(transform.position + (Vector3.up * 0.1f) - new Vector3(sideOffset,0,0), Vector3.down, out hitInfo, groundCheckDistance))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
