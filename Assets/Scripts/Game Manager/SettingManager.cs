using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class SettingManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown textureDropdown;
    public Dropdown antialiasingDropdown;
    public Dropdown vSyncDropdown;
    public Slider audioSlider;
    public Slider musicSlider;
    public Button applyButton;
    
    public AudioSource musicSource;
    public Resolution[] resolutions;        // my resolution array of resolutions

    void OnEnable()
    {
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { VSyncChange(); });
        
        musicSlider.onValueChanged.AddListener(delegate { MusicVolumeChange(); });
        applyButton.onClick.AddListener(delegate { OnApplyButton(); });
        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)       // every resolution in the resolution list
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));     // adds image of resolution types
        }
        LoadSettings();
    }

    public void OnFullscreenToggle()
    {
       Options.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        Options.resolutionIndex = resolutionDropdown.value;
    }

    public void OnTextureQualityChange()
    {
        QualitySettings.masterTextureLimit = Options.textureQuality = textureDropdown.value;
    }

   public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = (int)Mathf.Pow(2, antialiasingDropdown.value);
        Options.antialiasing = antialiasingDropdown.value;
    }

    public void VSyncChange()
    {
        QualitySettings.vSyncCount = Options.vSync = vSyncDropdown.value;
    }

    

    public void MusicVolumeChange()
    {
        musicSource.volume = Options.musicVolume = musicSlider.value;
    }

    public void OnApplyButton()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        // Create a new serializer version of Options and update so all variables are current.
        OptionsSerializer serializer = new OptionsSerializer();
        serializer.Update();

        string jsonData = JsonUtility.ToJson(serializer, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
    }

    public void LoadSettings()
    {
        // Load data
        OptionsSerializer serializer = new OptionsSerializer();
        File.ReadAllText(Application.persistentDataPath + "/gamesettings.json");
        serializer = JsonUtility.FromJson<OptionsSerializer>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

        // Transfer data into static Options class
        Options.deserialize(serializer);

        // Update values
        musicSlider.value = Options.musicVolume;
        
        vSyncDropdown.value = Options.vSync;
        antialiasingDropdown.value = Options.antialiasing;
        textureDropdown.value = Options.textureQuality;
        resolutionDropdown.value = Options.resolutionIndex;
        fullscreenToggle.isOn = Options.fullscreen;
        Screen.fullScreen = Options.fullscreen;  
        resolutionDropdown.RefreshShownValue();
    }
}
