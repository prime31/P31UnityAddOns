using UnityEngine;
using System.Collections;

public class MyGuiSampleManager : MonoBehaviour
{

	void Start()
	{
		// Save the texture size locally for easy access
		Vector2 textureSize = GUISpriteUI.instance.textureSize;
		
		// IMPORTANT: depth is 1 on top higher numbers on the bottom.  This means the lower the number is the closer it gets to the camera.
		GUISpriteButton playButton = GUISpriteUI.instance.addSpriteButton( new Rect( 10, 10, 108, 37 ), 3, new UVRect( 0, 0, 108, 37, textureSize ) ) as GUISpriteButton;
		playButton.highlightedUVframe = new UVRect( 0, 37, 108, 37, textureSize );
		
		
		// Scores button
		GUISpriteButton scores = GUISpriteUI.instance.addSpriteButton( new Rect( 10, 57, 108, 37 ), 3, new UVRect( 0, 74, 108, 37, textureSize ) ) as GUISpriteButton;
		scores.highlightedUVframe = new UVRect( 0, 111, 108, 37, textureSize );
		scores.highlightedTouchOffsets = new GUIEdgeOffsets( 30, 30, 30, 30 ); // Expand our highlighted touch area 30 pixels all around
		scores.action = onTouchUpInsideScoresButton;
		scores.color = new Color( 1, 1, 1, 0.5f );
		
		
		// Options button
		GUISpriteButton optionsButton = GUISpriteUI.instance.addSpriteButton( new Rect( 10, 130, 108, 37 ), 2, new UVRect( 0, 148, 108, 37, textureSize ) ) as GUISpriteButton;
		optionsButton.highlightedUVframe = new UVRect( 0, 148 + 37, 108, 37, textureSize );
		optionsButton.action = onTouchUpInsideOptionsButton;
		
		
		// Knob
		GUIKnob knob = new GUIKnob( new Rect( 200, 160, 72, 72 ), 3, new UVRect( 109, 0, 72, 72, textureSize ), onKnobChanged );
		knob.highlightedUVframe = new UVRect( 190, 0, 72, 72, textureSize );
		knob.highlightedTouchOffsets = new GUIEdgeOffsets( 30, 30, 30, 30 );
		GUISpriteUI.instance.addTouchableSprite( knob );
		knob.value = 0.3f;
		
		
		// Horizontal Slider.  Be sure to offset the sliderKnobs Y value to line it up properly
		GUISprite hSliderKnob = GUISpriteUI.instance.addSprite( new Rect( 20, 245, 30, 50 ), new UVRect( 120, 130, 30, 50, textureSize ), 1 );
		GUISlider hSlider = new GUISlider( new Rect( 20, 250, 200, 40 ), 5, new UVRect( 120, 80, 200, 40, textureSize ), hSliderKnob, onHSliderChanged );
		GUISpriteUI.instance.addTouchableSprite( hSlider );
		// Increase our hit area a bit while we are tracking along the slider
		hSlider.highlightedTouchOffsets = new GUIEdgeOffsets( 20, 30, 20, 30 );
		hSlider.value = 0.2f;
		
		
		// Vertical Slider.  Be sure to offset the sliderKnobs Y value to line it up properly
		GUISprite vSliderKnob = GUISpriteUI.instance.addSprite( new Rect( 412, 50, 35, 10 ), new UVRect( 345, 130, 35, 10, textureSize ), 1 );
		GUISlider vSlider = new GUISlider( new Rect( 420, 50, 20, 200 ), 3, new UVRect( 320, 130, 20, 200, textureSize ), vSliderKnob, SliderLayout.Vertical, onVSliderChanged );
		GUISpriteUI.instance.addTouchableSprite( vSlider );
		// Increase our hit area a bit while we are tracking along the slider
		vSlider.highlightedTouchOffsets = new GUIEdgeOffsets( 20, 30, 20, 30 );
		vSlider.continuous = true;
		vSlider.value = 0.5f;
		
		
		// Toggle Button
		UVRect normalUVframe = new UVRect( 0, 400, 50, 50, textureSize );
		UVRect highlightedUVframe = new UVRect( 0, 450, 50, 50, textureSize );
		UVRect selectedUVframe = new UVRect( 50, 400, 50, 50, textureSize );
		Rect toggleFrame = new Rect( 270, 80, 50, 50 );
		GUISpriteToggleButton toggleButton = new GUISpriteToggleButton( toggleFrame, 2, normalUVframe, selectedUVframe, highlightedUVframe );
		toggleButton.action = onToggleButtonChanged;
		GUISpriteUI.instance.addTouchableSprite( toggleButton );
		toggleButton.selected = true; // Dont change this until the button has been added
		
		
		// Progress/Health bar (be sure the bar is on a lower level than the GUIProgressBar
		GUISprite bar = GUISpriteUI.instance.addSprite( new Rect( 251, 267, 128, 8 ), new UVRect( 191, 430, 128, 8, textureSize ), 1 );
		GUIProgressBar progressBar = new GUIProgressBar( new Rect( 240, 250, 150, 30 ), 3, new UVRect( 180, 400, 150, 30, textureSize ), bar );
		progressBar.resizeTextureOnChange = true;
		GUISpriteUI.instance.addSprite( progressBar );
		progressBar.value = 0.0f;

		
		// Test movement
		StartCoroutine( marqueePlayButton( playButton ) );
		StartCoroutine( animateProgressBar( progressBar ) );
		StartCoroutine( pulseOptionButton( optionsButton ) );
	}
	
	
	#region CoRoutine animation tests that do not use the GUIAnimation system
	
	// Play coroutine that animates a button marquee style
	private IEnumerator marqueePlayButton( GUISpriteButton playButton )
	{
		while( true )
		{
			// Make sure we arent off the right side of the screen
			Vector3 pos = playButton.clientTransform.position;
			if( pos.x > 480 + playButton.width / 2 )
			{
				pos.x = -playButton.width / 2;
				playButton.clientTransform.position = pos;
			}
			
			playButton.clientTransform.Translate( 2.0f, 0, 0 );
			playButton.transform();
			
			yield return 0;
		}
	}
	
	
	private IEnumerator animateProgressBar( GUIProgressBar progressBar )
	{
		float value = 0.0f;
		
		while( true )
		{
			// Make sure the progress doesnt exceed 1
			if( value > 1.0f )
			{
				// Swap the progressBars resizeTextureOnChange property
				progressBar.resizeTextureOnChange = !progressBar.resizeTextureOnChange;
				value = 0.0f;
			}
			else
			{
				value += 0.01f;
			}
			
			progressBar.value = value;
			
			yield return 0;
		}
	}
	
	#endregion;
	
	
	#region GUIAnimations
	
	private IEnumerator animateLocalScale( GUISprite sprite, Vector3 to, float duration )
	{
		Vector3 originalPosition = sprite.clientTransform.localScale;
		
		// Go back and forth.  The chain() method will return when the animation is done
		GUIAnimation ani = GUISpriteUI.instance.to( sprite, duration, GUIAnimationProperty.LocalScale, to, Easing.Sinusoidal.factory(), Easing.EasingType.Out );
		yield return ani.chain();
		
		GUISpriteUI.instance.to( sprite, duration, GUIAnimationProperty.LocalScale, originalPosition, Easing.Circular.factory(), Easing.EasingType.In );
	}


	private IEnumerator animatePosition( GUISprite sprite, Vector3 to, float duration )
	{
		Vector3 originalPosition = sprite.clientTransform.position;
		
		// Go back and forth.  The chain() method will return when the animation is done
		GUIAnimation ani = GUISpriteUI.instance.to( sprite, duration, GUIAnimationProperty.Position, to, Easing.Quintic.factory(), Easing.EasingType.InOut );
		yield return ani.chain();
		
		GUISpriteUI.instance.to( sprite, duration, GUIAnimationProperty.Position, originalPosition, Easing.Quintic.factory(), Easing.EasingType.In );
	}


	private IEnumerator animateRotation( GUISprite sprite, Vector3 to, float duration )
	{
		Vector3 originalPosition = sprite.clientTransform.eulerAngles;
		
		// Go back and forth.  The chain() method will return when the animation is done
		GUIAnimation ani = GUISpriteUI.instance.to( sprite, duration, GUIAnimationProperty.EulerAngles, to, Easing.Sinusoidal.factory(), Easing.EasingType.Out );
		yield return ani.chain();
		
		GUISpriteUI.instance.to( sprite, duration, GUIAnimationProperty.EulerAngles, originalPosition, Easing.Circular.factory(), Easing.EasingType.In );
	}
	
	
	private IEnumerator pulseOptionButton( GUISpriteButton optionsButton )
	{
		GUIAnimation ani;
		
		while( true )
		{
			ani = GUISpriteUI.instance.to( optionsButton, 0.7f, GUIAnimationProperty.Alpha, 0.1f, Easing.Linear.factory(), Easing.EasingType.In );
			yield return ani.chain();
			
			ani = GUISpriteUI.instance.to( optionsButton, 0.7f, GUIAnimationProperty.Alpha, 1.0f, Easing.Linear.factory(), Easing.EasingType.Out );
			yield return ani.chain();
		}
	}
	
	#endregion;

	
	#region Callbacks
	
	// Button callback
	public void onTouchUpInsideScoresButton( GUISpriteButton sender )
	{
		StartCoroutine( animateLocalScale( sender, new Vector3( 1.3f, 1.3f, 1 ), 0.5f ) );
	}
	
	
	public void onTouchUpInsideOptionsButton( GUISpriteButton sender )
	{
		// Rotation should be around the z axis
		StartCoroutine( animateRotation( sender, new Vector3( 0, 0, 359 ), 1.0f ) );
		               
		// Dont forget to make the y negative because our origin is the top left
		StartCoroutine( animatePosition( sender, new Vector3( 270, -200, 1 ), 1.0f ) );
	}
	
	
	// Knob callback
	public void onKnobChanged( GUIKnob sender, float value )
	{
		//Debug.Log( "onKnobChanged: " + value );
	}
		

	// Slider callback
	public void onHSliderChanged( GUISlider sender, float value )
	{
		//Debug.Log( "onHSliderChanged to: " + value );
	}
	
	
	public void onVSliderChanged( GUISlider sender, float value )
	{
		//Debug.Log( "onVSliderChanged to: " + value );
	}

	
	public void onToggleButtonChanged( GUISpriteToggleButton sender, bool selected )
	{
		//Debug.Log( "onToggleButtonChanged to: " + selected );
	}
	
	#endregion;


}
