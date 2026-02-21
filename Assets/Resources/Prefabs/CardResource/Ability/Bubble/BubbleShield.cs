using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    public Vector2Int offsetDir;
    private PlayerMove1 playerMove;
    public float baseYOffset = 0f;
    private float damagePercent;
    Player playerStat;
    private float damageInterval = 0.3f; // 0.5초마다 데미지
    private Dictionary<Monster, float> lastDamageTime = new Dictionary<Monster, float>();
    //public void Init(PlayerMove1 player, Vector2Int dir , float percent)
    //{
    //    this.player = player;
    //    this.offsetDir = dir;
    //    damagePercent = percent;
    //    UpdatePosition();
    //}
    public void Init(PlayerMove1 move, Vector2Int dir, float percent)
    {
        playerMove = move;
        playerStat = move.GetComponent<Player>(); // ← Player 가져오기

        offsetDir = dir;
        damagePercent = percent;

        UpdatePosition();
    }

    void Update()
    {
        if (playerMove == null)
        {
            Destroy(gameObject);
            return;
        }

        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector2 cellSize = Grid15x15.Instance.cellWorldSize;
        int gridCount = Grid15x15.Instance.gridCount;

        float half = (gridCount - 1) / 2f;

        Vector2Int pos = playerMove.gridPos + offsetDir;

        float x = (pos.x - half) * cellSize.x;

        // 핵심: 기본 위치 + 위쪽 offset 추가
        float y = (pos.y - half) * cellSize.y + cellSize.y * baseYOffset;

        transform.position = new Vector3(x, y, playerMove.transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                //int damage = Mathf.RoundToInt(playerStat.PlayerATK * damagePercent);
                int damage = Mathf.Max(1, Mathf.RoundToInt(playerStat.PlayerATK * damagePercent));
                monster.TakeDamageFromBubble(damage);
                Debug.Log("몬스터 버블 데미지 받음");
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
            {
                float currentTime = Time.time;
                if (!lastDamageTime.ContainsKey(monster))
                    lastDamageTime[monster] = -damageInterval;

                if (currentTime - lastDamageTime[monster] >= damageInterval)
                {
                    int damage = Mathf.Max(1, Mathf.RoundToInt(playerStat.PlayerATK * damagePercent));
                    monster.TakeDamageFromBubble(damage);
                    lastDamageTime[monster] = currentTime;
                    Debug.Log("몬스터 버블 지속 데미지 받음");
                }
            }
        }
    }
}
