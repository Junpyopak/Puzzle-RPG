using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int monsterID;

    void Start()
    {
        MonsterData data =
            MonsterDataTable.Instance.monsterDic[monsterID];

        Debug.Log($"{data.Name} 생성 / 타입 : {data.MonsterType}");
    }
}

