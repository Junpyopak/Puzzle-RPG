using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneSward : MonoBehaviour
{
    public float speed = 5f;         // 검 이동 속도
    private Vector2 direction;       // 이동 방향
    private Camera mainCam;
    private Rigidbody2D rb;
    private Player playerSc;
    private int damage;
    void Awake()
    {
        mainCam = Camera.main;
        playerSc =FindObjectOfType<Player>();
        rb = GetComponent<Rigidbody2D>();
    }

    // 방향 세팅
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // 스프라이트가 오른쪽(→)을 바라보도록 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Rigidbody 속도 적용
        if (rb != null)
            rb.velocity = direction * speed;  // 여기만 사용
    }
    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
    void Update()
    {
        // 화면 밖이면 삭제
        if (!IsVisible())
        {
            Destroy(gameObject);
        }
    }
    private bool IsVisible()
    {
        if (mainCam == null) return false;

        Vector3 viewPos = mainCam.WorldToViewportPoint(transform.position);
        return viewPos.x >= 0f && viewPos.x <= 1f && viewPos.y >= 0f && viewPos.y <= 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Monster monster = collision.GetComponent<Monster>();
            if (monster != null && playerSc != null)
            {
                // PlayerATK 실시간 참조
                //int damage = Mathf.Max(1, Mathf.RoundToInt(playerSc.PlayerATK));
                //monster.TakeDamageFromPlayer(damage);
                int finalDamage = Mathf.Max(1, damage);
                monster.TakeDamageFromPlayer(finalDamage);
                // 2. 날카로운 검 출혈 적용
                if (playerSc.hasBloodDamage)
                {
                    if (Random.value <= playerSc.bloodDamageChance / 100f) // 확률 체크
                    {
                        // 몬스터에 Bleed 상태가 없으면 새로 생성
                        if (monster.bleed == null) monster.bleed = new Monster.BleedStatus();

                        monster.bleed.damagePerTurn = playerSc.bloodDamagePerTick;
                        monster.bleed.remainingTurns = playerSc.bloodDamageTurns;
                        monster.bleed.chance = playerSc.bloodDamageChance / 100f;

                        Debug.Log($"{monster.name}에게 출혈 적용! {playerSc.bloodDamagePerTick} 데미지, {playerSc.bloodDamageTurns}턴");
                    }
                }
            }
        }
    }
}
