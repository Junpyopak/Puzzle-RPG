using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenMode : MonoBehaviour
{
    public Dropdown ScreenModeDrop;
    // Start is called before the first frame update
    void Start()
    {
        InitScreenMode();
        ScreenModeDrop.onValueChanged.AddListener(OnDisplayModeChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void InitScreenMode()
    {
        ScreenModeDrop.options.Clear();
        ScreenModeDrop.options.Add(new Dropdown.OptionData("창 모드"));
        ScreenModeDrop.options.Add(new Dropdown.OptionData("전체 화면"));
        ScreenModeDrop.options.Add(new Dropdown.OptionData("보더 리스"));

        ScreenModeDrop.value = GetCurrentModeIndex();
        ScreenModeDrop.RefreshShownValue();
    }
    int GetCurrentModeIndex()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.Windowed:
                return 0;
            case FullScreenMode.ExclusiveFullScreen:
                return 1;
            case FullScreenMode.FullScreenWindow:
                return 2;
            default:
                return 0;
        }
    }

    void OnDisplayModeChanged(int index)
    {
        switch (index)
        {
            case 0: // 창모드
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

            case 1: // 전체화면
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;

            case 2: // 보더리스
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }

        Debug.Log($"화면 모드 변경: {Screen.fullScreenMode}");
    }
}
