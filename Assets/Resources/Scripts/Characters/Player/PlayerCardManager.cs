using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    public Player player; // ±‚¡∏ Player.cs
    public List<PlayerCard> ownedCards = new List<PlayerCard>();
    void Awake()
    {
        Debug.Log("PlayerCardManager ª˝º∫µ : " + gameObject.name);
    }
    public void GainCard(CardBaseData cardData)
    {
        PlayerCard card = ownedCards.Find(c => c.data.CardID == cardData.CardID);
        Debug.Log("GainCard »£√‚µ : " );
        if (card == null)
        {
            // ªı ƒ´µÂ »πµÊ
            card = new PlayerCard { data = cardData, level = 1 };
            ownedCards.Add(card);
            ApplyCardToPlayer(cardData, card.GetValue());
        }
        else
        {
            // ∑π∫ßæ˜
            if (card.level >= cardData.MaxLevel) return;

            float before = card.GetValue();
            card.level++;
            float after = card.GetValue();
            float diff = after - before;

            ApplyCardToPlayer(cardData, diff);
        }

        Debug.Log($"ƒ´µÂ »πµÊ/∑π∫ßæ˜: {cardData.CardName} | Lv:{card.level}");
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
        }
    }
}
