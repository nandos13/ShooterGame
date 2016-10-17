using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Adds score when executed.
 */

public class AddScore : MBAction
{
	[Range (-1000, 1000)]
	public int score = 0;

	public override void Execute ()
	{
		GameStatistics.AddScore(score);
	}
}
