using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class LanguageFlagDropdown : MonoBehaviour
{
    public TMP_Dropdown LanguageDropdown;

    public List<Sprite> localeFlags;

    private Image flagImage;
    private void Start()
    {


        flagImage = LanguageDropdown.transform.Find("Label/FlagImage").GetComponent<Image>();
        // 유니티가 기본으로 제공하는 드롭다운 요소들을 제거합니다.
       // LanguageDropdown.ClearOptions();

        // 드롭다운에 언어를 추가합니다.
        //LanguageDropdown.AddOptions(LocalizationSettings.AvailableLocales.Locales.ConvertAll(locale => locale.LocaleName));


        // 드롭 다운에 선택된 언어를 설정합니다.
        LanguageDropdown.value = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);

        // 드롭다운의 값이 변경되면 로컬라이징을 변경합니다.
        LanguageDropdown.onValueChanged.AddListener(ChangeLocale);

        // 드롭다운 현재 선택
        int currentIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        LanguageDropdown.value = currentIndex;
        UpdateFlag(currentIndex);


    }

    /// <summary>
    /// 로컬라이징을 변경합니다.
    /// </summary>
    /// <param name="index">Edit->Project Settings->Localization에 설정한 언어 순서의 Index입니다.</param>
    public void ChangeLocale(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        Debug.Log($"ChangeLocale 호출! index = {index}");

        UpdateFlag(index);
    }
    private void UpdateFlag(int index)
    {
        if (flagImage != null && index >= 0 && index < localeFlags.Count)
        {
            flagImage.sprite = localeFlags[index];
        }
    }
}


