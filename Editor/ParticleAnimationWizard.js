import System.IO;
class ParticleAnimationWizard extends ScriptableWizard
{
	var fileName = "New Animation";
	var frameCount = 16.00;
	var frameSize = 64;
	var fudgeCameraScale = 1.00;	var object : GameObject;	var rotationAxis : Vector3 = Vector3.zero;
	var lightDirection : Vector3 = Vector3(0, -1, 1);
	var lightColor : Color = Color.gray;
	var ambientColor : Color = Color.gray;	@MenuItem("Assets/Create Particle Animation...")	static function CreateWizard ()
	{		ScriptableWizard.DisplayWizard("Create Particle Animation", ParticleAnimationWizard, "Create");		}
	
	function OnWizardUpdate ()
	{
		helpString = "";
		errorString = "";
		
		if (!object)
			object = Selection.activeGameObject;
		
		if (fileName == String.Empty)
			errorString = "Give your new animated texture a fileName";
		else if (frameCount != 4 && frameCount != 8 && frameCount != 16 && frameCount != 32)
			errorString = "Set the frame count to a sane power of two";
		else if (frameSize != 16 && frameSize != 32 && frameSize != 64 && frameSize != 128 && frameSize != 256)
			errorString = "Set the frame size to a sane power of two";		else if (!object || !object.renderer)
			errorString = "Please set the object to something with a renderer";
		else if (rotationAxis == Vector3.zero)
			errorString = "Please set the rotation direction to non-zero";
		else if (lightDirection == Vector3.zero)
			errorString = "Please set the light direction to non-zero";
		else if (fudgeCameraScale == 0)
			errorString = "Your camera is so small";
		else
			helpString = "Good to go";
			
		isValid = errorString == "";
	}
		function OnWizardCreate ()
	{
		ready = true;
		if(fileName == String.Empty) ready = false;
		else if(frameCount != 4 && frameCount != 8 && frameCount != 16 && frameCount != 32) ready = false;
		else if(frameSize != 16 && frameSize != 32 && frameSize != 64 && frameSize != 128 && frameSize != 256) ready = false;		else if(!object || !object.renderer) ready = false;
		else if(rotationAxis == Vector3.zero) ready = false;
		else if(lightDirection == Vector3.zero) ready = false;
		else if(fudgeCameraScale == 0) ready = false;
		
		if(!ready)
		{
			Debug.Log("Create Particle Animation failed: " + helpString);
			return;	
		}
		
		var radius : float = object.renderer.bounds.size.magnitude / 2.00;
		
		var lighte = new GameObject("__Light", Light);
		lighte.transform.rotation = Quaternion.LookRotation(lightDirection);
		lighte.light.renderMode = LightRenderMode.ForcePixel;
		lighte.light.type = LightType.Directional;
		lighte.light.color = lightColor;
		
		var bgColor = Color(1, 0, 1, 1);
		
		var cam = new GameObject("__Camera", Camera);
		cam.transform.rotation = Quaternion.identity;
		cam.transform.position = object.transform.position - (Vector3.forward * radius * 2);
		cam.camera.orthographic = true;
		cam.camera.orthographicSize = radius * fudgeCameraScale;
		cam.camera.nearClipPlane = radius;
		cam.camera.farClipPlane = radius * 3;
		cam.camera.backgroundColor = bgColor;
		//cam.camera.cullingMask = 1 << 10;
		
		var rt = new RenderTexture(frameSize, frameSize, 16);
		rt.isPowerOfTwo = true;
		cam.camera.targetTexture = rt;
		
		var oldLayer = object.layer;
		var oldfog = RenderSettings.fog;
		var oldAmbient = RenderSettings.ambientLight;
		RenderSettings.fog = false;
		RenderSettings.ambientLight = ambientColor;
		//object.layer = 10;
		
		var copyTex = new Texture2D( frameSize, frameSize, TextureFormat.RGB24, false );
		var tex = new Texture2D( frameSize * frameCount, frameSize, TextureFormat.ARGB32, false );
		
		var i = 0.00;
		while(i < frameCount)
		{
			object.transform.rotation = Quaternion.AngleAxis(i / frameCount * 360.00, rotationAxis);
			cam.camera.Render();
			RenderTexture.active = rt;
			copyTex.ReadPixels( Rect(0, 0, frameSize, frameSize), 0, 0);
			pixels = copyTex.GetPixels(0, 0, frameSize, frameSize);
			tex.SetPixels(i * frameSize, 0, frameSize, frameSize, pixels);
			i++;
		}
		
		var avgColorV : Vector3;
		var count = 0;
		
		var x =0;
		var y = 0;
		while(x < tex.width)
		{
			y = 0;
			while(y < tex.height)
			{
				curP = tex.GetPixel(x, y);
				if(curP != bgColor)
				{
					avgColorV.x += curP.r;
					avgColorV.y += curP.g;
					avgColorV.z += curP.b;
					count++;
				}
				y++;	
			}	
			x++;
		}
		
		if(count > 0) avgColorV /= count;
		var avgColor = Color(avgColorV.x, avgColorV.y, avgColorV.z, 0);
		
		x =0;
		while(x < tex.width)
		{
			y = 0;
			while(y < tex.height)
			{
				curP = tex.GetPixel(x, y);
				if(curP == bgColor)
				{
					tex.SetPixel(x, y, avgColor);
				}
				y++;	
			}	
			x++;
		}
		
		object.transform.rotation = Quaternion.AngleAxis(0.00, rotationAxis);
		
		tex.Apply();
		var bytes = tex.EncodeToPNG();
   		File.WriteAllBytes(Application.dataPath + "/Editor/" + fileName + ".png", bytes);

   		RenderTexture.active = null;

   		DestroyImmediate( copyTex );
   		DestroyImmediate( tex );
		DestroyImmediate( rt );
		DestroyImmediate( cam );
		DestroyImmediate( lighte );

		RenderSettings.fog = oldfog;
		RenderSettings.ambientLight = oldAmbient;
		object.layer = oldLayer;
		
		Debug.Log("Create Particle Animation done, saved to:  " + "Assets/Editor/" + fileName + ".png");	}
	
	function CompareColor (one : Color) : boolean
	{
		if(one.r > 0.9 && one.g < 0.1 && one.b > 0.9) return true;
		else return false;
	}
}

