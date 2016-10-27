using UnityEngine;
using System.Collections; 
using UnityEngine.SceneManagement; 


//--    This is for swapping scenes, Loading Scene -> Menu Scene -> Game Scene. 


public class ChangeScenes : MonoBehaviour
{
	public void ChangeToScene (string sceneToChangeTo)
    {
        
        SceneManager.LoadScene(sceneToChangeTo);
    }
}
