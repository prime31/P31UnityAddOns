using UnityEngine;
using System.Collections;


public class Sound
{
	private So _manager;
	private AudioSource _audioSource;
	private GameObject _gameObject;
	
	public bool available = true;
	public bool destroyAfterPlay = false;
	
	public bool loop
	{
		set
		{
			_audioSource.loop = value;
		}
	}
	
	
	public Sound( So manager )
	{
		_manager = manager;
		
		// Create a GameObject to hold the audioSource for playing sounds
		_gameObject = new GameObject();
		_gameObject.transform.parent = manager.transform;
		
		_audioSource = _gameObject.AddComponent( "AudioSource" ) as AudioSource;
	}
	
	
	public void destroy()
	{
		_manager.removeSound( this );
		
		MonoBehaviour.Destroy( _gameObject );
	}
	
	
	public IEnumerator playAudioClip( AudioClip audioClip, AudioRolloffMode rolloff, float volume, Vector3 position )
	{
		available = false;
		
		// Setup the GameObject and AudioSource and start playing
		_gameObject.name = audioClip.name;
		_gameObject.transform.position = position;
		
		_audioSource.clip = audioClip;
		_audioSource.rolloffMode = rolloff;
		//_audioSource.pitch = Random.Range( 0.9f, 1.1f );
		_audioSource.audio.Play();
		
		// Wait for the clip to finish
		yield return new WaitForSeconds( _audioSource.clip.length + 0.1f );
		
		// Should we destory ourself after playing?
		if( destroyAfterPlay )
			this.destroy();
		
		available = true;
	}
	
}
