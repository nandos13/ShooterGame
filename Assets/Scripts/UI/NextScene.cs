using UnityEngine;
using System.Collections; 
using UnityEngine.SceneManagement; 

/* DESCRIPTION:
 * Loads the next scene when Execute () is called.
 */

public class NextScene : MBAction {

    public override void Execute()
    {
        // Load next scene
        string nextSceneName = LevelOrder.NextLevel;
        Debug.Log("next scene: " + nextSceneName);

        if (nextSceneName != "")
        {
            LevelOrder.SetNextLevel();
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
