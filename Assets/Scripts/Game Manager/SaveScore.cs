using UnityEngine;
using System.Collections;

public class SaveScore : MBAction {

	public override void Execute ()
	{
		GameStatistics.applyScore();
	}
}
