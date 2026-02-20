using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    Animator anim;
    public Camera cam;
    public float padding = 0.5f;
    private SpriteRenderer sr;

    [Header("데미지 관련")]
    public float flashAlpha = 0.3f;      // 깜빡일 때 알파
    public float fadeSpeed = 1f;
    public float flashDuration = 0.25f;   // 반짝임 유지 시간
    public Color flashColor = Color.red; // 깜빡일 색
    private float originalAlpha;
    private Color originalColor;
    private PlayerCardManager cardManager;
    [Header("타겟 관련")]
    public string enemyTag = "Enemy";   // 적 태그
    public float detectionDis = 10f;    // 탐지 거리
    public GameObject NearTarget;
    public GameObject MissilePrefab;
    public Transform AttackBox;
    public float MissileSpeed = 5f;

    public int Hp = 10;
    public int MaxHp = 10;
    public int Exp = 0;
    public int PlayerLevel = 1;
    public int PlayerATK = 1;
    private int baseATK;        // 기본 공격력 저장
    public int Defence = 0;

    [Header("플레이어 사망")]
    public bool isDie = false;
    private PuzzleBoard board;
    public GameObject defeatUI;

    [Header("레벨 시스템")]
    public int NeedExp = 5;

    [Header("레벨업 증가량")]
    public int HpPerLevel = 5;
    public int AtkPerLevel = 1;
    public int DefPerLevel = 0;

    [Header("회복 관련")]
    public float HealAlpha = 0.3f;      // 깜빡일 때 알파
    public float HealfadeSpeed = 1f;
    public float HealflashDuration = 0.25f;   // 반짝임 유지 시간
    public Color HealflashColor = Color.green; // 깜빡일 색

    [Header("이어하기 위치 조장")]
    private bool isClamp = false;

    [Header("부메랑 설정")]
    public GameObject boomerangPrefab;
    public int boomerangLevel = 0;

    [Header("은근슬쩍 회복 설정")]
    public bool Recovery = false;
    public int recoveryAmount = 0;
    public int recoveryTurnInterval = 2;
    public int recoveryTurnCounter = 0;

    [Header("패시브 카운터 설정")]
    public bool PassiveCounter = false;

    [Header("스파이크볼 설정")]
    public GameObject spikeballPrefab;
    public bool hasSpikeball = false;

    [Header("응급 설정")]
    public bool emergency = false;
    public float emergencyThresholdPercent = 0.05f; // 5% 이하일 때 발동
    public bool emergencyUsed = false;

    [Header("부활 설정")]
    public bool Revival = false;
    public bool revivalUsed = false; 
    
    [Header("럭키 설정")]
    public float LuckyChance = 0f;

    [Header("분노 설정")]
    public bool Anger = false;
    public int AngeAmount = 0;
    public int AngerTurnInterval = 3;
    public int AngerTurnCounter = 0;
    
    [Header("복수 설정")]
    public bool revenge = false;
    public float revengeAmount = 0;
    public int revengeTurnInterval = 3;
    public int revengeTurnCounter = 0;
    private int revengeHitThisTurn = 0; // 턴 동안 맞은 횟수
    public bool revengeActiveThisTurn = false;
    
    [Header("아이템 자석 설정")]
    public float pickupDistanceBonus = 0f; 

    [Header("경험치 보너스설정")]
    public int bonusExpIncrease = 0;

    [Header("날카로운 검 출혈 설정")]
    public bool hasBloodDamage = false;
    public float bloodDamageChance = 0f;  // 발동 확률 %
    public int bloodDamagePerTick = 1;     // 틱당 데미지
    public int bloodDamageTurns = 3;       // 지속 턴 수

    // Start is called before the first frame update

    private void Awake()
    {

        Debug.Log("PlayerATK at Awake: " + PlayerATK);
    }
    public void SetTemporaryATK(int value)
    {
        PlayerATK = value; // 최소 보장 없이 바로 적용
    }
    void Start()
    {
        baseATK = PlayerATK; // 초기 기본 공격력 저장
        recoveryTurnCounter = 0;
        Hp = MaxHp;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        originalAlpha = sr.color.a;
        originalColor = sr.color;
        GameManager.Instance.CardMgr.CardRarityOpen();
        cardManager = FindObjectOfType<PlayerCardManager>();
        board = FindObjectOfType<PuzzleBoard>();

    }
    void LateUpdate()
    {
        if (!isClamp) return;
        //플레이어가 화면 밖으로 못나가도록
        float z = Mathf.Abs(cam.transform.position.z - transform.position.z);

        Rect vr = cam.rect;

        //왼쪽·아래 경계 (Viewport 기준)
        Vector3 min = cam.ViewportToWorldPoint(
            new Vector3(vr.xMin, vr.yMin, z)
        );
        //오른쪽·위 경계 (전체 시야 기준)
        Vector3 max = cam.ViewportToWorldPoint(
            new Vector3(1f, vr.yMax, z)
        );

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, min.x + padding, max.x - padding);
        pos.y = Mathf.Clamp(pos.y, min.y + padding, max.y - padding);
        pos.z = transform.position.z;

        transform.position = pos;
    }
    // Update is called once per frame
    void Update()
    {
        //Attack();
        // 1. 발사 전에 현재 카드 인벤토리 상황을 내 변수에 동기화
        UpdateBoomerangLevel();

        //if (Input.GetMouseButtonDown(0) && !GameManager.Instance.CardMgr.isOpen)
        //{
        //    ShootBoomerangs();
        //}
    }
    public void Attack()
    {
        //v 누르면 공격 나가기
        //if (Input.GetKeyDown(KeyCode.V))
        //{        
        //    AutoTarget();
        //    anim.SetTrigger("Attack");
        //}
        //턴종료 버튼 누르면 나가기
        AutoTarget();
        anim.SetTrigger("Attack");
    }

    void AutoTarget()
    {
        //NearTarget = FindNearestEnemy();
        ////Missile.transform.position = NearTarget.transform.position;
        //if (NearTarget == null || !NearTarget.activeInHierarchy)
        //{
        //    Vector3 targetPos = NearTarget.transform.position;
        //    // 미사일 생성
        //    GameObject missile = Instantiate(MissilePrefab, AttackBox.position, Quaternion.identity);

        //    Vector3 direction = (targetPos - AttackBox.position).normalized;
        //    Vector2 dir = (targetPos - AttackBox.position).normalized;
        //    // Sprite가 위를 바라보는 경우 회전 조정
        //    // 위쪽 Sprite 기준 → Z축 각도 = atan2(y, x) - 90
        //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        //    missile.transform.rotation = Quaternion.Euler(0, 0, angle);

        //    // 미사일 이동 스크립트에 방향 전달
        //    MissileMove mm = missile.GetComponent<MissileMove>();
        //    if (mm != null)
        //        mm.SetDirection(dir, MissileSpeed);

        //}
        GameObject target = FindNearestEnemy();
        if (target == null || !target.activeInHierarchy) return;

        // 미사일 생성
        GameObject missile = Instantiate(MissilePrefab, AttackBox.position, Quaternion.identity);

        // 타겟 방향 계산
        Vector2 dir = (target.transform.position - AttackBox.position).normalized;

        // Sprite가 위를 바라보는 경우 회전 조정
        // 위쪽 Sprite 기준 → Z축 각도 = atan2(y, x) - 90
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        missile.transform.rotation = Quaternion.Euler(0, 0, angle);

        // 미사일 이동 스크립트에 방향 전달
        MissileMove mm = missile.GetComponent<MissileMove>();
        if (mm != null)
        mm.SetDirection(dir, MissileSpeed);

    }
    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearest = null;
        float minDist = detectionDis;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy;
            }
        }

        return nearest;
    }

    //public void ApplySaveData(SaveData data)
    //{
    //    transform.position = data.playerPosition;
    //    Hp = data.playerHp;
    //}
    public void ApplySaveData(SaveData data)
    {
        //transform.position = data.playerPosition;

        //if (data.playerStats != null)
        //{
        //    ApplyStats(data.playerStats);
        //    Debug.Log($"[Load] NeedExp: {NeedExp}, Exp: {Exp}, Level: {PlayerLevel}");
        //}
        StartCoroutine(SetPositonFrame(data));
    }
    public void ApplyStats(PlayerStatSaveData stats)
    {
        PlayerLevel = stats.level;
        Exp = stats.exp;
        NeedExp = stats.needExp;

        MaxHp = stats.maxHp;
        Hp = stats.currentHp;

        PlayerATK = stats.attack;
        Defence = stats.defense;
        //Recovery 이어하기 시에 복구
        Recovery = stats.recovery;
        recoveryAmount = stats.recoveryAmount;
        recoveryTurnInterval = stats.recoveryTurnInterval;
        recoveryTurnCounter = stats.recoveryTurnCounter;
        PlayerMove1 move = GetComponent<PlayerMove1>();

        move.moveCount = stats.moveCount;
        move.moveRemain = stats.moveRemain;
        //move.ResetMove();
    }
    private IEnumerator SetPositonFrame(SaveData data)
    {
        isClamp = true;
        yield return null;
        transform.position = data.playerPosition;
        // gridPos 복구
        PlayerMove1 move = GetComponent<PlayerMove1>();
        if (move != null)
        {
            move.LoadGridPosition(new Vector2Int(data.playerGridX, data.playerGridY));
        }

        if (data.playerStats != null)
        {
            ApplyStats(data.playerStats);
            Debug.Log($"[Load] NeedExp: {NeedExp}, Exp: {Exp}, Level: {PlayerLevel}");
        }
        isClamp = false;
    }
    public void SaveGame()
    {
        int slot = SaveContext.Instance.currentSlot;
        if (slot < 0) return;

        SaveData data = new SaveData();
        PuzzleBoard board = FindObjectOfType<PuzzleBoard>();
        Turn_Timer turn = FindObjectOfType<Turn_Timer>();
        if (board != null)
        {
            data.puzzleData = board.GetSaveData();
        }
        PlayerMove1 playerMove = GetComponent<PlayerMove1>();
        data.playerPosition = transform.position;
        data.playerGridX = playerMove.GridPos.x;
        data.playerGridY = playerMove.GridPos.y;
        data.playerMoveCount = playerMove.moveCount;
        //data.playerHp = Hp;
        //data.playerExp = Exp;
        data.currentScene = SceneManager.GetActiveScene().name;
        data.playerStats = new PlayerStatSaveData
        {
            level = PlayerLevel,
            exp = Exp,
            needExp = NeedExp,
            maxHp = MaxHp,
            currentHp = Hp,
            attack = PlayerATK,
            defense = Defence,
            recovery = Recovery,
            recoveryAmount = recoveryAmount,
            recoveryTurnInterval = recoveryTurnInterval,
            recoveryTurnCounter = recoveryTurnCounter,

        };
        int currentTurn = 1;
        if (turn != null)
        {
            currentTurn = turn.TurnCount;
        }
        data.TurnCount = currentTurn;
        // Save 직전에 로그
        Debug.Log($"[Save] NeedExp: {NeedExp}, Exp: {Exp}, Level: {PlayerLevel}");
        SaveManager.Save(slot, data);
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }
    public void AddExp(int amount)
    {
        Exp += amount;

        while (Exp >= NeedExp)
        {
            Exp -= NeedExp;
            LevelUp();
        }
    }

    public void HealHp(int amount)
    {
        Hp += amount;
        StartCoroutine(HealFlashCoroutine(1f));
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }
    //은근슬쩍 회복
    public void OnTurnEndRecovery()
    {
        if (!Recovery) return;

        recoveryTurnCounter++;

        if (recoveryTurnCounter >= recoveryTurnInterval)
        {
            HealHp(recoveryAmount);
            recoveryTurnCounter = 0;

            Debug.Log($"은근슬쩍 회복 발동 +{recoveryAmount}");
        }
    }

    // 분노 효과 - 턴 종료 시 발동 및 원복
    public void OnTurnEndAnger()
    {
        if (!Anger) return;

        AngerTurnCounter++;

        if (AngerTurnCounter >= AngerTurnInterval)
        {
            AngerTurnCounter = 0;

            // 분노 효과 적용 (턴 동안만)
            PlayerATK = Mathf.RoundToInt(baseATK * (1f + AngeAmount));
            Debug.Log($"분노효과 발동 +{AngeAmount * 100}%");

            // 바로 원복 처리 - 다음 턴으로 넘어가면 공격력 원래대로
            // (혹은 원하면 Update나 턴 종료 이벤트에서 다시 원복)
            PlayerATK = baseATK;
            Debug.Log("분노효과 종료, 공격력 원복");
        }
    }

    // 복수 효과 - 턴 종료 시 발동 및 원복
    public void OnTurnEndRevenge()
    {
        if (!revenge) return;

        revengeTurnCounter++;

        if (revengeTurnCounter >= revengeTurnInterval)
        {
            revengeTurnCounter = 0;

            // 분노 효과 적용 (턴 동안만)
            PlayerATK = Mathf.RoundToInt(baseATK * (1f + revengeAmount));
            Debug.Log($"복수 효과 발동 +{revengeAmount * 100}%");

            // 바로 원복 처리 - 다음 턴으로 넘어가면 공격력 원래대로
            // (혹은 원하면 Update나 턴 종료 이벤트에서 다시 원복)
            PlayerATK = baseATK;
            Debug.Log("복수 효과 종료, 공격력 원복");
        }
    }
    void LevelUp() //플레이어 레벨업
    {
        PlayerLevel++; //현재 플레이어 레벨 증가

        Hp += HpPerLevel;
        MaxHp = Hp;
        PlayerATK += AtkPerLevel;

        if (PlayerLevel < 3)
        {
            // 다음 레벨 필요 경험치 증가
            NeedExp *= 2;
        }
        else
        {
            NeedExp += 25;
        }
        GameManager.Instance.CardMgr.CardRarityOpen();
        Debug.Log($"레벨업! Lv.{PlayerLevel} / HP:{Hp} ATK:{PlayerATK} DEF:{Defence}");
        // 아래다가 레벨업시 나오는 카드 함수

        // 레벨업 후 바로 저장
        SaveGame();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                Debug.Log("플레이어 가 데미지 받음");
                StartCoroutine(FlashCoroutine(1f));
                int damage = monster.GetAttack();
                TakeDamage(damage, monster);
            }
        }
    }
    //럭키
    public void AddTemporaryDodge(float chance)
    {
        LuckyChance += chance;
        LuckyChance = Mathf.Clamp01(LuckyChance); // 최대 100%
    }
    public void TakeDamage(int damage, Monster attacker)
    {
        // Lucky 회피 확률 체크
        if (LuckyChance > 0f && Random.value < LuckyChance)
        {
            Debug.Log("럭키 회피 성공! 데미지 무시");
            return; // 데미지 무시
        }
        Hp -= damage;
        Debug.Log($"플레이어 데미지 {damage} / 현재 HP : {Hp}");


        // 복수 효과 발동
        if (revenge && !revengeActiveThisTurn)
        {
            revengeHitThisTurn++;

            if (revengeHitThisTurn % revengeTurnInterval == 0)
            {
                PlayerATK = Mathf.RoundToInt(baseATK * (1f + revengeAmount));
                revengeActiveThisTurn = true;
                Debug.Log($"복수 효과 발동! 맞은 횟수 {revengeHitThisTurn} → 공격력 +{revengeAmount * 100}%");
            }
        }

        if (PassiveCounter && attacker != null)
        {
            attacker.TakeDamageFromBubble(PlayerATK);

            Debug.Log($"카운터 발동 → {PlayerATK} 데미지 반사");
        }
        // 응급 회복 (한 번만 발동)
        if (emergency && !emergencyUsed)
        {
            int thresholdHp = Mathf.CeilToInt(MaxHp * emergencyThresholdPercent);

            if (Hp <= thresholdHp)
            {
                Hp = MaxHp;
                emergencyUsed = true;
                Debug.Log("응급 회복 발동 → HP 전체 회복");
                return;
            }
        }

        if (Hp <= 0)
        {
            if (Revival && !revivalUsed)
            {
                Hp = MaxHp;
                revivalUsed = true;
                Debug.Log("부활 발동 → 최대 체력으로 부활");

                return; //죽지 않게 여기서 종료
            }
            Hp = 0;
            Debug.Log("플레이어 사망");
            Die();
        }
    }

    private void Die()
    {
        if (isDie) return;
        anim.SetBool("PlayerDie", true);
        if (board != null)
        {
            board.isPlayerDead = true;
            board.isPlayerTurn = false;
        }
        // 이동 막기
        GetComponent<PlayerMove1>().enabled = false;
        // 콜라이더 막기 (선택)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;
        // 필요하면 Rigidbody 정지
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;
        Debug.Log("플레이어 완전히 사망");
    }

    public void ShowDefeatUI()
    {
        defeatUI.SetActive(true);
    }

    private IEnumerator FlashCoroutine(float totalTime)
    {
        float timer = 0f;

        while (timer < totalTime)
        {
            // 빨강으로 변경
            sr.color = flashColor;
            yield return new WaitForSeconds(flashDuration);

            // 원래 색으로 복원
            sr.color = originalColor;
            yield return new WaitForSeconds(flashDuration);

            // 경과 시간 증가
            timer += flashDuration * 2f; // 빨강+원래색
        }

        // 마지막에 원래 색 확실히 복원
        sr.color = originalColor;
    }

    private IEnumerator HealFlashCoroutine(float totalTime)
    {
        float timer = 0f;

        while (timer < totalTime)
        {
            sr.color = HealflashColor;
            yield return new WaitForSeconds(HealflashDuration);

            // 원래 색으로 복원
            sr.color = originalColor;
            yield return new WaitForSeconds(HealflashDuration);

            // 경과 시간 증가
            timer += HealflashDuration * 2f; // 빨강+원래색
        }

        // 마지막에 원래 색 확실히 복원
        sr.color = originalColor;
    }

    private Vector2[] shotDirections = new Vector2[]
    {
        Vector2.up,                          // 1: 상
        Vector2.left,                        // 2: 좌
        Vector2.right,                       // 3: 우
        new Vector2(-1, -1).normalized,      // 4: 좌하
        new Vector2(1, -1).normalized        // 5: 우하
    };



    public void UpdateBoomerangLevel()
    {
        if (GameManager.Instance.CardMgr == null) return;

        // CardManager의 리스트에서 내 부메랑 카드 개수를 세서 변수에 저장
        // 이렇게 하면 인스펙터에서도 숫자가 올라가는 게 보입니다.
        boomerangLevel = GameManager.Instance.CardMgr.selectCardNames.Count(name => name == "부메랑");
    }

    public void ShootBoomerangs()
    {
        Debug.Log("ShootBoomerangs called");
        if (boomerangPrefab == null)
        {
            Debug.Log("부메랑이 없습니다");
            return;
        }
        Debug.Log("부메랑 발사");
        // 2. 이제 헷갈릴 것 없이 무조건 내 변수(boomerangLevel)만 봅니다.
        if (boomerangLevel <= 0) return;

        int count = Mathf.Min(boomerangLevel, shotDirections.Length);

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(boomerangPrefab, transform.position, Quaternion.identity);
            Boomerang bScript = go.GetComponent<Boomerang>();

            if (bScript != null)
            { 
                bScript.Shot(shotDirections[i], this.transform);
            }
        }
    }

    public void ShootSpikeBall()
    {
        if (hasSpikeball == false) return;
        if (spikeballPrefab == null) return;
        GameObject ball = Instantiate(spikeballPrefab, transform.position, Quaternion.identity);

        SpikeMove spike = ball.GetComponent<SpikeMove>();

        PlayerCard boundCard = cardManager.ownedCards
            .Find(c => c.data.effectType == CardEffectType.BoundUp);

        if (boundCard != null)
        {
            spike.BoundCount = boundCard.level;
        }
    }
}
