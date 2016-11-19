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

	public static void applyScore ()
	{
		_score += _levelScore;
		_levelScore = 0;
	}

	public static void resetLevelScore ()
	{
		_levelScore = 0;
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
	public static bool unlockAttachment (GUN_ATTACHMENTS type, int cost)
	{
		/* Attempts to unlock an attachment for the player weapon */
	
		switch (type)
		{
		case GUN_ATTACHMENTS.MainAuto:
			return inUnlock (ref _atMainAuto, cost);
	
		case GUN_ATTACHMENTS.Shotty:
			if (_atMainAuto)
				return inUnlock (ref _atShotty, cost);
			break;
	
		case GUN_ATTACHMENTS.FlameThrower:
			if (_atMainAuto)
				return inUnlock (ref _atFlameThrower, cost);
			break;
	
		case GUN_ATTACHMENTS.Laser:
			if (_atShotty)
				return inUnlock (ref _atLaser, cost);
			break;
	
		case GUN_ATTACHMENTS.Minigun:
			if (_atFlameThrower)
				return inUnlock (ref _atMinigun, cost);
			break;
	
		case GUN_ATTACHMENTS.Bomber:
			if (_atLaser)
				return inUnlock (ref _atBomber, cost);
			break;
	
		case GUN_ATTACHMENTS.RPG:
			if (_atMinigun)
				return inUnlock (ref _atRPG, cost);
			break;
	
		default:
			return false;
		}
		return false;
	}
	
	private static bool inUnlock (ref bool attachment, int cost)
	{
		if ( (_score >= cost) && !attachment)
		{
			// Not currently unlocked, and unlock is affordable
			_score -= cost;
			attachment = true;
			return true;
		}
		return false;
	}
}
