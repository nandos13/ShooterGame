using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Toggles the pause menu
 
    //-- Drag PausePanel to PauseMenuController(script).
         In ResumeButton add a OnClick() event. Drag GameManager and set to UnPause()
         Add Lock Curse Script -> drag to OnResume Element.

 */

public class PauseMenuController : MBAction {

	public static PauseMenuController instance;				// Only allow a single instance of this script to exist
	[Tooltip("The canvas that will show on pause and hide on resume.")]
	public GameObject menu;									// Pause menu
	public List<MBAction> OnPause = new List<MBAction>();
	public List<MBAction> OnResume = new List<MBAction>();

	void Awake ()
	{
		if (!instance)
			instance = this;
		else
		{
			Destroy(this);
			Debug.LogWarning("More than one instance of class PauseMenuController exists. Deleting duplicate instance.");
		}
	}

	void Update () 
	{
        if (Input.GetKeyDown("escape"))
            Execute();
    }

	public override void Execute ()
	{
		/* Toggle the pause menu */

		// Toggle menu
		if (Options.Paused)
		{
            Unpause();
		}
		else
		{
            Pause();
		}
	}

    public void Pause ()
    {
        /* Pause game */
        Options.Paused = true;
        if (menu)
            menu.SetActive(true);
        Debug.Log("Pausing");

        // Run all OnPause scripts
        foreach (MBAction action in OnPause)
        {
            if (action)
            {
                action.Execute();
            }
        }
    }

    public void Unpause ()
    {
        /* Unpause game */
        Options.Paused = false;
        if (menu)
            menu.SetActive(false);

        Debug.Log("Playing");

        // Run all OnResume scripts
        foreach (MBAction action in OnResume)
        {
            if (action)
            {
                action.Execute();
            }
        }
    }
}
