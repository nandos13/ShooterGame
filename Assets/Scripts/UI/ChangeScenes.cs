using UnityEngine;
using System.Collections; 
using UnityEngine.SceneManagement; 


//--    This is for swapping scenes, Loading Scene -> Menu Scene -> Game Scene. 


public class ChangeScenes : MBAction
{

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

    public void loadScene(int i)
    {
        string nextSceneName = LevelOrder.levelIndex(i);

        if (nextSceneName != "")
        {
            LevelOrder.SetNextLevel();
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
