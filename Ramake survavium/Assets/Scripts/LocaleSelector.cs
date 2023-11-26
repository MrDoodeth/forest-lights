using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering.Universal;

public class LocaleSelector : MonoBehaviour
{
    private bool isActive = false;
    [SerializeField] private GameObject en;
    [SerializeField] private GameObject rus;
    [SerializeField] private TMP_Dropdown quality;
    private void Start()
    {
        int ID = PlayerPrefs.GetInt("LocaleKey", 0);
        ChangeLocale(ID);

        if (ID == 0)
        {
            en.SetActive(true);
            rus.SetActive(false);
        }
        else
        {
            en.SetActive(false);
            rus.SetActive(true);
        }
    }
    public void ChangeLocale(int localeID)
    {
        if (isActive)
            return;
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale (int _localeID)
    {
        isActive = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        PlayerPrefs.SetInt("LocaleKey", _localeID);

        int Id = PlayerPrefs.GetInt("LocaleKey", 0);
        if (Id == 0)
        {
            quality.options[0].text = "HIGH";
            quality.options[1].text = "MEDIUM";
            quality.options[2].text = "LOW";
        }
        else
        {
            quality.options[0].text = "Высокие";
            quality.options[1].text = "Средние";
            quality.options[2].text = "Низкие";
        }

        isActive = false;
    }
}
