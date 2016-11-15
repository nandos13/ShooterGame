using UnityEngine;
using System.Collections;

[System.Serializable]
public class OptionsSerializer {

    public bool fullscreen;
    public int resolutionIndex;
    public int textureQuality;
    public int antialiasing;
    public int vSync;

    public float musicVolume;

    public OptionsSerializer()
    {
        // Constructor
    }

    public void Update()
    {
        // Update all values to be current options values
        fullscreen = Options.fullscreen;
        resolutionIndex = Options.resolutionIndex;
        textureQuality = Options.textureQuality;
        antialiasing = Options.antialiasing;
        vSync = Options.vSync;

        musicVolume = Options.musicVolume;
    }
}
