using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelClose : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 현재 상태 반전
            panel.SetActive(!panel.activeSelf);
        }
    }
    public void Close()
    {
        panel.SetActive(!panel.activeSelf);
    }
    public void Exit()
    {
        if (Scenemgr.Instance != null)
        {
            Scenemgr.Instance.ChangeScene(eSCENE.TITLE);
        }
        Debug.Log("게임을 끝내겠습니다");
        panel.SetActive(!panel.activeSelf);
    }
}
    
