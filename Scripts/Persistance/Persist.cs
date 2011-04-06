using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


public static class Persist
{
    public static void serializeObjectToFile( object obj, string filename )
    {
        // Get the path to the file to save to
        string path = Persist.pathForDocumentsFile( filename );

        // Create a StreamWriter to write the XML to disk
        StreamWriter sw = new StreamWriter( path );
        XmlSerializer serializer = new XmlSerializer( obj.GetType() );
		serializer.Serialize( sw, obj );
        sw.Close();
    }
	
	
    public static object deserializeObjectFromFile( string filename, Type type )
    {
        // Get the path to the file to save to
        string path = Persist.pathForDocumentsFile( filename );
        object obj;

        StreamReader sr = new StreamReader( path );
        XmlSerializer serializer = new XmlSerializer( type );
        obj = serializer.Deserialize( sr );
		sr.Close();

        return obj;
    }
	
	
	public static void writeStringToFile( string str, string filename )
	{
        // Get the path to the file to save to
        string path = Persist.pathForDocumentsFile( filename );
		
        StreamWriter sw = new StreamWriter( path );
        sw.Write( str );
        sw.Close();
	}
	
	
	public static string readStringFromFile( string filename )
	{
        // Get the path to the file to save to
        string path = Persist.pathForDocumentsFile( filename );
		
        StreamReader sr = new StreamReader( path );
        string str = sr.ReadToEnd();
        sr.Close();
		
		return str;
	}
	
	
    public static string pathForDocumentsFile( string filename ) 
    {
		// this will drop the files in the Assets/Documents directory for the editor. Be sure to create the directory first!
		string basePath = ( Application.isEditor ) ? Application.dataPath : Environment.GetFolderPath( Environment.SpecialFolder.Personal );
		
		if( Application.isEditor )
			basePath = Path.Combine( basePath, "Documents" );
			
        return Path.Combine( basePath, filename );
    }

}