using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Run specified scripts on startup
 */

public class On_Startup : MonoBehaviour {

	public List<MBAction> Actions = new List<MBAction>();			// List of actions to execute on collision

	void Start () 
	{
		foreach (MBAction action in Actions) 
		{
			action.Execute();
		}
	}
}
