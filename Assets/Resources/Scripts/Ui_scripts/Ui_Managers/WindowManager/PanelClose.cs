using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelClose : MonoBehaviour
{
    public GameObject panel;
    public Player player;
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
    //public void Exit()
    //{
    //    if (Scenemgr.Instance != null)
    //    {
    //        Scenemgr.Instance.ChangeScene(eSCENE.TITLE);
    //    }
    //    Debug.Log("게임을 끝내겠습니다");
    //    panel.SetActive(!panel.activeSelf);
    //}
    public void Exit()
    {
        int slot = SaveContext.Instance.currentSlot;

        // 슬롯이 선택된 상태라면 저장
        if (slot >= 0)
        {
            SaveData data = new SaveData
            {
                //level = player.level,
                playerExp = player.Exp,
                playerHp = player.Hp,
                playerPosition = player.transform.position,
                saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm")
            };

            SaveManager.Save(slot, data);
            Debug.Log($"슬롯 {slot} 저장 완료");
        }
        else
        {
            Debug.LogWarning("선택된 슬롯이 없어 저장하지 않았습니다");
        }

        // 타이틀로 이동
        if (Scenemgr.Instance != null)
        {
            Scenemgr.Instance.ChangeScene(eSCENE.TITLE);
        }

        panel.SetActive(false);
    }
}
    
