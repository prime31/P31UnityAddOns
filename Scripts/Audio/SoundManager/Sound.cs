using UnityEngine;
using System.Collections;


public class Sound
{
	private So _manager;
	
	public AudioSource audioSource;
	public GameObject gameObject;
	public bool available = true;
	public bool destroyAfterPlay = false;
	
	public bool loop
	{
		set
		{
			audioSource.loop = value;
		}
	}
	
	
	public Sound( So manager )
	{
		_manager = manager;
		
		// Create a GameObject to hold the audioSource for playing sounds
		gameObject = new GameObject();
		gameObject.name = "empty";
		gameObject.transform.parent = manager.transform;
		
		audioSource = gameObject.AddComponent<AudioSource>();
	}
	
	
	public void destroySelf()
	{
		_manager.removeSound( this );
		
		MonoBehaviour.Destroy( gameObject );
	}
	
	
	public void stop()
	{
		audioSource.Stop();
		destroySelf();
	}
	
	
	public IEnumerator fadeOutAndStop( float duration )
	{
		return audioSource.fadeOut( duration, () => stop() );
	}
	
	
	public IEnumerator playAudioClip( AudioClip audioClip, AudioRolloffMode rolloff, float volume, Vector3 position )
	{
		// Setup the GameObject and AudioSource and start playing
		gameObject.name = audioClip.name;
		audioSource.clip = audioClip;
		
		return play( rolloff, volume, position );
	}
	
	
	public IEnumerator play( AudioRolloffMode rolloff, float volume, Vector3 position )
	{
		available = false;
		
		// Setup the GameObject and AudioSource and start playing
		gameObject.transform.position = position;
		
		audioSource.rolloffMode = rolloff;
		audioSource.volume = volume;
		//audioSource.pitch = Random.Range( 0.9f, 1.1f );
		audioSource.audio.Play();
		
		// Wait for the clip to finish
		yield return new WaitForSeconds( audioSource.clip.length + 0.1f );
		
		// Should we destory ourself after playing?
		if( destroyAfterPlay )
			this.destroySelf();
		
		available = true;
	}
	
}
