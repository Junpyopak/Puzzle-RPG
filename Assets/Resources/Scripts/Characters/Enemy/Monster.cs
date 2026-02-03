using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int monsterID;
    private MonsterData data;

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
    void Start()
    {
        TurnManager.Instance.monsters.Add(this);
        data = MonsterDataTable.Instance.monsterDic[monsterID];
        playerTpos = FindObjectOfType<PlayerMove1>();
        player = FindObjectOfType<Player>();
        Debug.Log($"{data.Name} 생성 / 타입 : {data.MonsterType} / 공격력 : {data.Atk} / 체력 :  {data.Hp} / 턴수 {data.RemainTurn}");
        sr = GetComponent<SpriteRenderer>();
        originalAlpha = sr.color.a;
        originalColor = sr.color;
        Hp = data.Hp;
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

    //죽었을때 턴메니저 리스트에서 삭제
    void OnDestroy()
    {
        if (TurnManager.Instance != null)
            TurnManager.Instance.monsters.Remove(this);
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
            player.TakeDamage(data.Atk);
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
            mm.SetDirection(dir, EnemyMissileSpeed,data.Atk);
    }
}

