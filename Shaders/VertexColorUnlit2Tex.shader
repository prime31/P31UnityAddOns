// per-vertex colors and no lighting

Shader "iOS/Unlit/Vertex color 2 tex"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Texture", 2D) = "white" {}
	}

	Category
	{
		Tags
		{
			"Queue" = "Geometry"
		}
		Lighting Off
		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		// ---- Dual texture cards
		SubShader
		{
			Pass
			{
				SetTexture[_MainTex]
				{
					combine texture * primary
				}
				SetTexture[_MainTex]
				{
					constantColor[_Color]
					combine previous lerp(previous) constant DOUBLE
				}
			}
		}

		// ---- Single texture cards (does not do vertex colors)
		SubShader
		{
			Pass
			{
				SetTexture[_MainTex]
				{
					constantColor[_Color]
					combine texture lerp(texture) constant DOUBLE
				}
			}
		}
	}
}