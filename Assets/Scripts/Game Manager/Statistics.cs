using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Static Statistics class to hold game stats (Score, Time, etc)
 */

[System.Serializable]
public class GameStatistics
{
	/* SCORE TRACKING */
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
		_levelScore += s;
	}


	/* WEAPON-ATTACHMENTS */
	// SIDE NOTE: I realize this is a very unclean way of achieving weapon attachments but it works well enough and does not need to be modular for this project.
	[SerializeField] public static bool _atMainAuto = false;
	[SerializeField] public static bool _atShotty = false;
	[SerializeField] public static bool _atFlameThrower = false;
	[SerializeField] public static bool _atLaser = false;
	[SerializeField] public static bool _atMinigun = false;
	[SerializeField] public static bool _atBomber = false;
	[SerializeField] public static bool _atRPG = false;
	public static void unlockAttachment (GUN_ATTACHMENTS type, int cost)
	{
		/* Attempts to unlock an attachment for the player weapon */
	
		switch (type)
		{
		case GUN_ATTACHMENTS.MainAuto:
			inUnlock (_atMainAuto, cost);
			break;
	
		case GUN_ATTACHMENTS.Shotty:
			if (_atMainAuto)
				inUnlock (_atShotty, cost);
			break;
	
		case GUN_ATTACHMENTS.FlameThrower:
			if (_atMainAuto)
				inUnlock (_atFlameThrower, cost);
			break;
	
		case GUN_ATTACHMENTS.Laser:
			if (_atShotty)
				inUnlock (_atLaser, cost);
			break;
	
		case GUN_ATTACHMENTS.Minigun:
			if (_atFlameThrower)
				inUnlock (_atMinigun, cost);
			break;
	
		case GUN_ATTACHMENTS.Bomber:
			if (_atLaser)
				inUnlock (_atBomber, cost);
			break;
	
		case GUN_ATTACHMENTS.RPG:
			if (_atMinigun)
				inUnlock (_atRPG, cost);
			break;
	
		default:
			break;
		}
	}
	
	private static void inUnlock (bool attachment, int cost)
	{
		if ( (_score > cost) && !attachment)
		{
			// Not currently unlocked, and unlock is affordable
			_score -= cost;
			attachment = true;
		}
	}
}
