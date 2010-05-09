// This is a C# messenger (modeled after NSNotificationCenter). It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other.
//
// Usage example:
// Messenger<float>.addListener( MessageType.OnSomeMessage, MyEventHandler );
// ...
// Messenger<float>.postMessage( MessageType.OnSomeMessage, 1.0f );


using System;
using System.Collections.Generic;


// Events - Add event types here to avoid accidental misspellings
public enum MessageType {
	OnStuff,
	OnOtherStuff,
	OnSomeEvent
};

// Delegates
public delegate void OnMessageDelegate();
public delegate void OnMessageDelegate<T>( T arg1 );
public delegate void OnMessageDelegate<T, U>( T arg1, U arg2 );


// Holds the magical event table and handles the eventTable when adding/removing listeners
static internal class MessengerInternal
{
    static public Dictionary<MessageType, Delegate> eventTable = new Dictionary<MessageType, Delegate>();

    static public void onAddListener( MessageType messageType, Delegate listenerBeingAdded )
	{
        if( !eventTable.ContainsKey( messageType ) )
            eventTable.Add( messageType, null );
    }

	
    static public void onRemoveListener( MessageType messageType )
	{
        if( eventTable[messageType] == null )
            eventTable.Remove( messageType );
    }

}


// No parameters
static public class Messenger
{
    private static Dictionary<MessageType, Delegate> eventTable = MessengerInternal.eventTable;
	
	
    static public void addListener( MessageType messageType, OnMessageDelegate handler )
	{
        MessengerInternal.onAddListener( messageType, handler );
        eventTable[messageType] = (OnMessageDelegate)eventTable[messageType] + handler;
    }

	
    static public void removeListener( MessageType messageType, OnMessageDelegate handler )
	{
		if( eventTable.ContainsKey( messageType ) )
		{
			eventTable[messageType] = (OnMessageDelegate)eventTable[messageType] - handler;
			MessengerInternal.onRemoveListener( messageType );
		}
    }

	
    static public void postMessage( MessageType messageType )
	{
        Delegate d;
        if( eventTable.TryGetValue( messageType, out d ) )
		{
            OnMessageDelegate OnMessageDelegate = d as OnMessageDelegate;
            if( OnMessageDelegate != null )
			{
                OnMessageDelegate();
            }
        }
    }

} // end Messenger


// One parameter
static public class Messenger<T>
{
    private static Dictionary<MessageType, Delegate> eventTable = MessengerInternal.eventTable;
	
	
    static public void addListener( MessageType messageType, OnMessageDelegate<T> handler )
	{
        MessengerInternal.onAddListener( messageType, handler );
        eventTable[messageType] = (OnMessageDelegate<T>)eventTable[messageType] + handler;
    }

	
    static public void removeListener( MessageType messageType, OnMessageDelegate<T> handler )
	{
		if( eventTable.ContainsKey( messageType ) )
		{
			eventTable[messageType] = (OnMessageDelegate<T>)eventTable[messageType] - handler;
			MessengerInternal.onRemoveListener( messageType );
		}
    }

	
    static public void postMessage( MessageType messageType, T arg1 )
	{
        Delegate d;
        if( eventTable.TryGetValue( messageType, out d ) )
		{
            OnMessageDelegate<T> OnMessageDelegate = d as OnMessageDelegate<T>;
            if( OnMessageDelegate != null )
                OnMessageDelegate( arg1 );
        }
    }

} // end Messenger<T>


// Two parameters
static public class Messenger<T, U>
{
    private static Dictionary<MessageType, Delegate> eventTable = MessengerInternal.eventTable;
	
	
    static public void addListener( MessageType messageType, OnMessageDelegate<T, U> handler )
	{
		MessengerInternal.onAddListener( messageType, handler );
		eventTable[messageType] = (OnMessageDelegate<T, U>)eventTable[messageType] + handler;
    }

	
    static public void removeListener( MessageType messageType, OnMessageDelegate<T, U> handler )
	{
		if( eventTable.ContainsKey( messageType ) )
		{
			eventTable[messageType] = (OnMessageDelegate<T, U>)eventTable[messageType] - handler;
			MessengerInternal.onRemoveListener( messageType );
		}
    }

	
    static public void postMessage( MessageType messageType, T arg1, U arg2 )
	{
        Delegate d;
        if( eventTable.TryGetValue( messageType, out d ) )
		{
            OnMessageDelegate<T, U> OnMessageDelegate = d as OnMessageDelegate<T, U>;
            if( OnMessageDelegate != null )
                OnMessageDelegate( arg1, arg2 );
        }
    }

} // end Messenger<T, U>
