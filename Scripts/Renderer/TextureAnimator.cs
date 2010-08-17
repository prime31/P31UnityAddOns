using UnityEngine;
using System.Collections;


[RequireComponent( typeof( Renderer ) )]
public class TextureAnimator : MonoBehaviour
{
	public float scrollSpeed = 0.5f;
	public string properyName = "_MainTex.offset.y";
	
	
	void Awake()
	{
		Animation anim = gameObject.AddComponent( typeof( Animation ) ) as Animation;
		AnimationClip clip = new AnimationClip();
		clip.name = "TextureAnimator";

		AnimationCurve curve = AnimationCurve.Linear( 0, 0, 1.0f / scrollSpeed, 1.0f );
		clip.SetCurve( string.Empty, typeof( Material ), properyName, curve );
		
		anim.AddClip( clip, clip.name );
		anim.wrapMode = WrapMode.Loop;
		anim.Play( clip.name );
	}
}
