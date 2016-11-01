using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KillObjective : ObjectiveBase {

	public List<Health> targets = new List<Health>();

	private bool warningShow = false;

	void Update () 
	{
		Evaluate();
	}

	protected override bool inEvaluate ()
	{
		if (targets.Count > 0)
		{
			bool thisPass = true;
			foreach (Health h in targets)
			{
				if (h)
				{
					if (h.Alive)
					{
						thisPass = false;
						break;
					}
				}
			}

			if (thisPass && tick)
				tick.gameObject.SetActive(true);
			return thisPass;
		}
		else if (!warningShow)
		{
			warningShow = true;
			Debug.LogWarning("No targets specified in script KillObjective on: " + transform.name);
		}
		return false;
	}
}
