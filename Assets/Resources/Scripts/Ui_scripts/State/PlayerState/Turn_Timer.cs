using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Turn_Timer : MonoBehaviour
{
    public static Turn_Timer Instance;
    [Header("슬라이더 설정")]
    public Slider turnSlider;   // UI 슬라이더
    public float maxTime = 30f; // 슬라이더 최대값
    public float decreaseSpeed = 1f; // 초당 감소 속도

    public Text EnemyTurnText;
    private int TurnCount = 1;

    private bool isRunning = false;
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
    }
    void Start()
    {
        turnSlider.maxValue = maxTime;
        turnSlider.value = maxTime;
        string roundWord = LocalizationSettings.StringDatabase.GetLocalizedString("Btn_Language", "Round");
        //EnemyTurnText.text = "라운드 : " + (TurnCount);
        EnemyTurnText.text = roundWord + " : " + TurnCount;
        StartTurn();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPaused) return;

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
        //적의턴으로 
        TurnManager.Instance.StartMonsterTurn();
        //우선 예시로 나의턴과 적의턴 돌아간횟수 증가를 표시하기 위함.
        //적의 턴이 끝나면 라운드 수 증가 
        //EnemyTurnText.text = "라운드 : " + (TurnCount);
        // 로컬라이즈된 라운드 텍스트
        string roundWord = LocalizationSettings.StringDatabase.GetLocalizedString("Btn_Language", "Round");
        EnemyTurnText.text = roundWord + " : " + TurnCount;
        TurnCount++;
    }
}
