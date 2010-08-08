using UnityEngine;
using System.Runtime.InteropServices;

public class UIBinding
{
    [DllImport("__Internal")]
    private static extern void _activateUIWithController( string controllerName );
 
    public static void activateUIWithController( string controllerName )
    {
        // Call plugin only when running on real device
        if( Application.platform != RuntimePlatform.OSXEditor )
		{
			Debug.Log( string.Format( "calling activate with name: {0}", controllerName ) );
			_activateUIWithController( controllerName );
		}
    }
	
	
    [DllImport("__Internal")]
    private static extern void _deactivateUI();
 
    public static void deactivateUI()
    {
        // Call plugin only when running on real device
        if( Application.platform != RuntimePlatform.OSXEditor )
			_deactivateUI();
    }
}
