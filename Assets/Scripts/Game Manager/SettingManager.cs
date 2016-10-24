using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class SettingManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public Dropdown resolutionDropdown;
    public Dropdown textureDropdown;
    public Dropdown antialiasingDropdown;
    public Dropdown vSyncDropdown;
    public Slider audioSlider;
    public Slider musicSlider;

    public AudioSource audioSource;
    public AudioSource musicSource;
    public Resolution[] resolutions;
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
        resolutions = Screen.resolutions;
    }

    public void OnFullscreenToggle()
    {
       gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {

    }

    public void OnTextureQualityChange()
    {
        QualitySettings.masterTextureLimit = gameSettings.textureQuality = textureDropdown.value;
    }

   public void OnAntialiasingChange()
    {
        QualitySettings.antiAliasing = gameSettings.antialiasing = (int)Mathf.Pow(2, antialiasingDropdown.value);
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

    public void SaveSettings()
    {

    }

    public void LoadSettings()
    {

    }


}
