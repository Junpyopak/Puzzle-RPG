using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOption : MonoBehaviour
{
    public Dropdown ScreenDropdown;
    List<Resolution> resolutions = new List<Resolution>();
    public int ScreenNum;
    // Start is called before the first frame update
    void Start()
    {
        InitUi();

        // 드롭다운 이벤트 연결 
        ScreenDropdown.onValueChanged.AddListener(DropdownChange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void InitUi()
    {
        resolutions.Clear();
        resolutions.AddRange(Screen.resolutions);

        ScreenDropdown.options.Clear();

        Resolution current = Screen.currentResolution;
        int optionNum = 0;

        foreach (Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = $"{item.width}x{item.height} {item.refreshRateRatio.value:F0}Hz";
            ScreenDropdown.options.Add(option);
            Debug.Log(option.text);

            //현재 해상도와 일치하면 선택
            if (item.width == current.width &&
                item.height == current.height)
            {
                ScreenDropdown.value = optionNum;
            }
            optionNum++;
        }

        ScreenDropdown.RefreshShownValue();
    }
    public void DropOptionChange(int x)
    {
        ScreenNum =x;
    }
    public void DropdownChange(int index)
    {
        Resolution r = resolutions[index];

        Screen.SetResolution(
            r.width,
            r.height,
            Screen.fullScreenMode,
            r.refreshRateRatio
        );

        Debug.Log($"해상도 변경: {r.width}x{r.height} {r.refreshRateRatio.value:F0}Hz");
    }
}
