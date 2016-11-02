using UnityEngine;
using System.Collections;

public class FPC_Debug : MonoBehaviour {

    private ChangeScenes sceneChanger;

	void Start ()
    {
        sceneChanger = gameObject.AddComponent<ChangeScenes>();
	}
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.L))    // skips between scenes
        {
            Debug.Log("pressed l");
            sceneChanger.Execute();
        }
	}
}
