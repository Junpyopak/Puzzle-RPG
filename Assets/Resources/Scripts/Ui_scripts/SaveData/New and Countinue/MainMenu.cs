using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    public GameObject ExitGameNotice;
    public GameObject CardCollection;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ÇÊ¿ä ½Ã
        }
        else
        {
            Destroy(gameObject);
        }
    }
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
