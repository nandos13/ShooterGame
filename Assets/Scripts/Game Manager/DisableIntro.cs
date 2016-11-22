using UnityEngine;
using System.Collections;

public class DisableIntro : MBAction {

	public GameObject intro;

	void Start ()
	{
		if (intro)
		{
			if (GameStatistics.introPlayed)
				intro.SetActive(false);
		}
	}

	public override void Execute ()
	{
		GameStatistics.introPlayed = true;
	}
}
