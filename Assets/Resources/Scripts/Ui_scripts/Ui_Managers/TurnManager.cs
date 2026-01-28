using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public PlayerMove1 player;
    public List<Monster> monsters = new List<Monster>();

    bool monsterTurnRunning = false;

    void Awake()
    {
        Instance = this;
    }

    // Turn_Timer.EndTurn() 에서 호출
    public void StartMonsterTurn()
    {
        if (!monsterTurnRunning)
            StartCoroutine(MonsterTurnCoroutine());
    }

    IEnumerator MonsterTurnCoroutine()
    {
        monsterTurnRunning = true;

        //몬스터 턴 동안 플레이어 이동 금지
        player.hasMoved = true;

        // 몬스터 전부 행동
        foreach (Monster m in monsters)
        {
            m.StartTurn();

            while (!m.Act())
            {
                yield return new WaitForSeconds(0.1f); // 한 칸씩 연출
            }
        }

        //모든 몬스터 행동 종료
        player.hasMoved = false;
        monsterTurnRunning = false;

        // 다음 라운드 시작
        Turn_Timer.Instance.StartTurn();
    }
}
