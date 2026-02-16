using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    public Vector2Int offsetDir;
    private PlayerMove1 player;
    public float baseYOffset = 0.85f;
    private float damagePercent;
    Player Player;
    public void Init(PlayerMove1 player, Vector2Int dir , float percent)
    {
        this.player = player;
        this.offsetDir = dir;
        damagePercent = percent;
        UpdatePosition();
    }

    void Update()
    {
        if (player == null)
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

        Vector2Int pos = player.gridPos + offsetDir;

        float x = (pos.x - half) * cellSize.x;

        // 핵심: 기본 위치 + 위쪽 offset 추가
        float y = (pos.y - half) * cellSize.y + cellSize.y * baseYOffset;

        transform.position = new Vector3(x, y, player.transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Monster monster = collision.GetComponent<Monster>();
        if (monster != null)
        {
            int damage = Mathf.RoundToInt(Player.PlayerATK * damagePercent);
            monster.TakeDamageFromBubble(damage);
        }
    }
}
