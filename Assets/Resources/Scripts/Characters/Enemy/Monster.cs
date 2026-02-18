using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int monsterID;
    public int enemyTypeIndex;//Pool 타입 인덱스
    private MonsterData data;
    bool loadedFromSave = false;
    PoolMgr poolMgr;
    [Header("Damage")]
    private SpriteRenderer sr;
    public float flashAlpha = 0.7f;      // 깜빡일 때 알파
    public float fadeSpeed = 5f;
    public float flashDuration = 0.1f;   // 반짝임 유지 시간
    public Color flashColor = Color.red; // 깜빡일 색
    private float originalAlpha;
    private Color originalColor;
    public int Hp;

    [Header("Grid Move")]
    public Vector2Int gridPos;
    int moveRemain;
    PlayerMove1 playerTpos;
    Player player;
    bool isInitialized = false;

    [Header("MissileAttack")]
    public GameObject EnemyMissilePre;
    public float EnemyMissileSpeed = 3f;
    public Transform firePoint;

    [Header("Exp Prefabs")]
    public GameObject expNormalPrefab;
    public GameObject expAlphaPrefab;
    public GameObject expSuperPrefab;

    [Header("food Prefabs")]
    public GameObject foodPrefab;
    void Start()
    {
        poolMgr = FindObjectOfType<PoolMgr>();

        //TurnManager.Instance.monsters.Add(this);
        data = MonsterDataTable.Instance.monsterDic[monsterID];
        playerTpos = FindObjectOfType<PlayerMove1>();
        player = FindObjectOfType<Player>();
        Debug.Log($"{data.Name} 생성 / 타입 : {data.MonsterType} / 공격력 : {data.Atk} / 체력 :  {data.Hp} / 턴수 {data.RemainTurn}");
        sr = GetComponent<SpriteRenderer>();
        originalAlpha = sr.color.a;
        originalColor = sr.color;
        //Hp = data.Hp;
        //if (SaveContext.Instance != null && SaveContext.Instance.isLoading)
        //    return;
        //if (SaveContext.Instance == null || !SaveContext.Instance.isLoading)
        //{
        //    Hp = data.Hp;   // 새 게임일 때만 풀피
        //}
        if (!loadedFromSave)
        {
            Hp = data.Hp;
        }
        StartCoroutine(InitAfterGridReady());
    }


    //전투
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("PlayerAttack")) return;
        Damage();
        Debug.Log("데미지 받음.");
    }
    void Damage()
    {
        Hp -= player.PlayerATK;
        StartCoroutine(FlashCoroutine());
        if (Hp <= 0)
        {
            Hp = 0;
            Die();
        }
    }
    public void TakeDamageFromBubble(int damage)
    {
        Hp -= damage;

        // 깜빡임 효과
        StartCoroutine(FlashCoroutine());

        if (Hp <= 0)
        {
            Hp = 0;
            Die();
        }

        Debug.Log($"버블 피해: {damage}, 남은 HP: {Hp}");
    }
    void Die()
    {
        Debug.Log($"{data.Name} 사망!");
        // 경험치 드랍
        OnMonsterDead();

        if (poolMgr != null)
        {
            poolMgr.ReturnEnemy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    private IEnumerator FlashCoroutine()//데미지 입었을때 빨간색 버전 
    {
        // 1. 빨강으로 변경
        sr.color = flashColor;

        // 2. 잠시 대기
        yield return new WaitForSeconds(flashDuration);

        // 3. 원래 색으로 복원
        sr.color = originalColor;
    }

    public int GetAttack()
    {
        return data.Atk;
    }
    //칸 이동 + 플레이어 추적
    public void StartTurn()
    {
        if (!isInitialized) return;
        moveRemain = data.RemainTurn;
        Debug.Log($"[몬스터 턴 시작] {data.Name} | 이동 가능:{moveRemain} | 현재위치:{gridPos}");
    }

    //return true = 이 몬스터 턴 종료
    public bool Act()
    {
        if (!isInitialized) return true;
        if (moveRemain <= 0)
            return true;
        //플레이어가 공격거리 내에 들어오면 공격후 턴 종료
        if (PlayerAttackRange())
        {
            AttackPlayer();
            moveRemain = 0;
            return true; // 공격하면 이 몬스터 턴 종료
        }

        MoveTowardPlayer();
        moveRemain--;
        Debug.Log($"[몬스터 이동] {data.Name} | 남은 이동:{moveRemain}");
        return moveRemain <= 0;
    }

    void MoveTowardPlayer()
    {
        InitGridPosFromWorld();
        Vector2Int playerPos = playerTpos.GridPos;
        Vector2Int dir = playerPos - gridPos;

        int x = Mathf.Clamp(dir.x, -1, 1);
        int y = Mathf.Clamp(dir.y, -1, 1);
        Debug.Log($"[추적 계산] {data.Name} | 플레이어:{playerPos} | 방향:({x},{y})");
        gridPos += new Vector2Int(x, y);

        SnapToCell();
    }

    void SnapToCell()
    {
        Vector2 cellSize = Grid15x15.Instance.cellWorldSize;
        int gridCount = Grid15x15.Instance.gridCount;
        float half = (gridCount - 1) / 2f;

        float x = (gridPos.x - half) * cellSize.x;
        float y = (gridPos.y - half) * cellSize.y;

        transform.position = new Vector3(Mathf.Round(x * 100f) / 100f, Mathf.Round(y * 100f) / 100f, transform.position.z); // 부동소수점 오차 방지
    }

    ////죽었을때 턴메니저 리스트에서 삭제
    //void OnDestroy()
    //{
    //    if (TurnManager.Instance != null)
    //        TurnManager.Instance.monsters.Remove(this);
    //}
    //폴링을 사용하여 재사용할때 오브젝트가 꺼졌으면 턴메니저에서 제거.
    void OnDisable()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.monsters.Remove(this);
    }

    //몬스터가 폴링으로 재생성시 턴메니저에 추가하고 hp 값 초기화 하기 위해
    void OnEnable()
    {
        //if (TurnManager.Instance != null)
        //    TurnManager.Instance.monsters.Add(this);

        //if (data != null)
        //{
        //    Hp = data.Hp;
        //    isInitialized = false;
        //    StartCoroutine(InitAfterGridReady());
        //}
        if (TurnManager.Instance != null)
            TurnManager.Instance.monsters.Add(this);

        if (SaveContext.Instance != null && SaveContext.Instance.isLoading)
            return;

        //if (data != null)
        //{
        //    //Hp = data.Hp;
        //    if (SaveContext.Instance == null || !SaveContext.Instance.isLoading)
        //    {
        //        Hp = data.Hp;   //새 게임일 때만 풀피
        //    }
        //    isInitialized = false;
        //    StartCoroutine(InitAfterGridReady());
        //}
        if (data != null && !loadedFromSave)
        {
            Hp = data.Hp;
            isInitialized = false;
            StartCoroutine(InitAfterGridReady());
        }
    }
    IEnumerator InitAfterGridReady()
    {
        // Grid 생성될 때까지 대기
        yield return new WaitForEndOfFrame();

        InitGridPosFromWorld();
        SnapToCell();
        isInitialized = true;
        Debug.Log($"{data.Name} gridPos 초기화 완료 : {gridPos}");
    }

    void InitGridPosFromWorld()
    {
        Vector2 cellSize = Grid15x15.Instance.cellWorldSize;
        int gridCount = Grid15x15.Instance.gridCount;
        float half = (gridCount - 1) / 2f;

        float wx = transform.position.x;
        float wy = transform.position.y;

        int gx = Mathf.RoundToInt(wx / cellSize.x + half);
        int gy = Mathf.RoundToInt(wy / cellSize.y + half);

        gx = Mathf.Clamp(gx, 0, gridCount - 1);
        gy = Mathf.Clamp(gy, 0, gridCount - 1);

        gridPos = new Vector2Int(gx, gy);
    }

    ///몬스터 공격 기능
    bool PlayerAttackRange()
    {
        Vector2Int playerPos = playerTpos.GridPos;
        Vector2Int diff = playerPos - gridPos;

        int distX = Mathf.Abs(diff.x);
        int distY = Mathf.Abs(diff.y);

        // 체스 King 이동 기준 (대각 포함)
        int chebyshevDist = Mathf.Max(distX, distY);

        return chebyshevDist <= data.AttackRange;
    }

    void AttackPlayer()
    {
        Debug.Log($"MonsterType 실제 값 : {data.MonsterType} ({(int)data.MonsterType})");
        Debug.Log($"[몬스터 공격] {data.Name} → 플레이어 공격!");

        if (data.MonsterType == MonsterType.원거리)
        {
            Debug.Log($"[몬스터 공격] {data.Name} → 플레이어 공격 투척!");
            ShootMissile();
        }
        else//근거리일때
        {
            player.TakeDamage(data.Atk, this);
        }

    }

    public void ShootMissile()
    {
        // 미사일 생성
        GameObject missile = Instantiate(EnemyMissilePre, firePoint.position, Quaternion.identity);

        Vector2 dir = (player.transform.position - firePoint.position).normalized;

        // Sprite가 위를 바라보는 경우 회전 조정
        // 위쪽 Sprite 기준 → Z축 각도 = atan2(y, x) - 90
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        missile.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 미사일 이동 스크립트에 방향 전달
        EnemyMissile mm = missile.GetComponent<EnemyMissile>();
        if (mm != null)
            mm.SetDirection(dir, EnemyMissileSpeed, data.Atk, this);
    }
    void OnMonsterDead()
    {
        ExpType expType = GetRandomExpType();

        GameObject prefab = GetExpPrefab(expType);
        if (prefab == null)
        {
            Debug.LogWarning($"Exp prefab missing for {expType}");
            return;
        }

        Instantiate(prefab, transform.position, Quaternion.identity);

        GameObject food = GetRandomFood();
        if (food != null)
        {
            Instantiate(food, transform.position, Quaternion.identity);
        }
    }

    // 몬스터 경험치 드랍 타입 결정
    public ExpType GetRandomExpType()
    {
        float total = data.ExpNormal + data.ExpAlpha + data.ExpSuper;

        float roll = Random.Range(0f, total);

        if (roll < data.ExpNormal)
            return ExpType.Normal;

        roll -= data.ExpNormal;

        if (roll < data.ExpAlpha)
            return ExpType.Alpha;

        return ExpType.Super;
    }

    GameObject GetExpPrefab(ExpType type)
    {
        switch (type)
        {
            case ExpType.Normal:
                return expNormalPrefab;
            case ExpType.Alpha:
                return expAlphaPrefab;
            case ExpType.Super:
                return expSuperPrefab;
            default:
                return null;
        }
    }

    // 몬스터 음식 드랍 결정
    public GameObject GetRandomFood()
    {

        float roll = Random.Range(0f, 100f);

        if (roll < data.FoodDrop)
        {
            Debug.Log("회복 음식 드랍.");
            return foodPrefab;
        }
     
        // 음식이 드롭되지 않으면 null 반환
        Debug.Log("음식 드랍 안됌.");
        return null;
    }

    public MonsterSaveData GetSaveData()
    {
        MonsterSaveData data = new MonsterSaveData();
        data.monsterID = monsterID;
        data.enemyTypeIndex = enemyTypeIndex;
        data.hp = Hp;
        data.gridX = gridPos.x;
        data.gridY = gridPos.y;
        return data;
    }

    // ================= 복원용 =================
    public void LoadFromSaveData(MonsterSaveData data)
    {
        StopAllCoroutines();

        loadedFromSave = true;

        monsterID = data.monsterID;
        enemyTypeIndex = data.enemyTypeIndex;
        Hp = data.hp;

        gridPos = new Vector2Int(data.gridX, data.gridY);

        StartCoroutine(ApplyLoadedPosition());
    }

    IEnumerator ApplyLoadedPosition()
    {
        // Grid 생성 대기
        yield return new WaitForEndOfFrame();

        SnapToCell();
        isInitialized = true;
    }
}

