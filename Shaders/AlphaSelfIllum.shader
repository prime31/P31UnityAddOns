// Unlit with a forward offset to help with sorting issues
Shader "iOS/Unlit/Alpha Self Illum"
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
		Cull Back
		Offset - 1, -1
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
			} // end Material
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