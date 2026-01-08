using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRoulette : MonoBehaviour
{
    [Header("Cards")]
    public RectTransform[] cards;   // 카드들 (왼쪽 → 오른쪽 순서)
    public float cardWidth = 200f;  // 카드 하나의 폭

    [Header("Move")]
    public float speed = 1200f;      // 이동 속도
    public bool isRolling = false;
    public bool isdecrease = false;
    public float decreasespeed = 2.0f;

    [Header("Spacing")]
    public float cardGap = 20f;

    void Awake()
    {
        //자식 자동 수집
        int count = transform.childCount;
        cards = new RectTransform[count];

        for (int i = 0; i < count; i++)
        {
            cards[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (!isRolling) return;

        if (isdecrease)
        {
            speed = Mathf.Lerp(speed, 0f, decreasespeed * Time.deltaTime);

            if (speed <= 0.01f)
            {
                speed = 0f;
                isRolling = false;
                isdecrease = false;
                return;
            }
        }

        MoveCards();
    }
    void MoveCards()
    {
        float move = speed * Time.deltaTime;

        RectTransform parent = (RectTransform)transform;

        float leftEdge = -parent.rect.width * 0.5f;
        float cardHalf = cardWidth * 0.5f;

        float spacing = cardWidth + cardGap;

        // 현재 가장 오른쪽 카드 위치
        float rightMostX = float.MinValue;
        for (int i = 0; i < cards.Length; i++)
            rightMostX = Mathf.Max(rightMostX, cards[i].anchoredPosition.x);

        for (int i = 0; i < cards.Length; i++)
        {
            RectTransform card = cards[i];

            // 이동
            card.anchoredPosition += Vector2.left * move;

            // 왼쪽을 완전히 벗어났으면
            if (card.anchoredPosition.x < leftEdge - cardHalf)
            {
                card.anchoredPosition =
                    new Vector2(rightMostX + spacing, card.anchoredPosition.y);

                // 다음 카드 기준 갱신
                rightMostX = card.anchoredPosition.x;
            }
        }
    }
    public void StartRoll()
    {
        if (isRolling)
        {
            isdecrease = true;
            return;
        }

        // 멈춰있으면 → 다시 시작
        isRolling = true;
        isdecrease = false;
    }

}
