using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    public static PlayerCardManager Instance;
    public Player player; // 기존 Player.cs
    public List<PlayerCard> ownedCards = new List<PlayerCard>();
    public PlayerMove1 move1;
    void Awake()
    {
        Debug.Log("PlayerCardManager 생성됨: " + gameObject.name);
    }

    void Start()
    {
        LoadCardsFromCardManager();
        if (SlotSelectContext.Instance.mode == SlotSelectMode.NewGame)
        {
            ClearCards(); // 이전 슬롯 카드 모두 삭제
            move1.ClearBubbles();
            Debug.Log("게임 씬 → PlayerCardManager 카드 초기화");
        }
    }

    public void GainCard(CardBaseData cardData)
    {
        PlayerCard card = ownedCards.Find(c => c.data.CardID == cardData.CardID);
        Debug.Log("GainCard 호출됨: ");
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
    void LoadCardsFromCardManager()
    {
        // DontDestroyOnLoad 상태의 CardManager 찾기
        CardManager cardManager = FindObjectOfType<CardManager>();

        if (cardManager == null)
        {
            Debug.LogError("CardManager를 찾을 수 없음!");
            return;
        }

        if (cardManager.selectCardNames == null || cardManager.selectCardNames.Count == 0)
        {
            Debug.Log("적용할 카드 없음");
            return;
        }

        foreach (string cardName in cardManager.selectCardNames)
        {
            GameObject prefab = cardManager.GetPrefabByName(cardName);

            if (prefab == null)
            {
                Debug.LogError("카드 프리팹 없음: " + cardName);
                continue;
            }

            Card card = prefab.GetComponent<Card>();

            if (card == null)
            {
                Debug.LogError("Card 컴포넌트 없음: " + cardName);
                continue;
            }

            GainCard(card.cardData);
        }

        Debug.Log("PlayerCardManager 카드 적용 완료");
    }
    void ApplyCardToPlayer(CardBaseData card, float value)
    {
        Debug.Log("ApplyCardEffect 호출됨");
        Debug.Log("effectType: " + card.effectType);
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
                    int increase = Mathf.CeilToInt(move1.moveCount * value);
                    move1.moveCount += increase;
                    move1.moveRemain += increase;

                    Debug.Log("moveRemain 증가됨(Percent): +" + increase);
                    Debug.Log("현재 moveRemain: " + move1.moveRemain);

                }
                else
                {
                    int increase = Mathf.CeilToInt(value);
                    move1.moveCount += increase;
                    move1.moveRemain += increase;

                    Debug.Log("moveRemain 증가됨(Fixed): +" + increase);
                    Debug.Log("현재 moveRemain: " + move1.moveRemain);
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

            case CardEffectType.Boomerang:
                player.boomerangLevel += Mathf.RoundToInt(value);

                Debug.Log("부메랑 레벨 증가: " + player.boomerangLevel);

                break;

            case CardEffectType.Counter:
                player.PassiveCounter = true;
                Debug.Log("카운터 활성화");

                break;

            case CardEffectType.BoundUp:
                player.hasSpikeball = true;
                break;

            case CardEffectType.Emergency:
                player.emergency = true;
                break;

            case CardEffectType.Revival:
                player.Revival = true;
                break;

            case CardEffectType.Lucky:
                AddLuckyChanceToPlayer(value);
                break;

            case CardEffectType.TwoPick:

                GameManager.Instance.CardMgr.twoPickValue = value;
                Debug.Log($"TwoPick 카드 적용: value = {value}% 확률");
                break;

            case CardEffectType.Angry:
                player.Anger = true;
                if (card.isPercent)
                    player.PlayerATK = Mathf.RoundToInt(player.PlayerATK * (1f + value));
                else
                    player.PlayerATK += Mathf.RoundToInt(value);
                break;
            case CardEffectType.Revenge:
                player.revenge = true;
                player.revengeAmount += value; // 중복 카드일 경우 누적
                Debug.Log($"복수 카드 적용 → 누적 공격력 증가 {player.revengeAmount * 100}%");
                break;

            case CardEffectType.MagnetRange:
                player.pickupDistanceBonus += value; // CSV 수치만큼 누적
                Debug.Log($"자석 거리 증가: +{value} → 총 {player.pickupDistanceBonus}");
                break;

            case CardEffectType.BonusExp:
                player.bonusExpIncrease += Mathf.RoundToInt(value); // CSV 값 그대로 누적
                Debug.Log($"보너스 경험치 카드 적용 → 누적 +{player.bonusExpIncrease}");
                break;

            case CardEffectType.BloodDamage:
                player.hasBloodDamage = true;
                // isPercent에 따라 확률 계산
                if (card.isPercent)
                    player.bloodDamageChance = value * 100f; // value = 0.05 → 5%
                else
                    player.bloodDamageChance = value;// value = 5 → 5%

                player.bloodDamagePerTick = 1;   // 틱 데미지 1
                player.bloodDamageTurns = 3;     // 3턴 지속

                Debug.Log($"블러드 카드 적용: 확률 {player.bloodDamageChance}% / 틱데미지 {player.bloodDamagePerTick} / {player.bloodDamageTurns}턴 지속");
                break;
            case CardEffectType.FireCount:
                player.hasSward = true;
                player.playerfireLevel += Mathf.RoundToInt(value);
                Debug.Log("1자스트아리크 레벨 증가: " + player.playerfireLevel);
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

    //새 게임일때 초기화
    public void ClearCards()
    {
        ownedCards.Clear();
        Debug.Log("PlayerCardManager 초기화 완료");
    }

    private void AddLuckyChanceToPlayer(float increase)
    {
        player.LuckyChance += increase; // CSV 값 그대로 누적
        //player.LuckyChance = Mathf.Clamp01(player.LuckyChance); // 최대 1(100%) 제한

        Debug.Log($"럭키 회피율 증가: {increase} → 현재 LuckyChance: {player.LuckyChance}");

    }
}
