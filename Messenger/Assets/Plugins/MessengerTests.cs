using UnityEngine;
using System;
using System.Collections.Generic;


public class MessengerTests : MonoBehaviour
{
	public void Start()
	{
		addListeners();
		broadcastEvents();
		removeListeners();
		broadcastEvents();
	}
	

	private void addListeners()
	{
		Messenger.addListener( MessageType.OnStuff, noParameters );
		Messenger.addListener( MessageType.OnStuff, noParametersTwo );
		Messenger<float>.addListener( MessageType.OnOtherStuff, oneFloatParam );
		Messenger<int, string>.addListener( MessageType.OnSomeEvent, twoParams );
	}
	
	
	private void removeListeners()
	{
		Messenger.removeListener( MessageType.OnStuff, noParameters );
		Messenger.removeListener( MessageType.OnStuff, noParametersTwo );
		Messenger<float>.removeListener( MessageType.OnOtherStuff, oneFloatParam );
		Messenger<int, string>.removeListener( MessageType.OnSomeEvent, twoParams );
	}
	
	
	private void broadcastEvents()
	{
		Messenger.postMessage( MessageType.OnStuff );
		Messenger<float>.postMessage( MessageType.OnOtherStuff, 5.6f );
		Messenger<int, string>.postMessage( MessageType.OnSomeEvent, 8, "a_string_of_letters" );
	}
	
	
	// Listeners
	private void noParameters()
	{
		Debug.Log( "--- callback noParameters" );
	}
	

	private void noParametersTwo()
	{
		Debug.Log( "--- callback noParametersTwo" );
	}
	
	
	private void oneFloatParam( float f )
	{
		Debug.Log( "--- callback oneFloatParam: " + f.ToString() );
	}
	
	
	private void twoParams( int i, string s )
	{
		Debug.Log( "--- callback twoParams: " + i.ToString() + ", " + s );
	}

}

