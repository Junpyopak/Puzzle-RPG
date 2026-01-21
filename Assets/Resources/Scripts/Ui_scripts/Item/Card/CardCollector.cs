using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class CardCollector : MonoBehaviour
{
    public static CardCollector Instance;
    [Header("UI")]
    public Transform content;       // ScrollView Content
    public GameObject slotPrefab;   // 카드 슬롯 프리팹

    [Header("Settings")]
    public int totalSlotCount = 30;
    private bool isInitialized = false;
    public int SlotCount = 0;

    public List<CardSlot> slots = new List<CardSlot>();

    [Header("Card Data")]
    public List<CardSprite> cardSpriteList = new List<CardSprite>();
    public CardGainData cardGainData;

    private void Awake()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject); // 필요 시
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }
    private void OnEnable()
    {
        if (isInitialized) return;
        if (CardSpriteManager.instance != null)
        {
            // CSV가 이미 로드된 상태라면 바로 참조
            cardSpriteList = CardSpriteManager.instance.cardSprite;

            // 획득 카드 데이터
            cardGainData = CardGainDataHolder.Instance.Data;
        }
        else
        {
            Debug.LogError("CardDatabase 인스턴스를 찾을 수 없음!");
        }
        CreateSlots();
        DebugGainedCardIDs();
        ShowGainedCards();
        isInitialized = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void CreateSlots()
    {
        SlotCount = content.childCount;
        if (SlotCount < totalSlotCount)
        {
            for (int i = SlotCount; i < totalSlotCount; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, content);

                // 카드 슬롯 컴포넌트 가져오기
                CardSlot slot = slotObj.GetComponent<CardSlot>();
                if (slot != null)
                {
                    slots.Add(slot); // slots 리스트에 추가
                }

                // 확인용 이름
                slotObj.name = $"CardSlot_{i + 1}";
            }
        }
        else
        {
            // 이미 존재하는 슬롯도 리스트에 추가
            foreach (Transform child in content)
            {
                CardSlot slot = child.GetComponent<CardSlot>();
                if (slot != null && !slots.Contains(slot))
                    slots.Add(slot);
            }
        }

        SlotCount = content.childCount; // 안전하게 동기화
    }
    //카드 획득 시 호출할 함수
    public void AddCard(Sprite cardSprite)
    {
        foreach (CardSlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.SetCard(cardSprite);
                return;
            }
        }

        Debug.Log("카드 도감 슬롯이 가득 찼습니다!");
    }
    //public void LoadCardsFromDatabase(CardDatabase database)
    //{
    //    if (database == null) return;

    //    // cardDataList를 database.cardList 참조로 연결
    //    cardDataList = database.cardList;

    //    Debug.Log($"카드 데이터 {cardDataList.Count}개 로드 완료");
    //}
    void DebugGainedCardIDs()
    {
        if (CardGainDataHolder.Instance == null ||
            CardGainDataHolder.Instance.Data == null)
        {
            Debug.LogWarning("CardGainDataHolder 또는 Data 없음");
            return;
        }

        var list = CardGainDataHolder.Instance.Data.gainedCardIDs;

        if (list.Count == 0)
        {
            Debug.Log("획득한 카드 없음");
            return;
        }

        Debug.Log("=== 획득한 카드 ID 목록 ===");
        foreach (int id in list)
        {
            Debug.Log($"CardID: {id}");
        }
    }

    void ShowGainedCards()
    {
        if (CardGainDataHolder.Instance == null ||
            CardGainDataHolder.Instance.Data == null)
        {
            Debug.LogWarning("CardGainDataHolder 또는 Data 없음");
            return;
        }

        var list = CardGainDataHolder.Instance.Data.gainedCardIDs;

        if (list.Count == 0)
        {
            Debug.Log("표시할 카드 없음");
            return;
        }

        int slotIndex = 0;

        foreach (int id in list)
        {
            CardSprite visual = CardSpriteManager.instance.GetVisual(id);

            if (visual == null)
            {
                Debug.LogWarning($"CardID {id} 스프라이트 없음");
                continue;
            }

            if (slotIndex >= slots.Count)
            {
                Debug.LogWarning("카드 도감 슬롯 부족");
                break;
            }

            slots[slotIndex].SetCard(visual.Sprite);
            slotIndex++;
        }
    }
}
