using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownBtnSound : MonoBehaviour
{
    public AudioClip buttonDownSound;

    public string type; //sfx or bgm
    private bool isInitialized = false;
    //드롭 다운 버튼의 사운드를 출력하는 함수
    private void Start()
    {
        // Start 이후소리 나도록
        isInitialized = true;
    }
    public void BtnSoundPlay()
    {
        if (!isInitialized) return;
        GameManager.Instance.SoundMgr.SoundPlay(type.ToString(), "버튼똑딱이는소리",buttonDownSound);
    }
}
