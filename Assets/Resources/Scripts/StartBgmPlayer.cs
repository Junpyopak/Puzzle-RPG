using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBgmPlayer : MonoBehaviour
{
    public AudioClip bgm;
    public string bgmName;
    
    //시작 할때 브금을 실행시키는 플레이어
    private void Start()
    {
        GameManager.Instance.SoundMgr.SoundPlay("bgm",bgmName.ToString(),bgm);
    }
}
