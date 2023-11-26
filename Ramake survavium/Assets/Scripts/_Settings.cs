using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class _Settings : MonoBehaviour
{
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private GameObject menu;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider renderScaleSlider;
    [SerializeField] private Toggle effectVolume;
    [SerializeField] private Toggle showTutorial;
    [SerializeField] private UniversalRenderPipelineAsset Low;
    [SerializeField] private UniversalRenderPipelineAsset Medium;
    [SerializeField] private UniversalRenderPipelineAsset High;
    private float currentMasterVolume;
    private float currentMusicVolume;
    private float currentRenderScale;
    private Resolution[] resolutions;
    public void SettingsOn()
    {
        menu.SetActive(false);
        panelSettings.SetActive(true);
    }
    public void SettingsOff()
    {
        panelSettings.SetActive(false);
        menu.SetActive(true);
    }

    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        switch (qualityIndex)
        {
            case 0:
                renderScaleSlider.value = High.renderScale; break;
            case 1:
                renderScaleSlider.value = Medium.renderScale; break;
            case 2:
                renderScaleSlider.value = Low.renderScale; break;
        }
    }
    public void SetMasterVolume(float volume)
    {
        //float value = Mathf.Lerp(-80, 0, volume);
        audioMixer.SetFloat("MasterVolume", volume);
        currentMasterVolume = volume;
    } 
    public void SetMusicVolume(float volume)
    {
        //float value = Mathf.Lerp(-80, 0, volume);
        audioMixer.SetFloat("MusicVolume", volume);
        currentMusicVolume = volume;
    }
    public void SetEffects(bool Effects) 
    {
        if (Effects)
        {
            audioMixer.SetFloat("EffectsVolume", 0f);
        }
        else
        {
            audioMixer.SetFloat("EffectsVolume", -55f);
        }
    }
    public void SetRenderScale(float scale)
    {
        currentRenderScale = scale;
        switch (qualityDropdown.value)
        {
            case 0:
                High.renderScale = scale; break;
            case 1:
                Medium.renderScale = scale; break;
            case 2:
                Low.renderScale = scale; break;
        }
    }


    public void SaveSettings()
    {
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
        PlayerPrefs.SetFloat("VolumeMasterPreference", currentMasterVolume);
        PlayerPrefs.SetFloat("VolumeMusicPreference", currentMusicVolume);
        PlayerPrefs.SetInt("VolumeEffects", System.Convert.ToInt32(effectVolume.isOn));
        PlayerPrefs.SetInt("ShowTutorial", System.Convert.ToInt32(showTutorial.isOn));
        PlayerPrefs.SetFloat("RenderScale", currentRenderScale);
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        //Quality
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        else
            qualityDropdown.value = 0;

        //Resolution
        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;

        //Fullscreen
        if (PlayerPrefs.HasKey("FullscreenPreference"))
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        else
            Screen.fullScreen = true;

        //MasterVolume
        if (PlayerPrefs.HasKey("VolumeMasterPreference"))
            masterVolumeSlider.value = PlayerPrefs.GetFloat("VolumeMasterPreference");
        else
            masterVolumeSlider.value = 0;

        //MusicVolume
        if (PlayerPrefs.HasKey("VolumeMusicPreference"))
            musicVolumeSlider.value = PlayerPrefs.GetFloat("VolumeMusicPreference");
        else
            musicVolumeSlider.value = 0;

        //EffectsVolume
        if (PlayerPrefs.HasKey("VolumeEffects"))
            effectVolume.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("VolumeEffects"));
        else
            effectVolume.isOn = true;

        //ShowTutorial
        if (PlayerPrefs.HasKey("ShowTutorial"))
            showTutorial.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("ShowTutorial"));
        else
            showTutorial.isOn = true;

        //RenderScale
        if (PlayerPrefs.HasKey("RenderScale"))
        {
            renderScaleSlider.value = PlayerPrefs.GetFloat("RenderScale");
            SetRenderScale(renderScaleSlider.value);
        }
        else
        {
            renderScaleSlider.value = 1;
            SetRenderScale(renderScaleSlider.value);
        }
    }
}
