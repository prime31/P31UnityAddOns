/**
* Show some debug info about our rig
*/
function OnDrawGizmos()
{
	var rbs = GetComponentsInChildren(Rigidbody);
	
	// show our center of gravity in red
	for( var rb:Rigidbody in rbs )
	{
		Gizmos.color = Color.red;
		
		var centerOfMass = rb.transform.TransformPoint(rb.centerOfMass);
		Gizmos.DrawSphere(centerOfMass, 0.02);
		Gizmos.DrawLine(rb.transform.position, rb.transform.TransformPoint(rb.centerOfMass));
		Gizmos.DrawCube(rb.transform.position, Vector3.one * 0.01);
	}
}