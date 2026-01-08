using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGame : MonoBehaviour
{
    public GameObject NewGameNotice;
    public GameObject TutoNotice;

    private void Start()
    {
        NewGameNotice.SetActive(false);
        TutoNotice.SetActive(false);
    }
    public void OnClickClosePanel()
    {
        NewGameNotice.SetActive(false);
    }
    public void OnClickOpenPanel()
    {
        NewGameNotice.SetActive(true);
    }
    public void OnClickTutorial()
    {
        SlotSelectContext.Instance.mode = SlotSelectMode.Continue;
        Scenemgr.Instance.ChangeScene(eSCENE.Tutorial);
    }
    public void OnClickNewGame()
    {
        SlotSelectContext.Instance.mode = SlotSelectMode.NewGame;
        Scenemgr.Instance.ChangeScene(eSCENE.SlotSelectScene);
    }
    public void OnClickCloseTutoinfo()
    {
        TutoNotice.SetActive(false);
    }
    public void OnClickOpenTutoinfo()
    {
        TutoNotice.SetActive(true);
    }
}
