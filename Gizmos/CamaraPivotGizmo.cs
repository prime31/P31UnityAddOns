using UnityEngine;
using System.Collections;

public class CamaraPivotGizmo : MonoBehaviour
{
	// Debug display of our trigger state
	void OnDrawGizmos()
	{
	   // set to whatever color you want to represent
	   Gizmos.color = Color.yellow;
	   
	   // we’re going to draw the gizmo in local space
	   Gizmos.matrix = transform.localToWorldMatrix;
	   
		Gizmos.DrawWireSphere( Vector3.zero, 0.2f );
	}
}
