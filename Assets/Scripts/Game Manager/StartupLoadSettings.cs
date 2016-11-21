using UnityEngine;
using System.Collections;
using System.IO;

/* DESCRIPTION:
 * This script should be run when the application starts. (Eg. place on the Main Menu)
 * This will 
 */

public class StartupLoadSettings : MonoBehaviour {

	void Start () 
	{
		// First, check for a custom graphics file.
		if (System.IO.File.Exists (Application.persistentDataPath + "/gamesettings.json"))
		{
			// A file exists, load the data from said file
			OptionsSerializer ser = new OptionsSerializer();
			string jsonData = File.ReadAllText(Application.persistentDataPath + "/gamesettings.json");
			ser = JsonUtility.FromJson<OptionsSerializer>(jsonData);

			Options.deserialize(ser);
		}
		else
		{
			// No file exists for user-defined graphics. Create one using the unity quality settings
			OptionsSerializer ser = new OptionsSerializer();
			ser.fullscreen = Screen.fullScreen;
			ser.resolutionIndex = 0;
			ser.textureQuality = QualitySettings.masterTextureLimit;
			switch (QualitySettings.antiAliasing)
			{
			case 0:
				ser.antialiasing = 0;
				break;
			case 2:
				ser.antialiasing = 1;
				break;
			case 4:
				ser.antialiasing = 2;
				break;
			case 8:
				ser.antialiasing = 3;
				break;
			case 16:
				ser.antialiasing = 4;
				break;
			default:
				ser.antialiasing = 0;
				break;
			}
			ser.vSync = QualitySettings.vSyncCount;
			ser.musicVolume = 0.5f;

			string jsonData = JsonUtility.ToJson(ser, true);
			File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
		}
	}
}
