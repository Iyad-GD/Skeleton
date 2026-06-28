using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    [Header("Volume Controls")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Graphics Controls")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private List<Resolution> uniqueResolutions = new List<Resolution>();

    private void Start()
    {
        InitializeVolumeSliders();
        InitializeQualityDropdown();
        InitializeResolutionDropdown();
        InitializeFullscreenToggle();
    }

    private void InitializeVolumeSliders()
    {
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (masterSlider != null)
        {
            masterSlider.value = masterVol;
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        if (musicSlider != null)
        {
            musicSlider.value = musicVol;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVol;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Apply immediately to Managers
        ApplyVolumes(masterVol, musicVol, sfxVol);
    }

    private void ApplyVolumes(float master, float music, float sfx)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetMasterVolume(master);
            MusicManager.Instance.SetMusicVolume(music);
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMasterVolume(master);
            SoundManager.Instance.SetSFXVolume(sfx);
        }
    }

    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();

        if (MusicManager.Instance != null) MusicManager.Instance.SetMasterVolume(value);
        if (SoundManager.Instance != null) SoundManager.Instance.SetMasterVolume(value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();

        if (MusicManager.Instance != null) MusicManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();

        if (SoundManager.Instance != null) SoundManager.Instance.SetSFXVolume(value);
    }

    private void InitializeQualityDropdown()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();
        List<string> options = new List<string>(QualitySettings.names);
        qualityDropdown.AddOptions(options);

        int savedQuality = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        qualityDropdown.value = savedQuality;
        qualityDropdown.onValueChanged.AddListener(SetQuality);

        QualitySettings.SetQualityLevel(savedQuality);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("QualityLevel", index);
        PlayerPrefs.Save();
    }

    private void InitializeResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        resolutionDropdown.ClearOptions();

        Resolution[] allResolutions = Screen.resolutions;
        List<string> options = new List<string>();
        uniqueResolutions.Clear();

        int currentResolutionIndex = 0;
        int savedWidth = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);

        // Filter out duplicate width/height combinations (e.g. from different refresh rates)
        HashSet<string> seenResolutions = new HashSet<string>();

        for (int i = 0; i < allResolutions.Length; i++)
        {
            string key = allResolutions[i].width + "x" + allResolutions[i].height;
            if (!seenResolutions.Contains(key))
            {
                seenResolutions.Add(key);
                uniqueResolutions.Add(allResolutions[i]);

                string optionText = allResolutions[i].width + " x " + allResolutions[i].height;
                options.Add(optionText);

                if (allResolutions[i].width == savedWidth && allResolutions[i].height == savedHeight)
                {
                    currentResolutionIndex = uniqueResolutions.Count - 1;
                }
            }
        }

        // Fallback in case screen resolution wasn't explicitly found
        if (uniqueResolutions.Count == 0)
        {
            uniqueResolutions.Add(Screen.currentResolution);
            options.Add(Screen.currentResolution.width + " x " + Screen.currentResolution.height);
            currentResolutionIndex = 0;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        // Apply saved resolution on start
        Resolution targetRes = uniqueResolutions[currentResolutionIndex];
        Screen.SetResolution(targetRes.width, targetRes.height, Screen.fullScreen);
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= uniqueResolutions.Count) return;

        Resolution resolution = uniqueResolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionWidth", resolution.width);
        PlayerPrefs.SetInt("ResolutionHeight", resolution.height);
        PlayerPrefs.Save();
    }

    private void InitializeFullscreenToggle()
    {
        if (fullscreenToggle == null) return;

        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.isOn = savedFullscreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        Screen.fullScreen = savedFullscreen;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}