using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameTimer : MonoBehaviour
{
    public static UI_GameTimer Instance;
    private Text Timer_Time;
    public float GameTime = 0f;
    public bool isRunning = true;
    public bool isPaused = false;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        Timer_Time = GetComponent<Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Timer_Time = GetComponent<Text>();
    }

    void Update()
    {
        if (isPaused) return;
        if (!isRunning) return;
        
        GameTime += Time.deltaTime;
        UpdateTimerUI();

    }
    public void LoadFromSave(float savedTime)
    {
        GameTime = savedTime;
        UpdateTimerUI();
    }
    private void UpdateTimerUI()
    {
        int h = Mathf.FloorToInt(GameTime / 3600);
        int m = Mathf.FloorToInt((GameTime % 3600) / 60);
        int s = Mathf.FloorToInt(GameTime % 60);

        if (Timer_Time != null)
            Timer_Time.text = string.Format("{0:00}:{1:00}:{2:00}", h, m, s);
    }
    public void SetTime(float time)
    {
        GameTime = time;
        UpdateTimerUI(); // UI 쓰면 필수
    }
}
