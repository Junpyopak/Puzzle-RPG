using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMgr : MonoBehaviour
{
    public PoolMgr poolMgr;            // PoolMgr 연결
    public Transform[] spawnPoints;    // 스폰 포인트 배열
    public float spawnInterval = 3f;   // 스폰 간격
    public int spawnCount = 3;         // 한 번에 스폰할 수
    public int maxMonsterCount = 10;   // 씬에 최대 몬스터 수

    private int nextSpawnIndex = 0;    // 다음 스폰 포인트 시작 인덱스



    [SerializeField] private float[] timeThresholds;    // 시간 경과별 활성 포인트 기준 (초)
    private int activeSpawnPointCount = 1;              // 현재 활성 포인트 수
    private float gameTimer = 0f;                       // 전체 게임 타이머


    private void Start()
    {
        if(timeThresholds.Length > 4)
            Debug.LogWarning("timeThresholds는 최대 4개까지만 사용하세요.");

        StartCoroutine(SpawnRoutine());
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
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnEnemies(spawnCount);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemies(int count)
    {
        int activeMonsterCount = GetActiveMonsterCount();
        if (activeMonsterCount >= maxMonsterCount)
            return; // 이미 충분하면 스폰하지 않음

        for (int i = 0; i < count; i++)
        {
            if (activeMonsterCount >= maxMonsterCount)
                break;

            int typeIndex = Random.Range(0, poolMgr.EnemyPrefabs.Length);
            GameObject enemy = poolMgr.GetEnemy(typeIndex);

            if (enemy != null)
            {
                // 반시계 방향 순환 + 활성 포인트 범위 내
                Transform point = spawnPoints[nextSpawnIndex % activeSpawnPointCount];
                enemy.transform.position = point.position;
                enemy.transform.rotation = point.rotation;

                nextSpawnIndex++;
                activeMonsterCount++;
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
}
