using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class So : MonoBehaviour
{
	public static So und;
	public int initialCapacity = 5;
	private List<Sound> _soundList;
	private Sound _bgSound;
	
	void Awake()
	{
		// Create the _soundList to speed up sound playing in game
		_soundList = new List<Sound>( initialCapacity );
		
		for( int i = 0; i < initialCapacity; i++ )
			_soundList.Add( new Sound( this ) );
		
		und = this;
	}
	
	
	public void playBGMusic( AudioClip audioClip, bool loop )
	{
		if( _bgSound == null )
			_bgSound = new Sound( this );

		_bgSound.loop = loop;
		StartCoroutine( _bgSound.playAudioClip( audioClip, AudioRolloffMode.Linear, 0.5f, Vector3.zero ) );
	}
	
	
	public void playSound( AudioClip audioClip )
	{
		playSound( audioClip, AudioRolloffMode.Linear, 0.5f, Vector3.zero );
	}
	
	
	public void playSound( AudioClip audioClip, AudioRolloffMode rolloff, float volume, Vector3 position )
	{
		bool multiPlay = false;
		if( !multiPlay )
		{
			if( GameObject.Find( "/" + audioClip.name ) || GameObject.Find( "/" + audioClip.name + "(clone)" ) )
				return;
		}
		
		// Find the first Sound not being used.  If they are all in use, create a new one
		Sound _sound;
		_sound = _soundList.Find( findFirstUnusedSound );

		if( _sound == null )
		{
			_sound = new Sound( this );
			_sound.destroyAfterPlay = true;
			_soundList.Add( _sound );
		}
		
		StartCoroutine( _sound.playAudioClip( audioClip, rolloff, volume, position ) );
	}
	
	
	public void removeSound( Sound s )
	{
		_soundList.Remove( s );
	}
	
	
	// Predicate to find the first available Sound in the list
	private static bool findFirstUnusedSound( Sound s )
	{
		return s.available;
	}

}
