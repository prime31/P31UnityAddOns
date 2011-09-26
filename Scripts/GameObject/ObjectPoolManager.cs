using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ObjectPoolManager
// Author: William Ravaine - spk02@hotmail.com (Spk on Unity forums)
// Date: 15-11-09
//
// <LEGAL BLABLA>
// This code package is provided "as is" with no express or implied warranty of any kind. You may use this code for both
// commercial and non-commercial use without any restrictions. Any modification and/or redistribution of this code should
// include the original author's name, contact information and also this paragraph.
// </LEGAL BLABLA>
//
// The goal of this class is to avoid costly runtime memory allocation for objects that are created and destroyed very often during
// gameplay (e.g. projectile, enemies, etc). It achieves this by recycling "destroyed" objects from an internal cache instead of physically
// removing them from memory via Object.Destroy().
//
// To use the ObjectPoolManager, you simply need to replace your regular object creation & destruction calls by ObjectPoolManager.createPooled()
// and ObjectPoolManager.destroyPooled(). Here's an exemple:
//
// 1) Without using the ObjectPoolManager:
// Projectile bullet = Instantiate( bulletPrefab, position, rotation ) as Projectile;
// Destroy( bullet.gameObject );
// 
// 2) Using the ObjetPoolManager:
// Projectile bullet = ObjectPoolManager.createPooled( bulletPrefab.gameObject, position, rotation ).GetComponent<Bullet>();
// ObjectPoolManager.destroyPooled( bullet.gameObject );
//
// When a recycled object is revived from the cache, the ObjectPoolManager calls its Start() method again, so this object can reset itself as
// if it just got newly created.
//
// When using the ObjectPoolManager with your objects, you need to keep several things in mind:
// 1. You need to be in full control of the creation and destruction of the object (so they go through ObjectPoolManager). This means you shouldn't
//	  use it on objects that use exotic destruction methods (e.g. auto-destroy option on particle effects) because the ObjectPoolManager will
//	  not be able to recycle the object 
// 2. When they get revived from the ObjectPoolManager cache, the pooled objects are responsible for re-initializing themselves as if they had
//	  just been newly created via a regular call Instantiate(). So look out for any dynamic component additions and modifications of the initial
//	  object public fields during gameplay


public class ObjectPoolManager : MonoBehaviour
{
#if UNITY_EDITOR
	// turn this on to activate debugging information
	public bool debug = false;

	// the GUI block where the debugging info will be displayed
	public Rect debugGuiRect = new Rect( 5, 200, 160, 400 );
#endif

	// This maps a prefab to its ObjectPool
	Dictionary<GameObject, ObjectPool> prefab2pool;

	// This maps a game object instance to the ObjectPool that created/recycled it
	Dictionary<GameObject, ObjectPool> instance2pool;

	// Only one ObjectPoolManager can exist. We use a singleton pattern to enforce this.
	static ObjectPoolManager _instance = null;
	public static ObjectPoolManager instance
	{
		get
		{
			if( !_instance )
			{
				// check if an ObjectPoolManager is already available in the scene graph
				_instance = FindObjectOfType( typeof( ObjectPoolManager ) ) as ObjectPoolManager;

				// nope, create a new one
				if( !_instance )
				{
					var obj = new GameObject( "ObjectPoolManager" );
					_instance = obj.AddComponent<ObjectPoolManager>();
				}
			}

			return _instance;
		}
	}


	void OnApplicationQuit()
	{
		// release reference on exit
		_instance = null;
	}


	#region Public Interface (static for convenience)

	// Create a pooled object. This will either reuse an object from the cache or allocate a new one
	public static GameObject createPooled( GameObject prefab, Vector3 position, Quaternion rotation )
	{
		return instance.internalCreate( prefab, position, rotation );
	}


	// Destroy the object now
	public static void destroyPooled( GameObject obj )
	{
		_instance.internalDestroy( obj );
	}


	// Destroy the object after <delay> seconds have elapsed
	public static void destroyPooled( GameObject obj, float delay )
	{
		_instance.StartCoroutine( _instance.internalDestroy( obj, delay ) );
	}

	#endregion

	#region Private implementation

	// Constructor
	void Awake()
	{
		prefab2pool = new Dictionary<GameObject, ObjectPool>();
		instance2pool = new Dictionary<GameObject, ObjectPool>();
	}


	private ObjectPool createPool( GameObject prefab )
	{
		var pool = new ObjectPool();
		pool.prefab = prefab;

		return pool;
	}


	private GameObject internalCreate( GameObject prefab, Vector3 position, Quaternion rotation )
	{
		ObjectPool pool;

		if( !prefab2pool.ContainsKey( prefab ) )
		{
			pool = createPool( prefab );
			prefab2pool[prefab] = pool;
		}
		else
		{
			pool = prefab2pool[prefab];
		}

		// create a new object or reuse an existing one from the pool
		GameObject obj = pool.instantiate( position, rotation );

		// remember which pool this object was created from
		instance2pool[obj] = pool;

		return obj;
	}


	private void internalDestroy( GameObject obj )
	{
		if( instance2pool.ContainsKey( obj ) )
		{
			//Debug.Log( "Recyling object " + obj.name );
			var pool = instance2pool[obj];
			pool.recycle( obj );
		}
		else
		{
			// This object was not created through the ObjectPoolManager, give a warning and destroy it the "old way"
			Debug.LogWarning( "Destroying non-pooled object " + obj.name );
			Object.Destroy( obj );
		}
	}


	// must be run as coroutine
	private IEnumerator internalDestroy( GameObject obj, float delay )
	{
		yield return new WaitForSeconds( delay );
		internalDestroy( obj );
	}

	#endregion

#if UNITY_EDITOR
	void OnGUI()
	{
		if( debug )
		{
			GUILayout.BeginArea( debugGuiRect );
			GUILayout.BeginVertical();

			GUILayout.Label( "Pools: " + prefab2pool.Count );

			foreach( var pool in prefab2pool.Values )
				GUILayout.Label( pool.prefab.name + ": " + pool.Count );

			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}
#endif

}
