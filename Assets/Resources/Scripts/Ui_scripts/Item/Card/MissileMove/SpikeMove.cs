using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeMove : MonoBehaviour
{
    private Camera cam;
    public float padding = 0.5f;
    public Vector3 velocity = new Vector3(1f, 1f, 0f); // 이동 속도
    public int BoundCount = 1;
    private PlayerCardManager cardManager;
    private Player player;
    private int damage;
    private void Start()
    {
        cam = Camera.main;
        cardManager = FindObjectOfType<PlayerCardManager>();
        player = FindObjectOfType<Player>();
    }
    public void SetDamage(int dmg)
    {
        damage = dmg;
    }
    void Update()
    {
        float z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        Rect vr = cam.rect;

        Vector3 min = cam.ViewportToWorldPoint(new Vector3(vr.xMin, vr.yMin, z));
        Vector3 max = cam.ViewportToWorldPoint(new Vector3(1f, vr.yMax, z));

        Vector3 pos = transform.position;

        // 항상 이동
        pos += velocity * Time.deltaTime;

        // x축 충돌
        if (pos.x < min.x + padding)
        {
            pos.x = min.x + padding;
            if (BoundCount > 0)
            {
                velocity.x *= -1;
                BoundCount--;
            }
            else
            {
                //velocity = Vector3.zero; // 튕김 횟수 다 되면 멈춤
                Destroy(gameObject);
                return;
            }
        }
        else if (pos.x > max.x - padding)
        {
            pos.x = max.x - padding;
            if (BoundCount > 0)
            {
                velocity.x *= -1;
                BoundCount--;
            }
            else
            {
                //velocity = Vector3.zero;
                Destroy(gameObject);
                return;
            }
        }

        // y축 충돌
        if (pos.y < min.y + padding)
        {
            pos.y = min.y + padding;
            if (BoundCount > 0)
            {
                velocity.y *= -1;
                BoundCount--;
            }
            else
            {
                //velocity = Vector3.zero;
                Destroy(gameObject);
                return;
            }
        }
        else if (pos.y > max.y - padding)
        {
            pos.y = max.y - padding;
            if (BoundCount > 0)
            {
                velocity.y *= -1;
                BoundCount--;
            }
            else
            {
                //velocity = Vector3.zero;
                Destroy(gameObject);
                return;
            }
        }

        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Monster monster = collision.GetComponent<Monster>();
        if (monster != null && player != null)
        {
            // PlayerATK 실시간 참조
            //int damage = Mathf.Max(1, Mathf.RoundToInt(player.PlayerATK));
            //monster.TakeDamageFromPlayer(damage);
            int finalDamage = Mathf.Max(1, damage);
            monster.TakeDamageFromPlayer(finalDamage);
            // 2. 날카로운 검 출혈 적용
            if (player.hasBloodDamage)
            {
                if (Random.value <= player.bloodDamageChance / 100f) // 확률 체크
                {
                    // 몬스터에 Bleed 상태가 없으면 새로 생성
                    if (monster.bleed == null) monster.bleed = new Monster.BleedStatus();

                    monster.bleed.damagePerTurn = player.bloodDamagePerTick;
                    monster.bleed.remainingTurns = player.bloodDamageTurns;
                    monster.bleed.chance = player.bloodDamageChance / 100f;

                    Debug.Log($"{monster.name}에게 출혈 적용! {player.bloodDamagePerTick} 데미지, {player.bloodDamageTurns}턴");
                }
            }
        }
    }
}
