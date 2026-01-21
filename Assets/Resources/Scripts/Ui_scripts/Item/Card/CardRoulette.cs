using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;

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
    public Text Rolltext;

    [Header("Spacing")]
    public float cardGap = 20f;

    public GameObject OpenItem;
    public GameObject RollButton;
    public GameObject RolletPanel;

    void Awake()
    {
        //자식 자동 수집
        int count = transform.childCount;
        cards = new RectTransform[count];

        for (int i = 0; i < count; i++)
        {
            cards[i] = transform.GetChild(i).GetComponent<RectTransform>();
        }
        if (OpenItem == null)
        {
            OpenItem = GameObject.Find("OpenItem");
            if (OpenItem == null)
                Debug.LogWarning("Rollet UI를 찾을 수 없습니다!");
        }

       OpenItem.SetActive(false);
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
                RectTransform pickedCard = GetCenterCard();
                SnapToCenter(pickedCard);

                return;
            }
        }

        MoveCards();
        UpdateCardScale();
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
        StartCoroutine(StopRolling());
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

        IEnumerator StopRolling()
        {
            //var locale = LocalizationSettings.SelectedLocale;
            //var table = LocalizationSettings.StringDatabase.GetTable("Btn_Language");

            //var StopEntry = table.GetEntry(LabelKey);
            //string Rolltext = StopEntry?.GetLocalizedString(new object[] { slotIndex + 1 }) ?? $"슬롯 {slotIndex + 1}";
            var tableOp = LocalizationSettings.StringDatabase.GetTableAsync("Btn_Language");
            yield return tableOp;

            StringTable table = tableOp.Result;
            Rolltext.text = table.GetEntry("Stop").GetLocalizedString();
            //Rolltext.text = ("멈추기");
            yield return new WaitForSeconds(3f); ;
        }
    }

    void OnEnable()
    {
        OpenItem.SetActive(false);
        ResetRouletteState();
        RandomCard5();
    }

    //private void RandomCard5()
    //{
    //    if (CardDatabase.Instance == null)
    //    {
    //        Debug.LogError("CardDatabase.Instance 없음");
    //        return;
    //    }

    //    List<CardBaseData> pool = CardDatabase.Instance.cardList;

    //    if (pool == null || pool.Count < 5)
    //    {
    //        Debug.LogError("카드 수 부족");
    //        return;
    //    }

    //    // 원본 보호용 복사 리스트
    //    List<CardBaseData> temp = new List<CardBaseData>(pool);

    //    Debug.Log("=== 카드 룰렛 결과 ===");

    //    for (int i = 0; i < 5; i++)
    //    {
    //        int rand = Random.Range(0, temp.Count);
    //        CardBaseData picked = temp[rand];
    //        //CardSprite visual = CardSpriteManager.instance.GetVisual(picked.CardID);
    //        Debug.Log($"{i + 1}. {picked.CardName} | Type:{picked.CardType} | Grade:{picked.Grade} | Description : {picked.Description}");

    //        temp.RemoveAt(rand); // 중복 방지
    //    }

    //}
    public void RandomCard5()
    {
        if (CardDatabase.Instance == null)
        {
            Debug.LogError("CardDatabase.Instance 없음");
            return;
        }

        List<CardBaseData> pool = CardDatabase.Instance.cardList;

        if (pool == null || pool.Count < 5)
        {
            Debug.LogError("카드 수 부족");
            return;
        }

        // 원본 보호용 복사 리스트
        List<CardBaseData> temp = new List<CardBaseData>(pool);

        Debug.Log("=== 카드 룰렛 결과 ===");

        for (int i = 0; i < 5; i++)
        {
            int rand = Random.Range(0, temp.Count);
            CardBaseData picked = temp[rand];

            // CardSprite 가져오기
            CardSprite visual = CardSpriteManager.instance.GetVisual(picked.CardID);

            // UI 카드 슬롯에 이미지 적용
            if (i < cards.Length)
            {
                Image img = cards[i].GetComponent<Image>();
                if (img != null && visual != null)
                {
                    img.sprite = visual.Sprite;  // CardSprite SO 안에 Sprite 필드
                    img.SetNativeSize();            // 필요 시 크기 맞춤
                }
                else
                {
                    Debug.LogWarning($"cards[{i}] Image 컴포넌트 또는 visual null");
                }
            }

            // 로그 출력
            Debug.Log($"{i + 1}. {picked.CardName} | Type:{picked.CardType} | Grade:{picked.Grade} | Description : {picked.Description} | Image: {visual.name}");

            temp.RemoveAt(rand); // 중복 방지
        }
    }


    void UpdateCardScale()
    {
        float centerX = 0f; // 부모 기준 중앙
        float maxScale = 1.2f;
        float minScale = 0.9f;
        float effectRange = cardWidth; // 중앙 영향 범위

        for (int i = 0; i < cards.Length; i++)
        {
            float dist = Mathf.Abs(cards[i].anchoredPosition.x - centerX);

            float t = Mathf.Clamp01(dist / effectRange);
            float targetScale = Mathf.Lerp(maxScale, minScale, t);

            // 부드럽게 스케일 변화
            cards[i].localScale =
                Vector3.Lerp(cards[i].localScale,
                             Vector3.one * targetScale,
                             Time.deltaTime * 8f);
        }
    }

    //가운데 카드 찾기
    RectTransform GetCenterCard()
    {
        RectTransform centerCard = cards[0];
        float minDist = Mathf.Abs(cards[0].anchoredPosition.x);

        for (int i = 1; i < cards.Length; i++)
        {
            float dist = Mathf.Abs(cards[i].anchoredPosition.x);
            if (dist < minDist)
            {
                minDist = dist;
                centerCard = cards[i];
            }
        }

        return centerCard;
    }

    //멈췄을 때 중앙으로
    void SnapToCenter(RectTransform target)
    {
        float offset = target.anchoredPosition.x;

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].anchoredPosition -= new Vector2(offset, 0f);
        }
        StartCoroutine(EndDrawCard());
    }

    IEnumerator EndDrawCard()
    { 
        yield return new WaitForSeconds(1.5f);

        //gameObject.SetActive(false);
        //RollButton.SetActive(false);
        RolletPanel.SetActive(false);
        Turn_Timer.Instance.isPaused =false;
        UI_GameTimer.Instance.isPaused = false;

    }

   public void ResetRouletteState()
    {
        isRolling = false;
        isdecrease = false;
        speed = 1200f;

        // 카드 위치 리셋 (초기 위치 저장했다가 복구 권장)
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].anchoredPosition = new Vector2(
                i * (cardWidth + cardGap),
                cards[i].anchoredPosition.y
            );
            cards[i].localScale = Vector3.one;
        }

        // 버튼 & 텍스트
        RollButton.SetActive(true);
    }
}
