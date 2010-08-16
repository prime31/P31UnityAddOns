using UnityEngine;
using System.Collections;


/**
 * Basic usage with an example of a bullet.  Assume spawnerPrefab is a bullet prefab and recycler is an ivar.
 * 
 * Start:
 		recycler = new ObjectRecycler( spawnerPrefab, 10 );
 	
   Shoot:
		// Grab the next free object from our recycler
		GameObject bullet = recycler.nextFree;
		
		// Make sure we had something before proceeding
		if( bullet )
		{
			// Shoot our bullet in a CoRoutine so we can destoy it a few seconds later
			StartCoroutine( shootBullet( bullet ) );
		}
	
	ShootBullet Coroutine:
		// Set bullet transforma nd prepare to apply forces
		bullet.active = true;
		
		// Wait for 3 seconds
		yield return new WaitForSeconds( 3.0f );
		
		// Recycle the bullet
		recycler.freeObject( bullet );
 * 
 */
public class ObjectRecycler
{
    private GameObject[] objectStore;
    private bool[] availableObjects;
    private int nextFreeLoopStart = 0;


    public ObjectRecycler( GameObject go, int maxObjects )
    {
        objectStore = new GameObject[maxObjects];
        availableObjects = new bool[maxObjects];

        for( int i = 0; i < maxObjects; i++ )
        {
            // Create a new instance and set ourself as the recycleBin
            GameObject newTransform = Object.Instantiate( go ) as GameObject;
			newTransform.gameObject.active = false;

            // Add it to our objectStore and set it to available
            objectStore.SetValue( newTransform, i );
            availableObjects[i] = true;
        }
    }


    // Gets the next available free object or null
    public GameObject nextFree
    {
        get
        {
            for( ; nextFreeLoopStart < availableObjects.Length; nextFreeLoopStart++ )
            {
                if( availableObjects[nextFreeLoopStart] )
                {
                    // Set the object to unavailable and return it
                    availableObjects[nextFreeLoopStart] = false;
                    return objectStore.GetValue( nextFreeLoopStart ) as GameObject;
                }
            }

            // We purposely do not reset our nextFreeLoopStart here because it will get reset when the next object gets freed
            return null;
        }
    }


    // MUST be called by any object that wants to be reused
    public void freeObject( GameObject objectToFree )
    {
        int index = System.Array.IndexOf( objectStore, objectToFree );
        if( index >= 0 )
        {
            // Reset the nextFreeLoopStart if this object has a lower index
            if( index < nextFreeLoopStart )
                nextFreeLoopStart = index;

			// Make the object inactive
			objectToFree.gameObject.active = false;

            // Set the object to available
            availableObjects[index] = true;
        }
    }


}
