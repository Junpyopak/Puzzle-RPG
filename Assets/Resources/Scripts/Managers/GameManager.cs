using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // 묶어줄 매니저들
    //public TurnManager TurnMgr;
    public CardManager CardMgr;
    public UIManager UIMgr;
    public SoundManager SoundMgr;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //InitManagers(); // 매니저 초기화
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //private void InitManagers()
    //{
    //    // 컴포넌트 방식으로 가져오기
    //    SoundMgr = GetComponentInChildren<SoundManager>();
    //    UIMgr = GetComponentInChildren<UIManager>();
    //    LevelMgr = GetComponentInChildren<LevelManager>();
    //    CardMgr = GetComponentInChildren<CardManager>();
    //    TurnMgr = GetComponentInChildren<TurnManager>();
    //}
}
