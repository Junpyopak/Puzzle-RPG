using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileMove : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
   // private int damage = 1;
    private Player player; // 플레이어 참조
    private PuzzleBoard board;
    // 발사 방향과 속도 설정
    public void SetDirection(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
    }
    void Start()
    {
        player = FindObjectOfType<Player>();
        board = FindObjectOfType<PuzzleBoard>();
    }
    void Update()
    {
        // 직선 이동, 회전은 발사 시점에서만 적용
        transform.position += (Vector3)direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Monster monster = collision.GetComponent<Monster>();
        ////if (monster != null && player != null)
        ////{
        ////    int damage = player.PlayerATK;// 플레이어 현재 ATK 읽기
        ////    monster.TakeDamageFromPlayer(damage);
        ////    Destroy(gameObject); // 충돌 후 미사일 제거
        ////}
        //if (monster != null)
        //{
        //    // 씬에 있는 Player 인스펙터 값을 직접 읽음
        //    Player player = FindObjectOfType<Player>();
        //    if (player != null)
        //    {
        //        Debug.Log("Missile firing damage: " + player.PlayerATK);

        //        int damage = Mathf.Max(1, player.PlayerATK); // 최소 데미지 1
        //        monster.TakeDamageFromPlayer(damage);
        //        Debug.Log($"Missile Damage: {damage} / PlayerATK: {player.PlayerATK}");
        //    }

        //    Destroy(gameObject); // 충돌 후 미사일 제거
        //}
        Monster monster = collision.GetComponent<Monster>();
        if (monster != null && player != null)
        {
            // PlayerATK 실시간 참조
            int damage = Mathf.Max(1, Mathf.RoundToInt(player.PlayerATK));
            monster.TakeDamageFromPlayer(damage);

            Destroy(gameObject);
        }
    }
}
