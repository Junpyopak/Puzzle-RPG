using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    public Player player; // 기존 Player.cs
    public List<PlayerCard> ownedCards = new List<PlayerCard>();
    public PlayerMove1 move1;
    void Awake()
    {
        Debug.Log("PlayerCardManager 생성됨: " + gameObject.name);
    }
    public void GainCard(CardBaseData cardData)
    {
        PlayerCard card = ownedCards.Find(c => c.data.CardID == cardData.CardID);
        Debug.Log("GainCard 호출됨: " );
        if (card == null)
        {
            // 새 카드 획득
            card = new PlayerCard { data = cardData, level = 1 };
            ownedCards.Add(card);
            ApplyCardToPlayer(cardData, card.GetValue());
        }
        else
        {
            // 레벨업
            if (card.level >= cardData.MaxLevel) return;

            float before = card.GetValue();
            card.level++;
            float after = card.GetValue();
            float diff = after - before;

            ApplyCardToPlayer(cardData, diff);
        }

        Debug.Log($"카드 획득/레벨업: {cardData.CardName} | Lv:{card.level}");
    }

    void ApplyCardToPlayer(CardBaseData card, float value)
    {
        switch (card.effectType)
        {
            case CardEffectType.AttackPercent:
                if (card.isPercent)
                    player.PlayerATK = Mathf.RoundToInt(player.PlayerATK * (1f + value));
                else
                    player.PlayerATK += Mathf.RoundToInt(value);
                break;

            case CardEffectType.DefensePercent:
                if (card.isPercent)
                    player.Defence = Mathf.RoundToInt(player.Defence * (1f + value));
                else
                    player.Defence += Mathf.RoundToInt(value);
                break;

            case CardEffectType.HpPercent:
                if (card.isPercent)
                {
                    player.MaxHp = Mathf.RoundToInt(player.MaxHp * (1f + value));
                    if (player.Hp > player.MaxHp) player.Hp = player.MaxHp;
                }
                else
                {
                    player.MaxHp += Mathf.RoundToInt(value);
                    player.Hp += Mathf.RoundToInt(value);
                }
                break; 
            case CardEffectType.CountUp:
                if (card.isPercent)
                {
                    move1.moveCount = Mathf.RoundToInt(move1.moveCount * (1f + value));
                    move1.ResetMove();
                }
                break; 

        }
    }
    public List<CardSaveData> GetSaveData()
    {
        List<CardSaveData> list = new List<CardSaveData>();

        foreach (var card in ownedCards)
        {
            CardSaveData save = new CardSaveData();
            save.cardID = card.data.CardID;
            save.level = card.level;

            list.Add(save);
        }

        return list;
    }
    public void LoadFromSaveData(List<CardSaveData> saveList)
    {
        ownedCards.Clear();

        foreach (var save in saveList)
        {
            CardBaseData data = CardDatabase.Instance.GetCardByID(save.cardID);

            if (data == null)
            {
                Debug.LogError("카드 데이터 없음 ID: " + save.cardID);
                continue;
            }

            PlayerCard card = new PlayerCard
            {
                data = data,
                level = save.level
            };

            ownedCards.Add(card);

            // 전체 효과 적용
            ApplyCardToPlayer(data, card.GetValue());
        }

        Debug.Log("카드 불러오기 완료");
    }
}
