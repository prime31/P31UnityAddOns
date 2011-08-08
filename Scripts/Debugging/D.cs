using UnityEngine;
using System.Collections;


// setting the conditional to the platform of choice will only compile the method for that platform
public class D
{
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	public static void log( string format, params object[] paramList )
	{
		Debug.Log( string.Format( format, paramList ) );
	}
	
	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	public static void warn( string format, params object[] paramList )
	{
		Debug.LogWarning( string.Format( format, paramList ) );
	}
	
	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	public static void error( string format, params object[] paramList )
	{
		Debug.LogError( string.Format( format, paramList ) );
	}
	
	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	public static void assert( bool condition )
	{
		assert( condition, string.Empty );
	}
	
	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	public static void assert( bool condition, string assertString )
	{
		if( !condition )
			Debug.LogError( "assert failed! " + assertString );
	}
	

	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	public static void assertFail()
	{
		assertFail( string.Empty );
	}
	
	
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	public static void assertFail( string assertString )
	{
		Debug.LogError( assertString );
	}

}
