using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public GameObject OptionInfo;

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
    }

    public void CloseOption()
    {
        OptionInfo.SetActive(false);
    }

    public void OptionSave()
    {
        CloseOption();
    }
}
