using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// JobManager is just a proxy object so we have a launcher for the coroutines
public class JobManager : MonoBehaviour
{	
	// only one JobManager can exist. We use a singleton pattern to enforce this.
	static JobManager _instance = null;

	public static JobManager instance {
		get {
			if (!_instance) {
				// check if an JobManager is already available in the scene graph
				_instance = FindObjectOfType (typeof(JobManager)) as JobManager;

				// nope, create a new one
				if (!_instance) {
					var obj = new GameObject ("JobManager");
					_instance = obj.AddComponent<JobManager> ();
				}
			}

			return _instance;
		}
	}

	void OnApplicationQuit ()
	{
		// release reference on exit
		_instance = null;
	}

}

public class Job
{
	public event System.Action<bool> jobComplete;
	
	private bool _running;

	public bool running { get { return _running; } }
	
	private bool _paused;

	public bool paused { get { return _paused; } }
	
	private IEnumerator _coroutine;
	private bool _jobWasKilled;
	private Stack<Job> _childJobStack;
	
	
	#region constructors
	
	public Job (IEnumerator coroutine) : this( coroutine, true )
	{
	}
	
	public Job (IEnumerator coroutine, bool shouldStart)
	{
		_coroutine = coroutine;
		
		if (shouldStart)
			start ();
	}
	
	#endregion
	
	
	#region static Job makers
	
	public static Job make (IEnumerator coroutine)
	{
		return new Job (coroutine);
	}
	
	public static Job make (IEnumerator coroutine, bool shouldStart)
	{
		return new Job (coroutine, shouldStart);
	}
	
	#endregion
	
	private IEnumerator doWork ()
	{
		// null out the first run through in case we start paused
		yield return null;
		
		while (_running) {
			if (_paused) {
				yield return null;
			} else {
				// run the next iteration and stop if we are done
				if (_coroutine.MoveNext ()) {
					yield return _coroutine.Current;
				} else {
					// run our child jobs if we have any
					if (_childJobStack != null && _childJobStack.Count > 0) {
						Job childJob = _childJobStack.Pop ();
						_coroutine = childJob._coroutine;
					} else
						_running = false;
				}
			}
		}
		
		// fire off a complete event
		if (jobComplete != null)
			jobComplete (_jobWasKilled);
	}
	
	#region public API
	
	public Job createAndAddChildJob (IEnumerator coroutine)
	{
		var j = new Job (coroutine, false);
		addChildJob (j);
		return j;
	}
	
	public void addChildJob (Job childJob)
	{
		if (_childJobStack == null)
			_childJobStack = new Stack<Job> ();
		_childJobStack.Push (childJob);
	}
	
	public void removeChildJob (Job childJob)
	{
		if (_childJobStack.Contains (childJob)) {
			var childStack = new Stack<Job> (_childJobStack.Count - 1);
			var allCurrentChildren = _childJobStack.ToArray ();
			System.Array.Reverse (allCurrentChildren);
			
			for (var i = 0; i < allCurrentChildren.Length; i++) {
				var j = allCurrentChildren [i];
				if (j != childJob)
					childStack.Push (j);
			}
			
			// assign the new stack
			_childJobStack = childStack;
		}
	}

	public void start ()
	{
		_running = true;
		JobManager.instance.StartCoroutine (doWork ());
	}
	
	public IEnumerator startAsCoroutine ()
	{
		_running = true;
		yield return JobManager.instance.StartCoroutine( doWork() );
	}
	
	public void pause ()
	{
		_paused = true;
	}
	
	public void unpause ()
	{
		_paused = false;
	}
	
	public void kill ()
	{
		_jobWasKilled = true;
		_running = false;
		_paused = false;
	}
	
	public void kill (float delayInSeconds)
	{
		var delay = (int)(delayInSeconds * 1000);
		new System.Threading.Timer (obj =>
		{
			lock (this) {
				kill ();
			}
		}, null, delay, System.Threading.Timeout.Infinite);
	}
	
	#endregion
}