using UnityEngine;
using System;


public abstract class GUITouchableSprite : GUISprite, IComparable
{
	public int touchCount;
	
	protected GUIEdgeOffsets _normalTouchOffsets;
	protected GUIEdgeOffsets _highlightedTouchOffsets;
	protected Rect _highlightedTouchFrame;
	protected Rect _normalTouchFrame;
	
	protected bool touchFrameIsDirty = true; // Indicates if the touchFrames need to be recalculated
	
	protected bool _highlighted;
	
	
	public GUITouchableSprite( Rect frame, int depth, UVRect uvFrame ):base( frame, depth, uvFrame )
	{
	}

	
	#region Properties and Getters/Setters

	// Adds or subtracts from the frame of the button to define a hit area
	public GUIEdgeOffsets highlightedTouchOffsets
	{
		get { return _highlightedTouchOffsets; }
		set
		{
			_highlightedTouchOffsets = value;
			touchFrameIsDirty = true;
		}
	}


	// Adds or subtracts from the frame of the button to define a hit area
	public GUIEdgeOffsets normalTouchOffsets
	{
		get { return _normalTouchOffsets; }
		set
		{
			_normalTouchOffsets = value;
			touchFrameIsDirty = true;
		}
	}


	// Returns a frame to use to see if this element was touched
	protected virtual Rect touchFrame
	{
		get
		{
			// If the frame is dirty, recalculate it
			if( touchFrameIsDirty )
			{
				touchFrameIsDirty = false;
				
				// Grab the normal frame of the sprite then add the offsets to get our touch frames
				Rect normalFrame = new Rect( clientTransform.position.x, -clientTransform.position.y, width, height );
				_normalTouchFrame = _normalTouchOffsets.addToRect( normalFrame );
				_highlightedTouchFrame = _highlightedTouchOffsets.addToRect( normalFrame );
			}
			
			// Either return our highlighted or normal touch frame
			return ( _highlighted ) ? _highlightedTouchFrame : _normalTouchFrame;
		}
	}
	
	#endregion;


	// Tests if a point is inside the current touchFrame
	public bool hitTest( Vector2 point )
	{
		return touchFrame.Contains( point );
	}

	
	// Indicates if there is a finger over this element
	public virtual bool highlighted
	{
		get { return _highlighted; }
		set { _highlighted = value;	}
	}


	// Transforms a point to local coordinates (origin is top left)
	protected Vector2 inverseTranformPoint( Vector2 point )
	{
		return new Vector2( point.x - _normalTouchFrame.xMin, point.y - _normalTouchFrame.yMin );
	}


	#region Touch handlers
	
	// Touch handlers.  Subclasses should override these to get their specific behaviour
	public virtual void onTouchBegan( Vector2 touchPos )
	{
		highlighted = true;
	}

	
	public virtual void onTouchMoved( Vector2 touchPos )
	{

	}
	
	
	public virtual void onTouchEnded( Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		highlighted = false;
	}

	#endregion;
	

    // IComparable - sorts based on the z value of the client
	public int CompareTo( object obj )
    {
        if( obj is GUITouchableSprite )
        {
            GUITouchableSprite temp = obj as GUITouchableSprite;
            return this.clientTransform.position.z.CompareTo( temp.clientTransform.position.z );
        }
		
		return -1;
    }

}

