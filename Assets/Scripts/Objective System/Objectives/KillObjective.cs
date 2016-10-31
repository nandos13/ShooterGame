using UnityEngine;
using System.Collections;

public class KillObjective : ObjectiveBase {

	public Health target;

	void Update () 
	{
		Evaluate();
	}

	protected override bool inEvaluate ()
	{
		if (target)
		{
			// Check if the target is dead
			if (!target.Alive)
				return true;
			else
				return false;
		}
		else
		{
			Debug.LogWarning("No target specified in script KillObjective on: " + transform.name);
			return false;
		}
	}
}
