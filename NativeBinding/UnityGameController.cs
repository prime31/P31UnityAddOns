using UnityEngine;


public class UnityGameController : MonoBehaviour
{
    void Awake()
    {
		// Set the GameObject name to the class name for easy access from Obj-C
		gameObject.name = this.GetType().ToString();
    }
	
	
	public void loadLevel( string scene )
	{
		this.ao = Application.LoadLevel( scene );
		
		// Be sure to deactivate the UI
		UIBinding.deactivateUI();
	}
	
	
	public void loadLevelAsync( string scene )
	{
		this.ao = Application.LoadLevelAdditiveAsync( scene );
		
		// Be sure to deactivate the UI
		UIBinding.deactivateUI();
	}
	
	
	public void loadLevelAdditive( string scene )
	{
		Application.LoadLevelAdditive( scene );
		
		// Be sure to deactivate the UI
		UIBinding.deactivateUI();
	}
	
	
	public void loadLevelAdditiveAsync( string scene )
	{
		this.ao = Application.LoadLevelAdditiveAsync( scene );
		
		// Be sure to deactivate the UI
		UIBinding.deactivateUI();
	}

}
