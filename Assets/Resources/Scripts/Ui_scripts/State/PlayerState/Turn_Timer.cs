using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turn_Timer : MonoBehaviour
{

    [Header("슬라이더 설정")]
    public Slider turnSlider;   // UI 슬라이더
    public float maxTime = 30f; // 슬라이더 최대값
    public float decreaseSpeed = 1f; // 초당 감소 속도

    private bool isRunning = false;

    void Start()
    {
        turnSlider.maxValue = maxTime;
        turnSlider.value = maxTime;

        StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            // 슬라이더 감소
            turnSlider.value -= decreaseSpeed * Time.deltaTime;

            // 0 이하로 내려가면 턴 종료
            if (turnSlider.value <= 0f)
            {
                turnSlider.value = 0f;
                EndTurn();
            }
        }
    }
    public void StartTurn()
    {
        isRunning = true;
        turnSlider.value = maxTime;
    }

    public void EndTurn()
    {
        isRunning = false;
        Debug.Log("턴 종료!");
        turnSlider.maxValue = maxTime;
        turnSlider.value = maxTime;
        // 여기에 턴 종료 후 처리할 코드 추가
    }
}
