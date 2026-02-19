using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterData
{
    public int ID;
    public string Name;
    public int RemainTurn;
    public int AttackRange;
    public MonsterType MonsterType;
    public int Hp;
    public int Atk;

    //경험치 확률
    public float ExpNormal;
    public float ExpAlpha;
    public float ExpSuper;

    //회복음식 확률
    public float FoodDrop;

    //아이템 상자 확률
    public float BoxDrop;
}
