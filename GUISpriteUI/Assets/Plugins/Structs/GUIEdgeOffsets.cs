using UnityEngine;

public struct GUIEdgeOffsets
{
	public int top;
	public int left;
	public int bottom;
	public int right;
	
	
	public GUIEdgeOffsets( int top, int left, int bottom, int right )
	{
		this.top = top;
		this.left = left;
		this.bottom = bottom;
		this.right = right;
	}


	// Used to expand or contract a rect by this
	public Rect addToRect( Rect frame )
	{
		// Clamp x and y to be greater than zero
		return new Rect
		(
			 Mathf.Clamp( frame.x - left, 0, 480 ),
			 Mathf.Clamp( frame.y - top, 0, 480 ),
			 frame.width + right + left,
			 frame.height + bottom + top
		);
	}
	
}
