using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    Animator anim;
    public Camera cam;
    public float padding = 0.5f;

    [Header("타겟 관련")]
    public string enemyTag = "Enemy";   // 적 태그
    public float detectionDis = 10f;    // 탐지 거리
    public GameObject NearTarget;
    public GameObject MissilePrefab;
    public Transform AttackBox;
    public float MissileSpeed = 5f;

    public int Hp = 10;
    public int Exp = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

    }
    void LateUpdate()
    {
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

    public void ApplySaveData(SaveData data)
    {
        transform.position = data.playerPosition;
        Hp = data.playerHp;
    }

    public void SaveGame()
    {
        int slot = SaveContext.Instance.currentSlot;
        if (slot < 0) return;

        SaveData data = new SaveData();
        data.playerPosition = transform.position;
        data.playerHp = Hp;
        data.playerExp = Exp;
        data.currentScene = SceneManager.GetActiveScene().name;

        SaveManager.Save(slot, data);
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }
}
