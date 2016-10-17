using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/* DESCRIPTION:
 * Updates the score each frame
 */

public class RefreshScore : MonoBehaviour {

	public Text scoreText;

	void Start () 
	{
	}

	void Update () 
	{
		scoreText.text = "Score: " + GameStatistics.Score.ToString();
	}
}
