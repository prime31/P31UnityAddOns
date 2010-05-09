@MenuItem ("Terrain/Apply Splatmap")

static function ApplySplatmap () {
	var splatmap : Texture2D = Selection.activeObject as Texture2D;
	if (splatmap == null) { 
		EditorUtility.DisplayDialog("No texture selected", "Please select a texture", "Cancel"); 
		return; 
	}
	if (splatmap.format != TextureFormat.ARGB32) {
		EditorUtility.DisplayDialog("Wrong format", "Splatmap must be in RGBA 32 bit format", "Cancel"); 
		return;
	}

	var w = splatmap.width;
	if (splatmap.height != w) {
		EditorUtility.DisplayDialog("Wrong size", "Splatmap width and height must be the same", "Cancel"); 
		return;
	}
	if (Mathf.ClosestPowerOfTwo(w) != w) {
		EditorUtility.DisplayDialog("Wrong size", "Splatmap width and height must be a power of two", "Cancel"); 
		return;	
	}

	var terrain = Terrain.activeTerrain.terrainData;
	terrain.alphamapResolution = w;
	var splatmapData = terrain.GetAlphamaps(0, 0, w, w);
	var mapColors = splatmap.GetPixels();
	if (splatmapData.Length < mapColors.Length*4) {
		EditorUtility.DisplayDialog("Add textures", "The terrain must have at least four textures", "Cancel"); 
		return;	
	}
	
	for (z = 0; z < 4; z++) {
		for (y = 0; y < w; y++) {
			for (x = 0; x < w; x++) {
				splatmapData[x,y,z] = mapColors[((w-1)-x)*w + y][z];
			}
		}
	}
	terrain.SetAlphamaps(0, 0, splatmapData);
}