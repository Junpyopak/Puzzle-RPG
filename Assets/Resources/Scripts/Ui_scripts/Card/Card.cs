using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public enum CardRarity
    {
        General,
        Legend
    }

    public CardRarity rarity;
    [HideInInspector]
    public CardBaseData cardData;
    // 아래에 카드의 이름, 공격력, 효과 등 추가 데이터를 넣기
    public string cardName;
    public string cardToolTip;
    public int CardID;

    [Header("UI 컴포넌트")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI toolTipText;


    void Awake()
    {
        LoadData();
        FindUIComponents();
        UpdateUI();
    }
    public void LoadData()
    {
        if (CardDatabase.Instance == null)
        {
            Debug.LogError("CardDatabase 없음");
            return;
        }

        cardData = CardDatabase.Instance.GetCardByID(CardID);

        if (cardData != null)
        {
            cardName = cardData.CardName;
            cardToolTip = cardData.Description;
        }

        // 필요하면 자동 동기화
        // name = data.CardName;
    }

    // 자식에서 이름 기준으로 UI 컴포넌트 찾기
    private void FindUIComponents()
    {
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var t in texts)
        {
            if (t.gameObject.name == "CardName")
                nameText = t;
            else if (t.gameObject.name == "CardToolTip")
                toolTipText = t;
        }

        if (nameText == null)
            Debug.LogWarning("CardName TextMeshProUGUI를 찾지 못했습니다.");
        if (toolTipText == null)
            Debug.LogWarning("CardToolTip TextMeshProUGUI를 찾지 못했습니다.");
    }


    public void UpdateUI()
    {
        if (nameText != null) nameText.text = cardName;
        if (toolTipText != null) toolTipText.text = cardToolTip;
    }

}
