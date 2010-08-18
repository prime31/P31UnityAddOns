using System;
using UnityEngine;
using System.Collections.Generic;


public class GUISpriteUI : GUISpriteManager
{
	// All access should go through instance
	static public GUISpriteUI instance = null;
	
	public const int MAX_CHANGED_TOUCHES = 4;
	
	public int drawDepth = 100;	
	public LayerMask UILayer = 0;
	public int layer;
	
	private Camera _uiCamera;
	private GameObject _uiCameraHolder;
	private GUITouchableSprite[] _spriteSelected;
	private Vector2 _screenResolution;
	
	// Holds all our touchable sprites
	private List<GUITouchableSprite> _touchableSprites = new List<GUITouchableSprite>();

	
	#region Unity MonoBehaviour Functions
	
	protected override void Awake()
	{
		base.Awake();
		
		// Set instance to this so we can be accessed from anywhere
		instance = this;
		
		iPhoneSettings.screenOrientation = iPhoneScreenOrientation.LandscapeLeft;
		
		// Create the camera
		_uiCameraHolder = new GameObject();
		_uiCameraHolder.AddComponent( "Camera" );
		_uiCamera = _uiCameraHolder.camera;
		_uiCamera.name = "__UICamera";
		_uiCamera.clearFlags = CameraClearFlags.Depth;
		_uiCamera.nearClipPlane = 0.3f;
		_uiCamera.farClipPlane = 100.0f;
		_uiCamera.depth = drawDepth;
		_uiCamera.rect = new Rect( 0.0f, 0.0f, 1.0f, 1.0f );
		_uiCamera.orthographic = true;
		
		if ( iPhoneSettings.screenOrientation == iPhoneScreenOrientation.Landscape )
		{
			//_screenResolution = new Vector2( 480.0f, 320.0f );
			//_uiCamera.orthographicSize = 160.0f;
			_screenResolution = new Vector2( Screen.width, Screen.height );
			_uiCamera.orthographicSize = Screen.height / 2;
		}
		else
		{
			_screenResolution = new Vector2( 320.0f, 480.0f );
			_uiCamera.orthographicSize = 240.0f;
		}

		// Set the camera position based on the screenResolution/orientation
		_uiCamera.transform.position = new Vector3( _screenResolution.x / 2, -_screenResolution.y / 2, -10.0f );
		_uiCamera.cullingMask = UILayer;
		
		// Cache the layer for later use when adding sprites
		// UILayer.value is a mask, find which bit is set 
		for( int i = 0; i < sizeof( int ) * 8; i++ )
		{
			if( ( UILayer.value & (1 << i) ) == (1 << i) )
			{
				layer = i;
				break;
			}
		}

		_spriteSelected = new GUITouchableSprite[MAX_CHANGED_TOUCHES];
		for( int i = 0; i < MAX_CHANGED_TOUCHES; ++i )
			_spriteSelected[i] = null;
	}


	protected void Update()
	{
		// Only do our touch processing if we have some touches
		if( Input.touchCount > 0 )
		{
			// Examine all current touches
			for( int i = 0; i < Input.touchCount; i++ )
				lookAtTouch( Input.GetTouch( i ) );
		}

		// Take care of updating our UVs, colors or bounds if necessary
		if( meshIsDirty )
		{
			meshIsDirty = false;
			updateMeshProperties();
		}
	}

	
	// Ensure that the instance is destroyed when the game is stopped in the editor.
	public void OnApplicationQuit()
	{
	   instance = null;
	}

	#endregion;


	#region Add Element and Button functions
	
	// Shortcut for adding a GUISpriteButton
	public GUISpriteButton addSpriteButton( Rect frame, int depth, UVRect uvFrame )
	{
		GUISpriteButton spriteButton = new GUISpriteButton( frame, depth, uvFrame );
		addTouchableSprite( spriteButton );
		
		return spriteButton;
	}

	
	public void addTouchableSprite( GUITouchableSprite touchableSprite )
	{
		addSprite( touchableSprite );
		
		// Add the sprite to our touchables and sort them		
		_touchableSprites.Add( touchableSprite );
		_touchableSprites.Sort();
	}
	
	#endregion;
	
	
	// Removes a sprite or touchableSprite
	public void removeElement( GUISprite sprite )
	{
		// If we are removing a GUITouchableSprite remove it from our internal array as well
		if( sprite is GUITouchableSprite )
			_touchableSprites.Remove( sprite as GUITouchableSprite );

		removeSprite( sprite );
	}

	
	#region Touch management and analysis helpers
	
	// Examines a touch and sends off began, moved and ended events
	private void lookAtTouch( Touch touch )
	{
		// Tranform the touch position so the origin is in the top left
		Vector2 fixedTouchPosition = new Vector2( touch.position.x, _screenResolution.y - touch.position.y );
		GUITouchableSprite button = getButtonForScreenPosition( fixedTouchPosition );

		bool touchEnded = ( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled );
		
		if( touch.phase == TouchPhase.Began )
		{
			if( button != null )
			{
				_spriteSelected[touch.fingerId] = button;
				button.onTouchBegan( touch, fixedTouchPosition );
			}
			else
			{
				// Deselect any selected sprites for this touch
				_spriteSelected[touch.fingerId] = null;
			}
		}
		else if( touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary )
		{
			if( button != null && _spriteSelected[touch.fingerId] == button )
			{
				// If we have a moving touch on a sprite keep sending touchMoved
				if( touch.phase == TouchPhase.Moved )
					_spriteSelected[touch.fingerId].onTouchMoved( touch, fixedTouchPosition );
			}
			else if( _spriteSelected[touch.fingerId] != null )
			{
				// If we have a button that isn't the selected button end the touch on it because we moved off of it
				_spriteSelected[touch.fingerId].onTouchEnded( touch, fixedTouchPosition, false );
				_spriteSelected[touch.fingerId] = null;
			}
		}
		else if( touchEnded )
		{
			if( button != null )
			{
				// If we are getting an exit over a previously selected button send it an onTouchEnded
				if( _spriteSelected[touch.fingerId] != button && _spriteSelected[touch.fingerId] != null )
				{
					_spriteSelected[touch.fingerId].onTouchEnded( touch, fixedTouchPosition, false );
				}
				else if( _spriteSelected[touch.fingerId] == button )
				{
					_spriteSelected[touch.fingerId].onTouchEnded( touch, fixedTouchPosition, true );
				}
				
				// Deselect the touched sprite
				_spriteSelected[touch.fingerId] = null;
			}
		}
	}
	
	
	// Gets the closets touchableSprite to the camera that contains the touchPosition
	private GUITouchableSprite getButtonForScreenPosition( Vector2 touchPosition )
	{
		// Run through our touchables in order.  They are sorted by z-index already.
		for( int i = 0, totalTouchables = _touchableSprites.Count; i < totalTouchables; i++ )
		{
			if( !_touchableSprites[i].hidden && _touchableSprites[i].hitTest( touchPosition ) )
				return _touchableSprites[i];
		}
		
		return null;
	}

	#endregion;


	#region Animations

	// Float version (for alpha)
	public GUIAnimation to( GUISprite sprite, float duration, GUIAnimationProperty aniProperty, float target, IEasing ease, Easing.EasingType easeType )
	{
		return animate( true, sprite, duration, aniProperty, target, ease, easeType );
	}
	
	
	// Vector3 version
	public GUIAnimation to( GUISprite sprite, float duration, GUIAnimationProperty aniProperty, Vector3 target, IEasing ease, Easing.EasingType easeType )
	{
		return animate( true, sprite, duration, aniProperty, target, ease, easeType );
	}

	
	// float version
	public GUIAnimation from( GUISprite sprite, float duration, GUIAnimationProperty aniProperty, float target, IEasing ease, Easing.EasingType easeType )
	{
		return animate( false, sprite, duration, aniProperty, target, ease, easeType );
	}
	

	// Vector3 version
	public GUIAnimation from( GUISprite sprite, float duration, GUIAnimationProperty aniProperty, Vector3 target, IEasing ease, Easing.EasingType easeType )
	{
		return animate( false, sprite, duration, aniProperty, target, ease, easeType );
	}

	
	// Sets up and starts a new animation in a CoRoutine - float version
	private GUIAnimation animate( bool animateTo, GUISprite sprite, float duration, GUIAnimationProperty aniProperty, float target, IEasing ease, Easing.EasingType easeType )
	{
		float current = 0.0f;
		
		// Grab the current value
		switch( aniProperty )
		{
			case GUIAnimationProperty.Alpha:
				current = sprite.color.a;
				break;
		}

		float start = ( animateTo ) ? current : target;

		// If we are doing a 'from', the target is our current position
		if( !animateTo )
			target = current;
		
		GUIAnimation ani = new GUIAnimation( sprite, duration, aniProperty, start, target, ease, easeType );
		StartCoroutine( ani.animate() );
		
		return ani;
	}
	

	// Sets up and starts a new animation in a CoRoutine - Vector3 version
	private GUIAnimation animate( bool animateTo, GUISprite sprite, float duration, GUIAnimationProperty aniProperty, Vector3 target, IEasing ease, Easing.EasingType easeType )
	{
		Vector3 current = Vector3.zero;
		
		// Grab the current value
		switch( aniProperty )
		{
			case GUIAnimationProperty.Position:
				current = sprite.clientTransform.position;
				break;
			case GUIAnimationProperty.LocalScale:
				current = sprite.clientTransform.localScale;
				break;
			case GUIAnimationProperty.EulerAngles:
				current = sprite.clientTransform.eulerAngles;
				break;
		}
		
		Vector3 start = ( animateTo ) ? current : target;
		
		// If we are doing a 'from', the target is our current position
		if( !animateTo )
			target = current;
		
		GUIAnimation ani = new GUIAnimation( sprite, duration, aniProperty, start, target, ease, easeType );
		StartCoroutine( ani.animate() );
		
		return ani;
	}
	
	#endregion;

}
