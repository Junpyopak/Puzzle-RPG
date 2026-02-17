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

            case CardEffectType.HpPercent: // 회복 카드
                //if (card.isPercent)
                //{
                //    player.MaxHp = Mathf.RoundToInt(player.MaxHp * (1f + value));
                //    if (player.Hp > player.MaxHp) player.Hp = player.MaxHp;
                //}
                //else
                //{
                //    player.MaxHp += Mathf.RoundToInt(value);
                //    player.Hp += Mathf.RoundToInt(value);
                //}
                int healAmount;

                if (card.isPercent)
                {
                    healAmount = Mathf.RoundToInt(player.MaxHp * value);
                }
                else
                {
                    healAmount = Mathf.RoundToInt(value);
                }

                player.Hp += healAmount;

                // 최대체력 초과 방지
                if (player.Hp > player.MaxHp)
                    player.Hp = player.MaxHp;
                break;   
            case CardEffectType.MaxHpUp:// 최대체력 증가 카드
                int increaseAmount;

                if (card.isPercent)
                {
                    increaseAmount = Mathf.RoundToInt(player.MaxHp * value);
                }
                else
                {
                    increaseAmount = Mathf.RoundToInt(value);
                }

                player.MaxHp += increaseAmount;
                // 증가한 만큼 회복
                player.Hp += increaseAmount;

                // 현재체력이 최대체력보다 클 경우만 제한
                if (player.Hp > player.MaxHp)
                    player.Hp = player.MaxHp;
                break;

            case CardEffectType.Recovery:
                player.Recovery = true;
                player.recoveryAmount += Mathf.RoundToInt(value);
                break;

            case CardEffectType.CountUp:
                if (card.isPercent)
                {
                    move1.moveCount = Mathf.RoundToInt(move1.moveCount * (1f + value));
                    move1.ResetMove();
                }
                break;  
            case CardEffectType.Bubble:
                if (card.isPercent)
                {
                    // AttackPercent처럼 float value를 그대로 적용
                    move1.CreateBubbleShield(value); // value = CSV 기반 float, 0.03 = 3%
                }
                else
                {
                    // 직접 수치 증가일 경우 (optional)
                    move1.CreateBubbleShield(value / player.PlayerATK);
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
