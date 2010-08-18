using UnityEngine;
using System.Collections;


public class TextureQuality : MonoBehaviour
{
	// Used to set the texture mipmap level based on device
	void Awake()
	{
		// Quality setting for in the editor
		if( Application.isEditor )
		{
			QualitySettings.currentLevel = QualityLevel.Simple;
			Debug.Log( "Texture quality in editor set to: " + QualitySettings.currentLevel );
			return;
		}
		
		if( iPhoneSettings.generation == iPhoneGeneration.iPhone4 || iPhoneSettings.generation == iPhoneGeneration.iPhone3GS
			|| iPhoneSettings.generation == iPhoneGeneration.iPad1Gen )
			QualitySettings.currentLevel = QualityLevel.Beautiful;
		else
			QualitySettings.currentLevel = QualityLevel.Good;
		
		Debug.Log( "Texture quality set to: " + QualitySettings.currentLevel );
		
		// disable ourself
		this.enabled = false;
	}
}
