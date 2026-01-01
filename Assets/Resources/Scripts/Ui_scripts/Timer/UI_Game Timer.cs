using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameTimer : MonoBehaviour
{
    private Text Timer_Time;
    private float GameTime = 0f;
    public bool isRunning = true;

    // Start is called before the first frame update
    void Start()
    {
        Timer_Time = GetComponent<Text>();
    }

    void Update()
    {
        if (!isRunning) return;

        // 시간 증가
        GameTime += Time.deltaTime;

        // 시, 분, 초 계산
        int hours = Mathf.FloorToInt(GameTime / 3600);
        int minutes = Mathf.FloorToInt((GameTime % 3600) / 60);
        int seconds = Mathf.FloorToInt(GameTime % 60);

        // UI 업데이트
        if (Timer_Time != null)
            Timer_Time.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
