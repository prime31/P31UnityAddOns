using UnityEngine;
using System;
using System.Collections;


public static class StringExtensions
{
	public static string pathForBundleResource( string file )
	{
		var path = Application.dataPath.Replace( "Data", "" );
		return System.IO.Path.Combine( path, file );
	}


	public static string pathForDocumentsResource( string file )
	{
		return System.IO.Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.Personal ), file );
	}
}
