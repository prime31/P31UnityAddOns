// Unlit with Alpha, double-sided
Shader "iOS/Unlit/Alpha Self Illum Double Sided"
{
	Properties
	{
		_Color("Color Tint", Color) = (1, 1, 1, 1)
		_MainTex("SelfIllum Color (RGB) Alpha (A)", 2D) = "white"
	}

	Category
	{
		Lighting Off
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Tags
		{
			"Queue" = "transparent"
		}
		SubShader
		{
			Material
			{
				Emission[_Color]
			} // End Material
			Pass
			{
				SetTexture[_MainTex]
				{
					Combine Texture * Primary, Texture * Primary
				} // end SetTexture
			} // end Pass
		} // end SubShader
	} // end Category
}