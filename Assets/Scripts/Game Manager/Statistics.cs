using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Static Statistics class to hold game stats (Score, Time, etc)
 */

[System.Serializable]
public class GameStatistics
{
	[SerializeField] private static int _score = 0;
	private static int _levelScore = 0;
	public static int Score
	{
		get { return _score; }
	}
	public static int LevelScore
	{
		get { return _levelScore; }
	}
	public static void AddScore (int s)
	{
		_score += s;
		_levelScore += s;
	}
}
