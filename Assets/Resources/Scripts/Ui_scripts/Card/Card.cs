using UnityEngine;

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
    public int CardID;

    void Awake()
    {
        LoadData();
    }
    public void LoadData()
    {
        if (CardDatabase.Instance == null)
        {
            Debug.LogError("CardDatabase 없음");
            return;
        }

        cardData = CardDatabase.Instance.GetCardByID(CardID);

        if (cardData == null)
        {
            Debug.LogError("CardID에 해당하는 데이터 없음: " + CardID);
        }

        // 필요하면 자동 동기화
        // name = data.CardName;
    }
}
