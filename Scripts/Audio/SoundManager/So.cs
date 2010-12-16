using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class So : MonoBehaviour
{
	public static So und = null;
	public int initialCapacity = 5;
	
	private List<Sound> _soundList;
	private Sound _bgSound;
	
	// state for app pause/unpause
	private bool _audioWasPlaying;
	private float _audioTime;
	
	
	void Awake()
	{
		// avoid duplicates
		if( und != null )
		{
			Destroy( gameObject );
			return;
		}
		
		und = this;
		DontDestroyOnLoad( gameObject );
		
		// Create the _soundList to speed up sound playing in game
		_soundList = new List<Sound>( initialCapacity );
		
		for( int i = 0; i < initialCapacity; i++ )
			_soundList.Add( new Sound( this ) );
	}
	
	
	void OnApplicationPause( bool didPause )
	{
		// are we pausing or unpausing?
		if( didPause )
		{
			// if we have a bgSound, save it
			if( _bgSound != null )
			{
				_audioWasPlaying = true;
				_audioTime = _bgSound.audioSource.time;
			}
		}
		else
		{
			// unpausing.  restart background music
			if( _audioWasPlaying )
			{
				_audioWasPlaying = false;
				_bgSound.audioSource.time = _audioTime;
				_bgSound.audioSource.Play();
			}
		}
	}
	

	public void playBGMusic( AudioClip audioClip, float volume, bool loop )
	{
		if( _bgSound == null )
			_bgSound = new Sound( this );

		_bgSound.loop = loop;
		StartCoroutine( _bgSound.playAudioClip( audioClip, AudioRolloffMode.Linear, volume, Vector3.zero ) );
	}
	
	
	public Sound playSound( AudioClip audioClip )
	{
		return playSound( audioClip, AudioRolloffMode.Linear, 0.9f, Vector3.zero );
	}


	public Sound playSound( AudioClip audioClip, AudioRolloffMode rolloff )
	{
		return playSound( audioClip, rolloff, 0.9f, Vector3.zero );
	}
	
	
	public Sound playSound( AudioClip audioClip, AudioRolloffMode rolloff, float volume )
	{
		return playSound( audioClip, rolloff, 0.9f );
	}
	
	
	public Sound playSound( AudioClip audioClip, AudioRolloffMode rolloff, float volume, Vector3 position )
	{
		/*
		bool multiPlay = false;
		if( !multiPlay )
		{
			if( GameObject.Find( "/" + audioClip.name ) || GameObject.Find( "/" + audioClip.name + "(clone)" ) )
				return;
		}
		*/
		
		// Find the first Sound not being used.  If they are all in use, create a new one
		// we prefer to find a sound that is available and has the same clipname as the current one
		Sound _sound = null;
		bool clipLoaded = false;
		bool foundEmpty = false;
		
		foreach( var s in _soundList )
		{
			// dont care about sounds that arent available
			if( !s.available )
				continue;
			
			if( s.gameObject.name == audioClip.name )
			{
				_sound = s;
				clipLoaded = true;
				break;
			}
			else
			{
				// if we already found an empty no need to check any further
				if( foundEmpty )
					continue;
				
				// empties are preferred if we didnt find a Sound with the clip already loaded
				if( s.gameObject.name == "empty" )
					foundEmpty = true;

				_sound = s;
			}
		}
		
		/* dumb search for available sounds
		Sound _sound = _soundList.Find( delegate( Sound s )
		{
			return s.available;
		});
		*/
		
		// if we didnt find an available found, bail out
		if( _sound == null )
		{
			_sound = new Sound( this );
			_sound.destroyAfterPlay = true;
			_soundList.Add( _sound );
		}
		
		// if we found a preloaded clip then use it
		if( clipLoaded )
			StartCoroutine( _sound.play( rolloff, volume, position ) );
		else
			StartCoroutine( _sound.playAudioClip( audioClip, rolloff, volume, position ) );
		
		return _sound;
	}
	
	
	public void removeSound( Sound s )
	{
		_soundList.Remove( s );
	}
}
