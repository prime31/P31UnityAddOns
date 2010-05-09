@MenuItem("Assets/Overwrite Texture")
static function Apply ()
{   
   var texture : Texture2D = Selection.activeObject as Texture2D;
   if (texture == null)
   {
      EditorUtility.DisplayDialog("Select Texture", "You Must Select a Texture first!", "Ok");
      return;
   }
   
   var path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
   if (path.Length != 0)
   {
      var www = new WWW ("file:// " + path);
      while (!www.isDone)
         ;
      www.LoadImageIntoTexture(texture);
   }
}