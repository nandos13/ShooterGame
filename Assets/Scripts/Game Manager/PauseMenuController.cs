using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* DESCRIPTION:
 * Toggles the pause menu
 */

public class PauseMenuController : MBAction {

	public static PauseMenuController instance;				// Only allow a single instance of this script to exist

	[Tooltip("The canvas that will show on pause and hide on resume.")]
	public Canvas menu;										// Pause menu
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

			// Run all OnResume scripts
			foreach (MBAction action in OnResume)
			{
				if (action)
				{
					action.Execute();
				}
			}
		}
		else
		{
			/* Pause game */
			Options.Paused = true;
			if (menu)
				menu.enabled = true;
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
	}
}
