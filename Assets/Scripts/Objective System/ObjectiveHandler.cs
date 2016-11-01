using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 */

public class ObjectiveHandler : MonoBehaviour {

	public static ObjectiveHandler instance;								// Only allow a single instance of this script to exist
	public List<ObjectiveBase> objectives = new List<ObjectiveBase>();		// A list of all objectives
	public List<MBAction> onPass = new List<MBAction>();					// List of actions executed once all primary objectives are passed
	private bool passed = false;

	void Awake ()
	{
		if (!instance)
			instance = this;
		else
		{
			Destroy(this);
			Debug.LogWarning("More than one instance of class ObjectiveHandler exists. Deleting duplicate instance.");
		}

		if (objectives.Count <= 0)
			Debug.LogWarning("ObjectiveHandler has no specified objectives.");
	}

	void Update () 
	{
		if (objectives.Count > 0)
		{
			if (!passed)
			{
				// Have all objectives been completed?
				bool allPass = true;
				foreach (ObjectiveBase o in objectives)
				{
					if (o != null)
					{
						bool thisPass = o.Evaluate();
						if (!thisPass && !o.optional && allPass)
							allPass = false;

						// Display tick based on pass state
						if (o.tick)
							o.tick.enabled = thisPass;
					}
				}

				// If passed, run onPass scripts.
				if (allPass)
					OnPass();
			}
		}
	}

	private void OnPass ()
	{
		// Only run once
		if (!passed)
		{
			passed = true;

			// Run all scripts
			foreach (MBAction action in onPass)
			{
				if (action)
					action.Execute();
			}
		}
	}
}
