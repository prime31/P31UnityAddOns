using UnityEngine;
using System;
using System.Collections;


public static class AudioSourceExtensions
{
	public static IEnumerator fadeOut( this AudioSource audioSource, float duration, Action onComplete )
	{
		float startingVolume = audioSource.volume;
		
		// fade out the volume
		while( audioSource.volume > 0.0f )
		{
			audioSource.volume -= Time.deltaTime * startingVolume / duration;
			yield return null;
		}

        // all done fading out
        if( onComplete != null )
			onComplete();
	}
}
