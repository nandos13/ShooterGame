using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

public class TimedExecutions : MonoBehaviour {

	public List<ActionValue> actions = new List<ActionValue>();

	void Start () 
	{
		StartCoroutine(Run());
	}

	IEnumerator Run ()
	{
		foreach (ActionValue av in actions)
		{
			// Wait for value time
			yield return new WaitForSeconds (av.value);

			// Execute action
			av.action.Execute();
		}
	}
}
