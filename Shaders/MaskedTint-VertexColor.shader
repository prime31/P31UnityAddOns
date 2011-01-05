Shader "iOS/Unlit/Masked Tint - Vertex Colors"
{
	Properties
	{
		_MainTex("Texture  (A = Tint Mask)", 2D) = ""
	}

	SubShader
	{
		Pass
		{
			BindChannels
			{
				Bind "Vertex", vertex
				Bind "Texcoord", texcoord
				Bind "Color", color
			}

			// tint by the vertex colors
			SetTexture[_MainTex]
			{
				combine primary * texture
			}

			// alpha blend between untinted and tinted
			SetTexture[_MainTex]
			{
				combine previous lerp(texture) texture
			}
		}
	} // end subShader
}