using UnityEngine;

public class Card : MonoBehaviour
{
    public enum CardRarity
    {
        General,
        Legend
    }

    public CardRarity rarity;
    // 아래에 카드의 이름, 공격력, 효과 등 추가 데이터를 넣기
    public string cardName;
}
