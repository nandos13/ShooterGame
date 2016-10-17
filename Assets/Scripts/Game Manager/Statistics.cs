using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Static Statistics class to hold game stats (Score, Time, etc)
 */

public class GameStatistics
{
	private static int _score = 0;
	public static int Score
	{
		get { return _score; }
	}
	public static void AddScore (int s)
	{
		_score += s;
	}
}
