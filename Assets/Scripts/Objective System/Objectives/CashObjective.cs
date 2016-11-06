using UnityEngine;
using System.Collections;

public class CashObjective : ObjectiveBase {

	public int targetProfit = 0;

	void Start ()
	{
		if (targetProfit < 0)
		{
			targetProfit = 0;
			Debug.LogWarning("Target Profit was negative in CashObjective script on: " + transform.name);
		}
	}

	void Update () 
	{
		Evaluate();
	}

	protected override bool inEvaluate ()
	{
		if (GameStatistics.LevelScore >= targetProfit)
			return true;
		return false;
	}
}
