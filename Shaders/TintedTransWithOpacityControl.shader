Shader "iOS/Tinted Transparent with Opacity Control"
{
	Properties
	{
		_Color("Tint Color (A = Opacity)", Color) = (1, 1, 1)
		_MainTex("Texture  (A = Opacity)", 2D) = ""
	}

	SubShader
	{
		Tags
		{
			Queue = Transparent
		}
		ZWrite Off
		Colormask RGB
		Blend SrcAlpha OneMinusSrcAlpha

		Color[_Color]
		Pass
		{
			SetTexture[_MainTex]
			{
				combine texture * primary
			}
		}
	}
}

// shader to still use the alpha from the image but have an overall opacity control that I can access to fade it in or out.
/*
// Example setting the opacity every frame
using UnityEngine; 

class SetOpacityEveryFrame : MonoBehaviour
{
	// Drag the material onto this variable slot
	// in the Inspector to avoid the GetComponent call
	// that comes with using renderer.material.
	public Material material;
	Color color;

	void Start()
	{
	   color = material.color;
	}

	void Update()
	{
		// color.a = (0-1 opacity value);
	    material.color = color;
	}
}
*/