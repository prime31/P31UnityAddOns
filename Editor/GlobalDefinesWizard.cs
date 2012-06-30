using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


public class GlobalDefinesWizard : ScriptableWizard
{
	[System.Serializable]
	public class GlobalDefine : ISerializable
	{
		public string define;
		public bool enabled;
		
		
		public GlobalDefine()
		{}
		
		
		protected GlobalDefine( SerializationInfo info, StreamingContext context )
		{
			define = info.GetString( "define" );
			enabled = info.GetBoolean( "enabled" );
		}
		
		
		public void GetObjectData( SerializationInfo info, StreamingContext context )
		{
			info.AddValue( "define", define );
			info.AddValue( "enabled", enabled );
		}

	}
	
	private const string _prefsKey = "kGlobalDefines";
	public List<GlobalDefine> _globalDefines = new List<GlobalDefine>();
	
	
	[MenuItem( "Global Defines/Edit Global Defines" )]
    static void createWizardFromMenu()
	{
		var helper = ScriptableWizard.DisplayWizard<GlobalDefinesWizard>( "Global Defines Manager", "Save", "Cancel" );
		helper.minSize = new Vector2( 500, 300 );
		helper.maxSize = new Vector2( 500, 300 );
		
		// load up the defines
		if( EditorPrefs.HasKey( _prefsKey ) )
		{
			var data = EditorPrefs.GetString( _prefsKey );
			var bytes = System.Convert.FromBase64String( data );
			var stream = new MemoryStream( bytes );
			
			var formatter = new BinaryFormatter();
			helper._globalDefines = (List<GlobalDefine>)formatter.Deserialize( stream );
		}
	}
	
	
	void OnGUI()
	{
		var toRemove = new List<GlobalDefine>();
		
		foreach( var define in _globalDefines )
		{
			if( defineEditor( define ) )
				toRemove.Add( define );
		}
		
		foreach( var define in toRemove )
			_globalDefines.Remove( define );
		
		if( GUILayout.Button( "Add Define" ) )
		{
			var d = new GlobalDefine();
			d.define = "NEW_DEFINE";
			d.enabled = false;
			_globalDefines.Add( d );
		}
		GUILayout.Space( 40 );
		
		if( GUILayout.Button( "Save" ) )
		{
			save();
			Close();
		}
	}
	
	
	private void save()
	{
		// nothing to save means delete everything
		if( _globalDefines.Count == 0 )
		{
			deleteFiles();
			
			EditorPrefs.DeleteKey( _prefsKey );
			Close();
			return;
		}
		
		// save some stuff, first to prefs then to disk
		var formatter = new BinaryFormatter();
		using( var stream = new MemoryStream() )
		{
			formatter.Serialize( stream, _globalDefines );
			var data = System.Convert.ToBase64String( stream.ToArray() );
			stream.Close();
			
			EditorPrefs.SetString( _prefsKey, data );
		}
		
		// what shall we write to disk?
		var toDisk = _globalDefines.Where( d => d.enabled ).Select( d => d.define ).ToArray();
		if( toDisk.Length > 0 )
		{
			var builder = new System.Text.StringBuilder( "-define:" );
			for( var i = 0; i < toDisk.Length; i++ )
			{
				if( i < toDisk.Length - 1 )
					builder.AppendFormat( "{0};", toDisk[i] );
				else
					builder.Append( toDisk[i] );
			}
			
			writeFiles( builder.ToString() );
			
			AssetDatabase.Refresh();
			reimportSomethingToForceRecompile();
		}
		else
		{
			// nothing enabled to save, kill the files
			deleteFiles();
		}
	}
	
	
	private void reimportSomethingToForceRecompile()
	{
		var dataPathDir = new DirectoryInfo( Application.dataPath );
		var dataPathUri = new System.Uri( Application.dataPath );
		foreach( var file in dataPathDir.GetFiles( "GlobalDefinesWizard.cs", SearchOption.AllDirectories ) )
		{
			var relativeUri = dataPathUri.MakeRelativeUri( new System.Uri( file.FullName ) );
			var relativePath = System.Uri.UnescapeDataString( relativeUri.ToString() );
			AssetDatabase.ImportAsset( relativePath, ImportAssetOptions.ForceUpdate );
		}
	}
	
	
	private void deleteFiles()
	{
		var smcsFile = Path.Combine( Application.dataPath, "smcs.rsp" );
		var gmcsFile = Path.Combine( Application.dataPath, "gmcs.rsp" );
		
		if( File.Exists( smcsFile ) )
			File.Delete( smcsFile );

		if( File.Exists( gmcsFile ) )
			File.Delete( gmcsFile );
	}
	
	
	private void writeFiles( string data )
	{
		var smcsFile = Path.Combine( Application.dataPath, "smcs.rsp" );
		var gmcsFile = Path.Combine( Application.dataPath, "gmcs.rsp" );
		
		// -define:debug;poop
		File.WriteAllText( smcsFile, data );
		File.WriteAllText( gmcsFile, data );
	}
	
	
	private bool defineEditor( GlobalDefine define )
	{
		EditorGUILayout.BeginHorizontal();
		
		define.define = EditorGUILayout.TextField( define.define );
		define.enabled = EditorGUILayout.Toggle( define.enabled );
		
		var remove = false;
		if( GUILayout.Button( "Remove" ) )
			remove = true;
		
		EditorGUILayout.EndHorizontal();
		
		return remove;
	}


	// Called when the 'save' button is pressed
    void OnWizardCreate()
    {
		// .Net 2.0 Subset: smcs.rsp
		// .Net 2.0: gmcs.rsp
		// -define:debug;poop
    }
	    
	    
    void OnWizardOtherButton()
    {
    	this.Close();
    }

}