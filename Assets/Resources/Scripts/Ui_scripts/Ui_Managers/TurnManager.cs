using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    public PlayerMove1 playerMove;
    public Player player;
    public List<Monster> monsters = new List<Monster>();
    public PuzzleBoard board;

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
        Debug.Log("MonsterTurnCoroutine 실행됨");
        monsterTurnRunning = true;
        Turn_Timer.Instance.ShowMonsterUI();
        //몬스터 턴 동안 플레이어 이동 금지
        playerMove.isPlayerTurn = false;
        board.isPlayerTurn = false;
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
        playerMove.isPlayerTurn = true;
        board.isPlayerTurn = true;
        player.Attack();
        player.ShootBoomerangs();
        player.ShootSpikeBall();
        player.OnTurnEndRecovery();
        playerMove.ResetMove();
        monsterTurnRunning = false;
        yield return new WaitForSeconds(1f);

        // 다음 라운드 시작
        Turn_Timer.Instance.StartTurn();
    }
}
