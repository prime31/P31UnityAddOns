// Unlit with a forward offset to help with sorting issues
Shader "iOS/Unlit/Unlit Forward"
{
	Properties
	{
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

	Category
	{
		Lighting Off
		ZWrite On
		Cull Back
		Offset - 1, -1
		
		SubShader
		{
			Pass
			{
				SetTexture[_MainTex]
				{
					constantColor[_Color]
					Combine texture * constant, texture * constant
				} // end SetTexture
			} // end Pass
		} // end SubShader
	} // end Category
}