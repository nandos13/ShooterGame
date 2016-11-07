using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* DESCRIPTION:
 * Updates the score each frame
 */

public class RefreshLevelScore : MonoBehaviour {

	public Text scoreText;
	public string prefix = "Current Level Damage: $";

	void Start () 
	{
	}

	void Update () 
	{
		if (scoreText)
			scoreText.text = prefix + " " + GameStatistics.LevelScore.ToString();
	}
}
