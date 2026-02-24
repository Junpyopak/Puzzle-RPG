using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMgr : MonoBehaviour
{
    public static SpawnMgr Instance;
    public PoolMgr poolMgr;            // PoolMgr 연결
    public Transform[] spawnPoints;    // 스폰 포인트 배열
    //public float spawnInterval = 3f;   // 스폰 간격
    public int spawnCount = 1;         // 한 번에 스폰할 수
    public int maxMonsterCount = 10;   // 씬에 최대 몬스터 수

    private int nextSpawnIndex = 0;    // 다음 스폰 포인트 시작 인덱스
    private BossPanel bossPanel;


    [SerializeField] private float[] timeThresholds;    // 시간 경과별 활성 포인트 기준 (초)
    private int activeSpawnPointCount = 1;              // 현재 활성 포인트 수
    private float gameTimer = 0f;                       // 전체 게임 타이머

    
    [Header("사운드")]
    public AudioClip bossAppears; //보스 등장음

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        bossPanel = FindObjectOfType<BossPanel>();
        if (timeThresholds.Length > 4)
            Debug.LogWarning("timeThresholds는 최대 4개까지만 사용하세요.");
        if (SaveContext.Instance != null && SaveContext.Instance.isLoading)
        {
            RestoreMonstersFromSave();
        }
        else
        {
            //SpawnNewGameMonsters();
            StartCoroutine(SpawnFirstRoundDelay());

        }
        // StartCoroutine(SpawnRoutine());
    }
    private IEnumerator SpawnFirstRoundDelay()
    {
        yield return null; // 한 프레임 대기

        SpawnNewGameMonsters();
    }
    private void Update()
    {
        gameTimer += Time.deltaTime;

        // 게임 시간이 지남에 따라 활성 포인트 수 업데이트
        for (int i = activeSpawnPointCount; i < timeThresholds.Length; i++)
        {
            if (gameTimer >= timeThresholds[i])
            {
                activeSpawnPointCount = i + 1;
            }
            else
            {
                break; // 아직 다음 시간 안됨
            }
        }
    }
    //private IEnumerator SpawnRoutine()
    //{
    //    while (true)
    //    {
    //        SpawnEnemies(spawnCount);
    //        yield return new WaitForSeconds(spawnInterval);
    //    }
    //}

    //void RestoreMonstersFromSave()
    //{
    //    SaveData save = SaveContext.Instance.currentSaveData;

    //    foreach (var m in save.monsters)
    //    {
    //        GameObject obj = poolMgr.GetEnemy(m.enemyTypeIndex);
    //        Monster monster = obj.GetComponent<Monster>();

    //        monster.LoadFromSaveData(m);
    //    }

    //    Debug.Log($"[이어하기] 몬스터 {save.monsters.Count}마리 복원 완료");
    //}

    void RestoreMonstersFromSave()
    {
        SaveData save = SaveContext.Instance.currentSaveData;

        // 🔥 핵심: 기존 몬스터 전부 비활성화
        foreach (var pool in poolMgr.Pools)
        {
            foreach (var enemy in pool)
            {
                if (enemy.activeInHierarchy)
                    poolMgr.ReturnEnemy(enemy);
            }
        }

        // 🔥 저장된 몬스터만 다시 활성화
        foreach (var m in save.monsters)
        {
            GameObject obj = poolMgr.GetEnemy(m.enemyTypeIndex);
            Monster monster = obj.GetComponent<Monster>();

            monster.LoadFromSaveData(m);
        }

        Debug.Log($"[이어하기] 몬스터 {save.monsters.Count}마리 복원 완료");
    }

    // ================= 새게임 =================
    void SpawnNewGameMonsters()
    {
        Debug.Log("[새게임] 라운드 기본 스폰");

        SpawnOnRoundStart();
    }


    public void SpawnOnRoundStart()
    {
        SpawnEnemies(spawnCount);
    }

    //private void SpawnEnemies(int count)
    //{
    //    int activeMonsterCount = GetActiveMonsterCount();
    //    if (activeMonsterCount >= maxMonsterCount)
    //        return; // 이미 충분하면 스폰하지 않음

    //    for (int i = 0; i < count; i++)
    //    {
    //        if (activeMonsterCount >= maxMonsterCount)
    //            break;

    //        //int typeIndex = Random.Range(0, poolMgr.EnemyPrefabs.Length);
    //        int typeIndex = GetEnemyTypeIndexByRound();
    //        GameObject enemy = poolMgr.GetEnemy(typeIndex);

    //        if (enemy != null)
    //        {
    //            // 반시계 방향 순환 + 활성 포인트 범위 내
    //            Transform point = spawnPoints[nextSpawnIndex % activeSpawnPointCount];
    //            enemy.transform.position = point.position;
    //            enemy.transform.rotation = point.rotation;

    //            nextSpawnIndex++;
    //            activeMonsterCount++;
    //        }
    //    }
    //}
    private void SpawnEnemies(int count)
    {
        // 현재 활성 몬스터 수
        int activeMonsterCount = GetActiveMonsterCount();

        // 실제로 스폰 가능한 수
        int canSpawn = Mathf.Min(count, maxMonsterCount - activeMonsterCount);
        if (canSpawn <= 0) return; // 이미 최대치면 종료

        for (int i = 0; i < canSpawn; i++)
        {
            int typeIndex = GetEnemyTypeIndexByRound();
            GameObject enemy = poolMgr.GetEnemy(typeIndex);

            if (enemy != null)
            {
                Transform point = spawnPoints[nextSpawnIndex % activeSpawnPointCount];
                enemy.transform.position = point.position;
                enemy.transform.rotation = point.rotation;

                nextSpawnIndex++;
            }
        }
    }

    private int GetActiveMonsterCount()
    {
        int count = 0;
        foreach (var pool in poolMgr.Pools)
        {
            foreach (var enemy in pool)
            {
                if (enemy.activeInHierarchy)
                    count++;
            }
        }
        return count;
    }
    private int GetEnemyTypeIndexByRound()
    {
        int round = Turn_Timer.Instance.TurnCount -1;
        //float rand =  Random.value;

        //// 1 ~ 4라운드 : 스켈레톤만
        //if (round <= 4)
        //{
        //    return 1; // 스켈레톤
        //}
        //// 5라운드 : 리자드만
        //else if (round == 5)
        //{
        //    return 0; // 리자드
        //}
        //// 8라운드 :  소악마
        //else if (round == 8)
        //{
        //    return 2; //소악마
        //}
        //// 6 ~ 9 라운드 : 섞어서 (선택)
        //else if (round < 10)
        //{
        //    // 확률로 스켈레톤 / 리자드 스폰 확률 나누기
        //    if (rand < 0.7f)
        //    {
        //        return 2; //소악마
        //    }
        //    else if (rand < 0.9f)
        //    {
        //        return 0; // 리자드 (20%)
        //    }
        //    else
        //    {
        //        return 1; //스켈레톤 (10%)
        //    }
        //}

        //// 10 라운드 이상 : 리자드만
        //else
        //{
        //    return 0; // 리자드
        //}
        int[] weights = GetWeightsByRound(round);

        return GetWeightedRandomIndex(weights);
    }
    private int[] GetWeightsByRound(int round)
    {
        // [슬라임,스켈,리자드,소악마,오크,오크보스]
        if (round <= 4)
            return new int[] { 100, 0, 0, 0, 0, 0 };

        if (round == 5)
            return new int[] { 0,100, 0, 0, 0,0 };

        if (round <= 7)
            return new int[] { 30, 70, 0, 0, 0, 0 };

        if (round == 8)
            return new int[] { 0, 0, 0 , 100, 0, 0 };

        if (round <= 10)
            return new int[] { 50, 20, 20, 10, 0 , 0};

        if (round == 15)
            return new int[] { 0, 0, 100, 0, 0, 0 };

        if (round <= 16)
            return new int[] { 10, 20, 20, 50, 0 , 0 };

        if(round ==24)
            return new int[] { 0, 0, 0, 0, 100,0 };

        // 50배수 50,100,150,200 등 보스 라운드
        if (round % 50 == 0)
        {
            bossPanel.ShowPopup();
            GameManager.Instance.SoundMgr.SoundPlay("sfx","보스  등장음",bossAppears);
            return new int[] { 0, 0, 0, 0, 0, 100 };
        }
           

        // 중 ~ 후반부
        return new int[] { 10, 10, 30, 20, 30 , 0 };
    }
    private int GetWeightedRandomIndex(int[] weights)
    {
        int total = 0;
        for (int i = 0; i < weights.Length; i++)
            total += weights[i];

        int rand = Random.Range(0, total);
        int cumulative = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            cumulative += weights[i];
            if (rand < cumulative)
                return i;
        }

        return 0; // fallback
    }
}
