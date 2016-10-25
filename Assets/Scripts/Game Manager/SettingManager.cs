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
    public AudioSource audioSource;
    public AudioSource musicSource;
    public Resolution[] resolutions;        // my resolution array of resolutions
    [HideInInspector]
    public GameSettings gameSettings;

    void OnEnable()
    {
        gameSettings = new GameSettings();
        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        textureDropdown.onValueChanged.AddListener(delegate { OnTextureQualityChange(); });
        antialiasingDropdown.onValueChanged.AddListener(delegate { OnAntialiasingChange(); });
        vSyncDropdown.onValueChanged.AddListener(delegate { VSyncChange(); });
        audioSlider.onValueChanged.AddListener(delegate { AudioVolumeChange(); });
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
       gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
        gameSettings.resolutionIndex = resolutionDropdown.value;
    }

    public void OnTextureQualityChange()
    {
        QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureDropdown.value;
    }

   public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = (int)Mathf.Pow(2, antialiasingDropdown.value);
        gameSettings.antialiasing = antialiasingDropdown.value;
    }

    public void VSyncChange()
    {
        QualitySettings.vSyncCount = gameSettings.vSync = vSyncDropdown.value;
    }

    public void AudioVolumeChange()
    {
        audioSource.volume = gameSettings.audioVolume = audioSlider.value;
    }

    public void MusicVolumeChange()
    {
        musicSource.volume = gameSettings.musicVolume = musicSlider.value;
    }

    public void OnApplyButton()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
    }

    public void LoadSettings()
    {
        File.ReadAllText(Application.persistentDataPath + "/gamesettings.json");
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));    
        musicSlider.value = gameSettings.musicVolume;
        audioSlider.value = gameSettings.audioVolume;
        vSyncDropdown.value = gameSettings.vSync;
        antialiasingDropdown.value = gameSettings.antialiasing;
        textureDropdown.value = gameSettings.textureQuality;
        resolutionDropdown.value = gameSettings.resolutionIndex;
        fullscreenToggle.isOn = gameSettings.fullscreen;
        Screen.fullScreen = gameSettings.fullscreen;  
        resolutionDropdown.RefreshShownValue();
    }
}
