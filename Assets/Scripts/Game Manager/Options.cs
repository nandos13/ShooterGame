using UnityEngine;
using System.Collections;

/* DESCRIPTION:
 * Static Options class to hold game options (Audio, Visual, etc)
 */

[System.Serializable]
public static class Options 
{
    // Game Settings var
    public static bool fullscreen;
    public static int resolutionIndex;
    public static int textureQuality;
    public static int antialiasing;
    public static int vSync;
    public static float audioVolume;
    public static float musicVolume;



    // Pausing
    private static bool _paused = false;
	public static bool Paused
	{
		get { return _paused; }
		set
		{
			_paused = value;
			if (value)
				Time.timeScale = 0;
			else
				Time.timeScale = 1;
		}
	}

    public static void deserialize(OptionsSerializer ser)
    {
        fullscreen = ser.fullscreen;
        resolutionIndex = ser.resolutionIndex;
        textureQuality = ser.textureQuality;
        antialiasing = ser.antialiasing;
        vSync = ser.vSync;
        audioVolume = ser.audioVolume;
        musicVolume = ser.musicVolume;
    }
}
