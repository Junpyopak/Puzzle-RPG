using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageSwithcher : MonoBehaviour
{
    public void ChangeLanguage(string localCode)
    {
        StartCoroutine(SetLocalse(localCode));
    }

    IEnumerator SetLocalse(string localCode)
    {
        yield return LocalizationSettings.InitializationOperation;
        var selectedLocale = LocalizationSettings.AvailableLocales.GetLocale(localCode);
        if (selectedLocale != null)
        {
            LocalizationSettings.SelectedLocale = selectedLocale;
        }
        else
        {
            Debug.LogWarning("Locale not found: " + localCode);
        }
    }
}

