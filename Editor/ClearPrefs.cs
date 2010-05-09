using UnityEngine; 
using UnityEditor; 

public class ClearPrefs 
{ 
   [MenuItem ("Tools/Clear PlayerPrefs")] 
   static void ClearPlayerPrefs() 
   { 
      PlayerPrefs.DeleteAll(); 
   } 
}