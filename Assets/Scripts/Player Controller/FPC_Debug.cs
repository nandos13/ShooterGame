using UnityEngine;
using System.Collections;

public class FPC_Debug : MonoBehaviour {

    private NextScene sceneChanger;

	void Start ()
    {
		sceneChanger = gameObject.AddComponent<NextScene>();
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
