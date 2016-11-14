using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

/* DESCRIPTION:
 * Attempts to load a scene based on index. This uses the static List<string> in the LevelOrder class.
 */

public class LoadScene : MBAction {

	public int index;

	public override void Execute ()
	{
		string nextSceneName = LevelOrder.levelIndex(index);

		if (nextSceneName != "")
		{
			LevelOrder.SetNextLevel();
			SceneManager.LoadScene(nextSceneName);
		}
	}
}
