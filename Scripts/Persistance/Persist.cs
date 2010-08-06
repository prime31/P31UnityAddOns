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
		// TODO: Switch to using:
		//		Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/image.png";
		
        // Application.dataPath returns
        // /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data
#if !UNITY_IPHONE 
		string path = Path.Combine( Application.dataPath, "Documents" );
		Debug.Log( path );
#else
        string path = Application.dataPath.Substring( 0, Application.dataPath.Length - 5 );
#endif
		
        // Strip application name 
        path = path.Substring( 0, path.LastIndexOf( '/' ) );
		
        return Path.Combine( Path.Combine( path, "Documents" ), filename );
    }


}