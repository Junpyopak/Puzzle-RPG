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
}

