import System.IO;

class LightmapWizard extends ScriptableWizard
{
    var size = 512;
    var shadowMultisampling = 2;
    var heightmapShadingContrast = 2.00;
    var hardShadowAmbientLight = 0.1;
    var brightColor = 0.7;
    var darkColor = 0.05;
    var treeBrightness = 0.4;
    var water : Transform;
    var light : Light;
    var terrain : Terrain;
    var decal : Texture2D;
    private var terrainData : TerrainData;
    
    @MenuItem("Terrain/CustomLightmap...")
    static function CreateWizard ()
    {
        ScriptableWizard.DisplayWizard("Custom Lightmap", LightmapWizard, "Bake");  
    }
    
    function OnWizardUpdate ()
    {
        helpString = "I'm ready!";
        if(!terrain) {
            helpString = "I need a terrain!";
        } else {
            terrainData = terrain.terrainData;
        }
        if(!light || light.type != LightType.Directional) helpString = "I need a directional light!";
        
        
        
    }
    
    function OnWizardCreate ()
    {
        // Calculate diffuse lighting and terrain shadows
        var diffuse = new Texture2D(size, size, TextureFormat.ARGB32, true);
        var treeColor = new Texture2D(size, size, TextureFormat.ARGB32, true);
        for(var x = 0.00; x < size; x++) {
            for(var y = 0.00; y < size; y++) {
                var normal = terrainData.GetInterpolatedNormal(x/size, y/size);
                var worldPos = Vector3((x/size)*terrainData.size.x,0,(y/size)*terrainData.size.z);
                worldPos += normal;
                worldPos.y += terrain.SampleHeight(worldPos);
                brightness = Mathf.Pow((Vector3.Dot(normal, -light.transform.forward)*heightmapShadingContrast+1)*0.5, 2);
                if(Physics.Raycast(worldPos, -light.transform.forward)) brightness = 0;
                diffuse.SetPixel(x, y, Color(brightness, 0, 0, 0));
                treeColor.SetPixel(x, y, Color.Lerp(Color.black, Color.white, brightness + treeBrightness));
            }
        }
        diffuse.Apply(); 
        treeColor.Apply();
        
        // Calculate shadows from trees!
        var shadowMap = Shadowmap(size * shadowMultisampling, size);
        
        // Find the correct texture to write to
        var targetTexture : Texture2D;
        if(terrainData.lightmap) {
            targetTexture = terrainData.lightmap;
            if(targetTexture.width != size || targetTexture.height != size) targetTexture.Resize(size, size);
        } else {
            targetTexture = new Texture2D(size, size, TextureFormat.ARGB32, true);
            targetTexture.name = "Lightmap";
            AssetDatabase.AddObjectToAsset(targetTexture, terrainData);
        }
        
        // Blend ingredients in to a tasty lightmap smoothie in the result texture
        for(x = 0; x < size; x++) {
            for(y = 0; y < size; y++) {
                var d = diffuse.GetPixel(x, y);
                var s = shadowMap.GetPixel(x, y);
                var value = Mathf.Lerp(darkColor, brightColor, Mathf.Max(d.r*s.r, hardShadowAmbientLight));
                targetTexture.SetPixel(x, y, Color(value, value, value, 1));
            }
        }
        
        targetTexture.Apply();
        terrainData.lightmap = targetTexture;
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(terrainData));
        
        UnityEditor.TerrainLightmapper.UpdateTreeLightmapColor(treeColor, terrainData);
        
        DestroyImmediate(shadowMap);
        DestroyImmediate(diffuse);  
        DestroyImmediate(treeColor);    
    }
    
    
    
    
    function Shadowmap(sampleSize : int, resultSize : int) : Texture2D
    {
        var hijackShaderText =
"Shader \"Hidden/TerrainEngine/Splatmap/VertexLit-BaseMap\" {" +
"Properties { " +
"   _LightMap (\"LightMap (RGB)\", 2D) = \"white\" {} " +
"   _BaseMap (\"BaseMap (RGB)\", 2D) = \"white\" {} " +
"} " +
"SubShader { " +
"   Tags { \"SplatCount\" = \"0\" } " +
"   Pass { " +
"      Tags {\"LightMode\" = \"Always\" } " +
"      Fog { Mode Off } " +
"Program \"\" {" +
"SubProgram \"opengl \" {" +
"Keywords { } " +
"Bind \"vertex\", Vertex " +
"Matrix 9, [_MyShadowMatrix] " +
"Matrix 13, [_MyInverseView] " +
"\"!!ARBvp1.0 " +
"PARAM c[17] = { program.local[0]," +
"      state.matrix.modelview[0]," +
"      state.matrix.mvp," +
"      program.local[9..16] };" +
"TEMP R0;" +
"TEMP R1;" +
"DP4 R1.w, vertex.position, c[4];" +
"DP4 R1.z, vertex.position, c[3];" +
"DP4 R1.x, vertex.position, c[1];" +
"DP4 R1.y, vertex.position, c[2];" +
"DP4 R0.w, R1, c[16];" +
"DP4 R0.z, R1, c[15];" +
"DP4 R0.x, R1, c[13];" +
"DP4 R0.y, R1, c[14];" +
"DP4 result.texcoord[0].w, R0, c[12];" +
"DP4 result.texcoord[0].z, R0, c[11];" +
"DP4 result.texcoord[0].y, R0, c[10];" +
"DP4 result.texcoord[0].x, R0, c[9];" +
"DP4 result.position.w, vertex.position, c[8];" +
"DP4 result.position.z, vertex.position, c[7];" +
"DP4 result.position.y, vertex.position, c[6];" +
"DP4 result.position.x, vertex.position, c[5];" +
"END" +
"\"" +
"}" +
"}" +
"      SetTexture [_MyShadowMap] { combine texture }" +
"   }" +
"}" +
"}";
        
        var oldSplatDist = terrain.basemapDistance;
        var oldTreeDist = terrain.treeDistance;
        var oldBillDist = terrain.treeBillboardDistance;
        var oldMaxCount = terrain.treeMaximumFullLODCount;
        var oldDetailDist = terrain.detailObjectDistance;
        var oldLightmap = terrain.lighting;
        var oldLayer = terrain.gameObject.layer;
        
        var terrSize = terrain.terrainData.size;
        var center = terrSize * 0.5;
        var terrRadius = center.magnitude;
        terrRadius *= 0.8;
        
        
        // render shadow map
        var cam = new GameObject("__ShadowCamera", Camera);
        cam.transform.rotation = light.transform.rotation;
        cam.transform.position = center - cam.transform.forward * terrRadius * 1.1;
        cam.camera.orthographic = true;
        cam.camera.orthographicSize = terrRadius;
        cam.camera.nearClipPlane = terrRadius * 0.1;
        cam.camera.farClipPlane = terrRadius * 2.2;
        cam.camera.backgroundColor = Color.white;
        cam.camera.cullingMask = 1 << 10;
        cam.camera.clearFlags = CameraClearFlags.Color;
        
        var rt = new RenderTexture(resultSize, resultSize, 16);
        rt.isPowerOfTwo = true;
        cam.camera.targetTexture = rt;
        
        var oldfog = RenderSettings.fog;
        RenderSettings.fog = true;
        var oldAmbient = RenderSettings.ambientLight;
        RenderSettings.ambientLight = Color.black;
        var olddensity = RenderSettings.fogDensity;
        RenderSettings.fogDensity = 0.1;
        var oldcolor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.white;
        
        light.enabled = false;
        
        terrain.basemapDistance = 0;
        terrain.treeDistance = 100000;
        terrain.treeBillboardDistance = 0;
        terrain.treeMaximumFullLODCount = 100000;
        terrain.detailObjectDistance = 0;
        terrain.lighting = TerrainLighting.Vertex;
        terrain.gameObject.layer = 10;
        cam.camera.Render();
    
        // setup shadow matrix  
        var matrix : Matrix4x4;
        var texmatrix = Matrix4x4.identity;
        texmatrix[0,0] = 0.5;
        texmatrix[1,1] = 0.5;
        texmatrix[2,2] = 0.5;
        texmatrix[0,3] = 0.5;
        texmatrix[1,3] = 0.5;
        texmatrix[2,3] = 0.5;
        matrix = texmatrix * cam.camera.projectionMatrix * cam.camera.worldToCameraMatrix;
        
        Shader.SetGlobalMatrix( "_MyShadowMatrix", matrix );
        Shader.SetGlobalTexture("_MyShadowMap", rt);
        DestroyImmediate(cam);
        
        // render top-down view with the shadow map
        var cam2 = new GameObject("__TopCamera", Camera);
        cam2.transform.rotation = Quaternion.Euler( 90, 0, 0 );
        cam2.transform.position = Vector3( center.x, terrSize.y + 0.1, center.z );
        cam2.camera.orthographic = true;
        cam2.camera.orthographicSize = center.x;
        cam2.camera.nearClipPlane = 0.1;
        cam2.camera.farClipPlane = terrSize.y + 0.2;
        cam2.camera.backgroundColor = Color.white;
        cam2.camera.cullingMask = 1 << 10;
        cam2.camera.clearFlags = CameraClearFlags.Color;
        
        var rt2 = new RenderTexture(resultSize, resultSize, 16);
        rt2.isPowerOfTwo = true;
        cam2.camera.targetTexture = rt2;
        
        terrain.basemapDistance = 0;
        terrain.treeDistance = 0;
        terrain.treeBillboardDistance = 0;
        terrain.treeMaximumFullLODCount = 0;
        terrain.detailObjectDistance = 0;
        
        Shader.SetGlobalMatrix("_MyInverseView", cam2.camera.cameraToWorldMatrix);
        
        RenderSettings.fog = oldfog;
        RenderSettings.ambientLight = oldAmbient;
        RenderSettings.fogDensity = olddensity;
        RenderSettings.fogColor = oldcolor;
        
        var mat = new Material(hijackShaderText); // hijack terrain shader!
        EditorUtility.ImportShader(mat.shader, hijackShaderText);
        
        cam2.camera.Render();
        
        EditorUtility.ImportShader(mat.shader, "Shader \"__Dummy\" { SubShader{Pass{}}}");
        DestroyImmediate(mat.shader);
        DestroyImmediate(mat);
        
        DestroyImmediate(cam2);
        
        light.enabled = true;
        
        DestroyImmediate(rt);
        
        terrain.basemapDistance = oldSplatDist;
        terrain.treeDistance = oldTreeDist;
        terrain.treeBillboardDistance = oldBillDist;
        terrain.treeMaximumFullLODCount = oldMaxCount;
        terrain.detailObjectDistance = oldDetailDist;
        terrain.lighting = oldLightmap;
        terrain.gameObject.layer = oldLayer;
        
        // Save as PNG
        var oldrt = RenderTexture.active;
        RenderTexture.active = rt2;
        var width = rt2.width;
        var height = rt2.height;
        var tex = new Texture2D( width, height, TextureFormat.RGB24, false );
        tex.ReadPixels( Rect(0, 0, width, height), 0, 0 );
        tex.Apply();
        RenderTexture.active = oldrt;
        DestroyImmediate(rt2);
        return tex;
    }
}