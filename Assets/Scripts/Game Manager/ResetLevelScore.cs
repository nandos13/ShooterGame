using UnityEngine;
using System.Collections;

public class ResetLevelScore : MBAction {

	public override void Execute ()
	{
		GameStatistics.resetLevelScore();
	}
}
