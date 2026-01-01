using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PoolMgr : MonoBehaviour
{
    public GameObject[] EnemyPrefabs;     // 풀링할 적들
    public int poolSize = 10;             // 각 종류당 풀 크기
    public List<GameObject>[] Pools;     // 풀 배열

    private void Awake()
    {
        // 풀 배열 초기화
        Pools = new List<GameObject>[EnemyPrefabs.Length];

        for (int i = 0; i < EnemyPrefabs.Length; i++)
        {
            Pools[i] = new List<GameObject>();
            for (int j = 0; j < poolSize; j++)
            {
                GameObject obj = Instantiate(EnemyPrefabs[i]);
                obj.SetActive(false);
                Pools[i].Add(obj);
            }
        }
    }

    // 사용 가능한 몬스터 가져오기
    public GameObject GetEnemy(int typeIndex)
    {
        List<GameObject> pool = Pools[typeIndex];
        foreach (var enemy in pool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        // 모두 사용 중이면 새로 생성
        GameObject obj = Instantiate(EnemyPrefabs[typeIndex]);
        pool.Add(obj);
        return obj;
    }

    // 몬스터 반환
    public void ReturnEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
    }
}

