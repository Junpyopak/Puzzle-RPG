using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int Damage;
    private Monster attacker;
    // 발사 방향과 속도 설정
    public void SetDirection(Vector2 dir, float spd, int Atk, Monster attacker)
    {
        direction = dir.normalized;
        speed = spd;
        Damage = Atk;
        this.attacker = attacker;
    }

    void Update()
    {
        // 직선 이동, 회전은 발사 시점에서만 적용
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            
            if (player != null)
            {
                player.TakeDamage(Damage, attacker); // ← Counter 발동 핵심
            }
            Destroy(this.gameObject);
        }
    }
}
