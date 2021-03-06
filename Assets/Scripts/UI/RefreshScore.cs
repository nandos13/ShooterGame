﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* DESCRIPTION:
 * Updates the score each frame
 */

public class RefreshScore : MonoBehaviour {

	public Text scoreText;
	public string prefix = "Credits: $";

	void Start () 
	{
	}

	void Update () 
	{
		if (scoreText)
		{
			int total = GameStatistics.Score + GameStatistics.LevelScore;
			scoreText.text = prefix + " " + total.ToString();
		}
	}
}
