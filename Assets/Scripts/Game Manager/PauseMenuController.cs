using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Toggles the pause menu
 */

public class PauseMenuController : MBAction {

	public static PauseMenuController instance;				// Only allow a single instance of this script to exist

	public Canvas menu;										// Pause menu

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
			Execute ();
	}

	public override void Execute ()
	{
		/* Toggle the pause menu */

		// Toggle menu
		if (Options.Paused)
		{
			/* Unpause game */
			Options.Paused = false;
			if (menu)
				menu.enabled = false;
			Debug.Log("Playing");
		}
		else
		{
			/* Pause game */
			Options.Paused = true;
			if (menu)
				menu.enabled = true;
			Debug.Log("Pausing");
		}
	}
}
