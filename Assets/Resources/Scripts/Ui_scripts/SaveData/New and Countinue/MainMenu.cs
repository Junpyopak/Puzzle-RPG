using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject ExitGameNotice;
    public GameObject CardCollection;

    private void Start()
    {
        ExitGameNotice.SetActive(false);
    }
    public void OnClickClosePanel()
    {
        ExitGameNotice.SetActive(false);
    }
    public void OnClickOpenPanel()
    {
        ExitGameNotice.SetActive(true);
    }
    public void GoTitle()
    {
        Scenemgr.Instance.ChangeScene(eSCENE.TITLE);
    }
    public void OnClickNewGame()
    {
        Scenemgr.Instance.ChangeScene(eSCENE.GameScene);

    }
    public void OnClickOpenCollect()
    {
        CardCollection.SetActive(true);
    }
    public void OnClickCloseCollect()
    {
        CardCollection.SetActive(false);
    }

}
