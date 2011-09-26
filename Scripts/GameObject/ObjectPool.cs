using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// The ObjectPool is the storage class for pooled objects of the same kind (e.g. "Pistol Bullet", or "Enemy A")
// This is used by the ObjectPoolManager and is not meant to be used separately
public class ObjectPool : System.Object
{
	// The type of object this pool is handling
	public GameObject prefab;

	// This stores the cached objects waiting to be reactivated
	public Queue<GameObject> pool;	


	// How many objects are currently sitting in the cache
	public int Count
	{
		get { return pool.Count; }
	}


	public ObjectPool()
	{
		pool = new Queue<GameObject>();
	}


	public GameObject instantiate( Vector3 position, Quaternion rotation )
	{
		GameObject obj;

		// if we don't have any object already in the cache, create a new one
		if( pool.Count == 0 )
		{
			obj = Object.Instantiate( prefab, position, rotation ) as GameObject;
		}
		else // else pull one from the cache
		{
			obj = pool.Dequeue();

			// reactivate the object
			obj.transform.parent = null;
			obj.transform.position = position;
			obj.transform.rotation = rotation;
			obj.SetActiveRecursively( true );

			// Call Start again
			obj.SendMessage( "Start", SendMessageOptions.DontRequireReceiver );
		}

		return obj;
	}


	// put the object in the cache and deactivate it
	public void recycle( GameObject obj )
	{
		// deactivate the object
		obj.SetActiveRecursively( false );

		// put the recycled object in this ObjectPool's bucket
		obj.transform.parent = ObjectPoolManager.instance.gameObject.transform;

		// put object back in cache for reuse later
		pool.Enqueue( obj );
	}

}
