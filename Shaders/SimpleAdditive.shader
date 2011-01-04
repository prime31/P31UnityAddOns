// Here is a small example shader that adds a texture to whatever is on the screen already:
Shader "iOS/Simple Additive"
{
	Properties
	{
		_MainTex("Texture to blend", 2D) = "black" {}
	}

	SubShader
	{
		Blend One One

		Pass
		{
			SetTexture[_MainTex]
			{
				combine texture
			}
		}
	}
}