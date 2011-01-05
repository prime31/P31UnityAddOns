// Very basic, single texture with lighting
Shader "iOS/Lit/Simply Lit"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1)
		_MainTex("Texture", 2D) = ""
	}

	SubShader
	{
		Lighting On
		Material
		{
			Ambient[_Color]
			Diffuse[_Color]
		}

		Pass
		{
			SetTexture[_MainTex]
			{
				Combine texture * primary Double
			}
		}
	} // end subShader
}