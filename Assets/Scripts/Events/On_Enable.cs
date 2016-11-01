using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class On_Enable : MonoBehaviour {

	public List<MBAction> Actions = new List<MBAction>();			// List of actions to execute on collision

	void OnEnable () 
	{
		foreach (MBAction action in Actions) 
		{
			if (action)
				action.Execute();
		}
	}
}
