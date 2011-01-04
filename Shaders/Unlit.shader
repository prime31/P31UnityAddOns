// Unlit (The ultimate no frills shader)
Shader "iOS/Unlit/Unlit Texture Only (FAST)"
{
	Properties
	{
		_MainTex("Texture", 2D) = ""
	}

	SubShader
	{
		Pass
		{
			SetTexture[_MainTex]
		}
	}
}