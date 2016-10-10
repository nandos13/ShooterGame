using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Static Options class to hold game options (Audio, Visual, etc)
 */

public static class Options 
{
	private static bool paused = false;
	public static bool Paused
	{
		get { return paused; }
		set
		{
			paused = value;
			if (value)
				Time.timeScale = 0;
			else
				Time.timeScale = 1;
		}
	}
}
