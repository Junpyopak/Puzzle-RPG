using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Image mainUI;
    public Text bossText;
    public Image noticeImage;
    public Image noticeImage2;

    [Header("Effect Settings")]
    public float flashSpeed = 3f;     // 깜빡임 속도
    public float stayTime = 2f;       // 화면에 머무르는 시간

    private bool isShowing = false;
    private float timer = 0f;
    private float localTime = 0f;
    // UI 켜기
    public void ShowPopup()
    {
        gameObject.SetActive(true);

        // 켰을 때 항상 알파 1로 초기화
        SetAlpha(1f);

        isShowing = true;
        timer = 0f;
        localTime = 0f;
    }

    void Update()
    {
        if (!isShowing) return;

        localTime += Time.deltaTime;

        // 깜빡임: 알파를 0.5~1 사이로 깜박이도록 조정
        float alpha = 0.5f + 0.5f * Mathf.PingPong(localTime * flashSpeed, 1f);
        SetAlpha(alpha);

        timer += Time.deltaTime;
        if (timer >= stayTime)
        {
            // 깜빡임 종료 후, 알파 1로 복원
            SetAlpha(1f);
            isShowing = false;
            gameObject.SetActive(false);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (mainUI != null)
        {
            Color c = mainUI.color;
            c.a = alpha;
            mainUI.color = c;
        }

        if (bossText != null)
        {
            Color c = bossText.color;
            c.a = alpha;
            bossText.color = c;
        }

        if (noticeImage != null)
        {
            Color c = noticeImage.color;
            c.a = alpha;
            noticeImage.color = c;
        }

        if (noticeImage2 != null)
        {
            Color c = noticeImage2.color;
            c.a = alpha;
            noticeImage2.color = c;
        }
    }

}
