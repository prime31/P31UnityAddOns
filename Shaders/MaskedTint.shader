/*
http://www.unifycommunity.com/wiki/index.php?title=Masked_Tint
Description
These shaders tint the model's texture based on a mask, so that you can selectively colorize portions of your texture. The Masked Tint - Vertex Colors shader allows you to have many different color variations for the tinted area, on many different instances of your model, but only use one draw call.
Masked Tint will allow you to save memory, by not having to store a different mesh for every color you want to use, but does not offer the batching power of Masked Tint - Vertex Colors. This shader may be a good choice if you only want to colorize using a single color, and your meshes have over 300 vertices, removing the ability to use dynamic batching.
Finally, Masked Tint - Color from UV 2 is an alternative to Masked Tint - Vertex Colors, and is basically workaround, in the case where you don't have the ability to export vertex colors. (As far as I know, this is true of modo, for example.)
[edit]Usage

Create a texture, with RGB colors where you want to share colors between all instances of the mesh. For the areas that you will want to colorize differently, I recommend using grayscale. Create a mask, stored in the alpha channel, that gets whiter as you want more colorization. I assume that for most usage, you will want to mainly use pure white or black, with the gray antialiased edges between making transitions look good.
If you're using Masked Tint, then all you need to do is drag the RGBA texture onto the material's only variable slot, and choose a Tint Color. You'll need a different Unity Material for every different color you want to use.
Masked Tint - Vertex Colors requires the use of only one material for all similar objects, but you will need to create a separate mesh for each color scheme. As the name implies, you paint the tint colors into the vertex colors.
*/
Shader "iOS/Unlit/Masked Tint"
{
	Properties
	{
		_Color("Tint Color", Color) = (1, 1, 1)
		_MainTex("Texture  (A = Tint Mask)", 2D) = ""
	}

	SubShader
	{
		Pass
		{
			// tint the texture
			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
				combine texture * constant
			}

			// alpha blend between untinted and tinted
			SetTexture[_MainTex]
			{
				combine previous lerp(texture) texture
			}
		}
	}
}