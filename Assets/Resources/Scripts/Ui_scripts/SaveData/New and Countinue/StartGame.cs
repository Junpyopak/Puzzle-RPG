using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    public GameObject OptionInfo;
    public GameObject DarkWindow;

    private void Start()
    {
        OptionInfo.SetActive(false);
    }
    public void OnClickNewGame()
    {
        SlotSelectContext.Instance.mode = SlotSelectMode.NewGame;
        Scenemgr.Instance.ChangeScene(eSCENE.SlotSelectScene);
    }

    public void OnClickContinue()
    {
        SlotSelectContext.Instance.mode = SlotSelectMode.Continue;
        Scenemgr.Instance.ChangeScene(eSCENE.SlotSelectScene);
    }

    public void OpenOption()
    {
        OptionInfo.SetActive(true);
        DarkWindow.SetActive(true);
        GameManager.Instance.UIMgr.isOpenPauseMenu = true;
    }

    public void CloseOption()
    {
        OptionInfo.SetActive(false);
        DarkWindow.SetActive(false);
        GameManager.Instance.UIMgr.isOpenPauseMenu = false;
    }

    public void OptionSave()
    {
        CloseOption();
    }
}
