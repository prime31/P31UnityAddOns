using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;



public class DebugToggleMenuItem : MonoBehaviour
{
	private const string DEBUG_LINE = "#define DEBUG";
	
	
	[MenuItem( "Prime31/Debug Define/Enable", true )]
	static bool enableDefineCheck()
	{
		return PlayerPrefs.GetInt( "debugEnabled" ) == 0;
	}


	[MenuItem( "Prime31/Debug Define/Disable", true )]
	static bool disableDefineCheck()
	{
		return PlayerPrefs.GetInt( "debugEnabled" ) == 1;
	}


	[MenuItem( "Prime31/Debug Define/Enable" )]
	static void enableDebug()
	{
		enableDebug( true );
	}
	
	
	[MenuItem( "Prime31/Debug Define/Disable" )]
	static void disableDebug()
	{
		enableDebug( false );
	}
	
	
	private static void enableDebug( bool enable )
	{
		PlayerPrefs.SetInt( "debugEnabled", enable ? 1 : 0 );
		
		var path = Path.Combine( Directory.GetCurrentDirectory(), "Assets" );
		var files = Directory.GetFiles( path, "*.cs", SearchOption.AllDirectories );
		
		foreach( var file in files )
		{
			// No need to modify Editor files
			if( file.Contains( "/Editor/" ) )
				continue;

			// Modify only the first line of the file!
		    var tempfile = Path.GetTempFileName();
		    using( var writer = new StreamWriter( tempfile ) )
		    {
				using( var reader = new StreamReader( file ) )
				{
					if( enable )
				    	writer.WriteLine( DEBUG_LINE );
				    
				    while( !reader.EndOfStream )
				    {
				    	var line = reader.ReadLine();
				    	if( !enable )
				    	{
				    		// skip the debug line if we are disabling
				    		if( line.Contains( DEBUG_LINE ) )
				    			continue;
				    	}
				    	writer.WriteLine( line );
				    }
				}		    
		    }

		    File.Copy( tempfile, file, true );
		}
	}
	
	
	private static void addDebugLine( string file )
	{
	
	}
	
	
	private static void removeDebugLine( string file )
	{
		
	}


}