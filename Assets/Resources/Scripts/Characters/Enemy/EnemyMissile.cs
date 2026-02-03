using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int Damage;
    // 발사 방향과 속도 설정
    public void SetDirection(Vector2 dir, float spd, int Atk)
    {
        direction = dir.normalized;
        speed = spd;
        Damage = Atk;
        
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
            Destroy(this.gameObject);
        }
    }
}
