using UnityEngine;


public delegate void GUIButtonTouchUpInside( GUISpriteButton sender );

public class GUISpriteButton : GUITouchableSprite
{
	public GUIButtonTouchUpInside action = null; // Delegate for when we get a touchUpInside
	
	private UVRect _normalUVframe; // Holds a copy of the uvFrame that the button was initialized with
	public UVRect highlightedUVframe;

	
	#region Constructors/Destructor
	
	public GUISpriteButton( Rect frame, int depth, UVRect uvFrame ):base( frame, depth, uvFrame )
	{
		// Save a copy of our uvFrame here so that when highlighting turns off we have the original UVs
		_normalUVframe = uvFrame;
		
		// If a highlighted frame has not yet been set use the normalUVframe
		if( highlightedUVframe == UVRect.zero )
			highlightedUVframe = uvFrame;
	}


	public GUISpriteButton( Rect frame, int depth, UVRect uvFrame, UVRect highlightedUVframe ):this( frame, depth, uvFrame )
	{
		this.highlightedUVframe = highlightedUVframe;
	}

	#endregion;


	// Sets the uvFrame of the original GUISprite and resets the _normalUVFrame for reference when highlighting
	public override UVRect uvFrame
	{
		get { return _uvFrame; }
		set
		{
			_uvFrame = value;
			_normalUVframe = value;
			manager.updateUV( this );
		}
	}

	
	public override bool highlighted
	{
		set
		{
			// Only set if it is different than our current value
			if( _highlighted != value )
			{			
				_highlighted = value;
				
				if ( value )
					base.uvFrame = highlightedUVframe;
				else
					base.uvFrame = _normalUVframe;
			}
		}
	}


	// Override transform() so we can mark the touchFrame as dirty
	public override void transform()
	{
		base.transform();
		
		touchFrameIsDirty = true;
	}


	// Touch handlers
	public override void onTouchEnded( Touch touch, Vector2 touchPos, bool touchWasInsideTouchFrame )
	{
		highlighted = false;
		
		// If the touch was inside our touchFrame and we have an action, call it
		if( touchWasInsideTouchFrame && action != null )
			action( this );
	}


}