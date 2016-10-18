using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ActionValue
{
	public MBAction action;
	public float value = 0;

	public ActionValue()
	{
		// Constructor
		value = 0;
	}
}

public class TimedExecutions : MBAction {

	[SerializeField]
	public List<ActionValue> actions = new List<ActionValue>();

	public override void Execute () 
	{
		StartCoroutine(Run());
	}

	IEnumerator Run ()
	{
		foreach (ActionValue av in actions)
		{
			if (av != null)
			{
				// Wait for value time
				yield return new WaitForSeconds (av.value);

				// Execute action
				if (av.action)
					av.action.Execute();
			}
		}
	}
}
