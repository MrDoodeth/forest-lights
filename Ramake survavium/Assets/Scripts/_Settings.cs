using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Platforms;

public class _Settings : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour _player;

    [SerializeField] private GameObject panelSettings;
    [SerializeField] private GameObject menu;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider renderScaleSlider;
    [SerializeField] private Toggle effectVolume;
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

    private void Awake()
    {
        _player = GameObject.Find("Man")?.GetComponent<PlayerBehaviour>();
        SetMasterVolume(PlayerPrefs.GetFloat("VolumeMasterPreference"));
        SetMusicVolume(PlayerPrefs.GetFloat("VolumeMusicPreference"));
        LoadSettings();
    }

    private void OnEnable()
    {
        LoadSettings();
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
        PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
        PlayerPrefs.SetFloat("VolumeMasterPreference", currentMasterVolume);
        PlayerPrefs.SetFloat("VolumeMusicPreference", currentMusicVolume);
        PlayerPrefs.SetInt("VolumeEffects", System.Convert.ToInt32(effectVolume.isOn));
        PlayerPrefs.SetFloat("RenderScale", currentRenderScale);
    }

    public void LoadSettings()
    {
        //Quality
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        }
        else
            qualityDropdown.value = 0;
        SetQuality(qualityDropdown.value);

        //MasterVolume
        if (PlayerPrefs.HasKey("VolumeMasterPreference"))
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("VolumeMasterPreference");
        }
        else
            masterVolumeSlider.value = 0;
        SetMasterVolume(masterVolumeSlider.value);

        //MusicVolume
        if (PlayerPrefs.HasKey("VolumeMusicPreference"))
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("VolumeMusicPreference");
        }
        else
            musicVolumeSlider.value = 0;
        SetMusicVolume(musicVolumeSlider.value);

        //EffectsVolume
        if (PlayerPrefs.HasKey("VolumeEffects"))
        {
            effectVolume.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("VolumeEffects"));
        }
        else
            effectVolume.isOn = true;
        SetEffects(effectVolume.isOn);


        //RenderScale
        if (PlayerPrefs.HasKey("RenderScale"))
        {
            renderScaleSlider.value = PlayerPrefs.GetFloat("RenderScale");
        }
        else
        {
            renderScaleSlider.value = 1;
        }
        SetRenderScale(renderScaleSlider.value);
    }
}
