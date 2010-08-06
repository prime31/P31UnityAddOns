// Unlit (The ultimate no frills shader)

Shader "Unlit/Unlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = ""
	}

	SubShader
	{
		Pass
		{
			SetTexture[_MainTex]
		}
	}
}