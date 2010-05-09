using UnityEngine;
using System.Collections;

public class ShowBoxColliders : MonoBehaviour
{
	// Debug display of our trigger state
	void OnDrawGizmos()
	{
	   // set to whatever color you want to represent
	   Gizmos.color = Color.blue;
	   
	   // we’re going to draw the gizmo in local space
	   Gizmos.matrix = transform.localToWorldMatrix;
	   
	   // draw a box collider based on its size
	   BoxCollider box = (BoxCollider)GetComponent( typeof( BoxCollider ) );
	   Gizmos.DrawWireCube( box.center, box.size );
	}
}
