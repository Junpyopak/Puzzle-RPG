using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardBaseData
{
    public int CardID;
    public string CardName;
    public string CardType;// 액티브 / 패시브
    public string Description;//카드 설명
    public string Grade;//등급
    public int MaxLevel;//최대 레벨

    [Header("Effect")]
    public CardEffectType effectType;
    public bool isPercent = true;   // true = 퍼센트, false = 직접 수치 증가
    public float baseValue;         // 퍼센트면 0.1 = 10%, 직접 수치면 5
    public float valuePerLevel;     // 레벨당 증가량

    public float GetValue(int level)
    {
        return baseValue + (level - 1) * valuePerLevel;
    }
}

