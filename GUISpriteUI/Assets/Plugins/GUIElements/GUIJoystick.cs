using System;
using UnityEngine;


struct GUIBoundary 
{
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;
	
	public static GUIBoundary boundaryFromPoint( Vector2 point, float maxDistance )
	{
		GUIBoundary boundary = new GUIBoundary();
		
		boundary.minX = point.x - maxDistance;
		boundary.maxX = point.x + maxDistance;
		boundary.minY = point.y - maxDistance;
		boundary.maxY = point.y + maxDistance;
		
		return boundary;
	}
}


public class GUIJoystick : GUITouchableSprite
{
	public Vector2 position; // x and y offset of the joystick always between 1 and -1
	public Vector2 deadZone = Vector2.zero; // Controls when position output occurs
	public bool normalize; // Normalize output after the dead-zone?
	
	private UVRect _normalUVframe; // Holds a copy of the uvFrame that the button was initialized with
	public UVRect highlightedUVframe = UVRect.zero; // Highlighted UV's for the joystick
	
	private GUISprite _joystickSprite; // sprite to use for the joystick.  Automatically added to the GUISpriteUI.
	private Vector3 _joystickOffset;
	private GUIBoundary _joystickBoundary;
	private float _maxJoystickMovement = 50.0f; // max distance from _joystickOffset that the joystick will move
	
	
	// frame - the screen area that you want to listen to touches in
	// uvFrame - set to UVRect.zero unless you want a portion of the texture mapped to the entire touch area
	// joystickOffset - the offset from the top-left point in the frame (local coordinates) to the center of the joystick
	public GUIJoystick( Rect frame, int depth, UVRect uvFrame, GUISprite joystickSprite, Vector2 joystickOffset ):base( frame, depth, uvFrame )
	{
		// Add the joystickSprite to the manager
		manager.addSprite( joystickSprite );
		
		// Save out the uvFrame for the sprite so we can highlight
		_normalUVframe = joystickSprite.uvFrame;
		
		// Save the joystickSprite and make it a child of the us for organization purposes
		_joystickSprite = joystickSprite;
		_joystickSprite.clientTransform.parent = this.clientTransform;
		
		// Move the joystick to its default position after converting the offset to a vector3
		_joystickOffset = new Vector3( joystickOffset.x, joystickOffset.y );
		
		// Set the maxMovement which will in turn calculate the _joystickBoundary
		this.maxJoystickMovement = _maxJoystickMovement;
		
		resetJoystick();
	}
	
	
	// constructor that sets common defaults and calls the default constructor
	public GUIJoystick( Rect frame, GUISprite joystickSprite, Vector2 joystickOffset ):this( frame, 1, UVFrame.zero, joystickSprite, joystickOffset )
	{
		
	}
	
	
	public float maxJoystickMovement
	{
		get { return _maxJoystickMovement; }
		set
		{
			_maxJoystickMovement = value;
			_joystickBoundary = GUIBoundary.boundaryFromPoint( _joystickOffset, _maxJoystickMovement );
		}
	}
	
	
	public void addBackgroundSprite( Rect frame, int depth, UVRect uvFrame )
	{
		GUISprite sprite = new GUISprite( frame, depth, uvFrame, true );
		sprite.clientTransform.parent = this.clientTransform;
		GUISpriteUI.instance.addSprite( sprite );
		sprite.clientTransform.localPosition = new Vector3( _joystickOffset.x, _joystickOffset.y, depth );
		sprite.transform();
	}
	
	
	// Resets the sprite to default position and zeros out the position vector
	private void resetJoystick()
	{
		_joystickSprite.clientTransform.localPosition = _joystickOffset;
		_joystickSprite.transform();
		position.x = position.y = 0.0f;
		
		// If we have a highlightedUVframe, swap the original back in
		if( highlightedUVframe != UVRect.zero )
			_joystickSprite.uvFrame = _normalUVframe;
	}
	
	
	private void layoutJoystick( Vector2 localTouchPosition )
	{
		// Clamp the new position based on the boundaries we have set.  Dont forget to reverse the Y axis!
		Vector3 newPosition = Vector3.zero;
		newPosition.x = Mathf.Clamp( localTouchPosition.x, _joystickBoundary.minX, _joystickBoundary.maxX );
		newPosition.y = Mathf.Clamp( -localTouchPosition.y, _joystickBoundary.minY, _joystickBoundary.maxY );
		
		// Set the new position and update the transform		
		_joystickSprite.clientTransform.localPosition = newPosition;
		_joystickSprite.transform();
		
		// Get a value between -1 and 1 for position
		position.x = ( newPosition.x - _joystickOffset.x ) / _maxJoystickMovement;
		position.y = ( newPosition.y - _joystickOffset.y ) / _maxJoystickMovement;
		
		// Adjust for dead zone	
		float absoluteX = Mathf.Abs( position.x );
		float absoluteY = Mathf.Abs( position.y );
	
		if( absoluteX < deadZone.x )
		{
			// Report the joystick as being at the center if it is within the dead zone
			position.x = 0;
		}
		else if( normalize )
		{
			// Rescale the output after taking the dead zone into account
			position.x = Mathf.Sign( position.x ) * ( absoluteX - deadZone.x ) / ( 1 - deadZone.x );
		}
		
		if( absoluteY < deadZone.y )
		{
			// Report the joystick as being at the center if it is within the dead zone
			position.y = 0;
		}
		else if( normalize )
		{
			// Rescale the output after taking the dead zone into account
			position.y = Mathf.Sign( position.y ) * ( absoluteY - deadZone.y ) / ( 1 - deadZone.y );
		}
	}
	
	
	public override void onTouchBegan( Touch touch, Vector2 touchPos )
	{
		highlighted = true;
		
		this.layoutJoystick( this.inverseTranformPoint( touchPos ) );
		
		// If we have a highlightedUVframe, swap it in
		if( highlightedUVframe != UVRect.zero )
			_joystickSprite.uvFrame = highlightedUVframe;
	}
	
	
	public override void onTouchMoved( Touch touch, Vector2 touchPos )
	{
		this.layoutJoystick( this.inverseTranformPoint( touchPos ) );
	}
	
	
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		// Set highlighted to avoid calling super
		this.highlighted = false;
		
		// Reset back to default state
		this.resetJoystick();
	}
	
}


