using UnityEngine;
using System;


public class GUIProgressBar : GUISprite
{
	public bool resizeTextureOnChange = false;
	
	private float _value = 0;
	private GUISprite _bar;
	private float _barOriginalWidth;
	private UVRect _barOriginalUVframe;
	
	
	public GUIProgressBar( Rect frame, int depth, UVRect uvFrame, GUISprite bar ):base( frame, depth, uvFrame )
	{
		// Save the bar and make it a child of the slider for organization purposes
		_bar = bar;
		_bar.clientTransform.parent = this.clientTransform;
		
		// Save the bars original size
		_barOriginalWidth = _bar.width;
		_barOriginalUVframe = _bar.uvFrame;
	}


	public float value
	{
		get { return _value; }
		set
		{
			if( value != _value )
			{
				// Set the value being sure to clamp it to our min/max values
				_value = Mathf.Clamp( value, 0, 1 );
				
				// Update the bar UV's if resizeTextureOnChange is set
				if( resizeTextureOnChange )
				{
					// Set the uvFrame's width based on the value
					UVRect newUVframe = _barOriginalUVframe;
					newUVframe.uvDimensions.x *= _value;
					_bar.uvFrame = newUVframe;
				}

				// Update the bar size based on the value
				_bar.setSize( _value * _barOriginalWidth, _bar.height );
			}
		}
	}

}

