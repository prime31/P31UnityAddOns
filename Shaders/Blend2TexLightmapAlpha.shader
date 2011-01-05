/*
http://www.unifycommunity.com/wiki/index.php?title=Blend_2_Textures_by_Lightmap_Alpha

This shader blends between two tiling textures based on a greyscale "splatmap", and uses a lightmap instead of real-time lighting.

In your modeling app, you create two UV maps - one for Texture 1 and Texture 2, and another for the lightmap and splatmap. The idea is that you will either extend the UVs of the model outside the 0-1 range, overlap them, or both, in the first map, and not do any of that in the second - standard lightmapping practice. By default, this means that your two main textures will tile at the same "rate", but you can use the Offset and/or Tiling controls in the Material, if you think it will add visual interest.
The splatmap is stored in the alpha channel of the lightmap. The two textures mix as if, in Photoshop, you had Texture 1 in a layer above Texture 2, and Texture 1 used the splatmap as a layer mask. i.e. white = Texture 1, black = Texture 2. In my usage so far, I've found that it's important to have the lightmap/splatmap be at a high resolution, to preserve the shape of fine splat details, but that it's okay to use PVRTC 2 bpp compression, as the color detail is mainly coming from Texture 1 and Texture 2.
*/
Shader "iOS/Unlit/Blend 2 Textures by Lightmap Alpha"
{


	Properties
	{
		_MainTex("Texture 1  (RGB)", 2D) = ""
		_Texture2("Texture 2  (RGB)", 2D) = ""
		_LightMap("Lightmap  (A = Splat)", 2D) = ""
	}

	// iPhone 3GS and later
	SubShader
	{
		Pass
		{
			BindChannels
			{
				Bind "Vertex", vertex

				// 1st UV - tiling textures
				Bind "texcoord", texcoord0
				Bind "texcoord", texcoord2

				// 2nd UV - lightmap and splatmap
				Bind "texcoord1", texcoord1
				Bind "texcoord1", texcoord3
			}

			// tile Texture 2
			SetTexture[_Texture2]

			// put the splat map into the alpha channel
			SetTexture[_LightMap]
			{
				combine previous, texture
			}

			// tile Texture 1 and combine it with Texture 2 based on the splat map
			SetTexture[_MainTex]
			{
				combine texture lerp(previous) previous
			}

			// apply the lightmap
			SetTexture[_LightMap]
			{
				combine previous * texture
			}
		}
	}

	// pre-3GS devices, including the September 2009 8GB iPod touch
	SubShader
	{
		Pass
		{
			BindChannels
			{
				Bind "Vertex", vertex
				Bind "texcoord", texcoord0
				Bind "texcoord1", texcoord1
			}

			// tile Texture 2
			SetTexture[_Texture2]

			// apply the lightmap to Texture 2
			SetTexture[_LightMap]
			{
				combine previous * texture
			}
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			BindChannels
			{
				Bind "Vertex", vertex
				Bind "texcoord", texcoord0
				Bind "texcoord1", texcoord1
			}

			// tile Texture 1
			SetTexture[_MainTex]

			// apply the lightmap to Texture 1 
			// and blend in the result, using the splat map
			SetTexture[_LightMap]
			{
				combine previous * texture, texture
			}
		}
	}
}