using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    public Vector2Int offsetDir;
    private PlayerMove1 player;

    public void Init(PlayerMove1 player, Vector2Int dir)
    {
        this.player = player;
        this.offsetDir = dir;

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
        Vector2Int pos = player.gridPos + offsetDir;

        Vector2 cellSize = Grid15x15.Instance.cellWorldSize;
        int gridCount = Grid15x15.Instance.gridCount;

        float half = (gridCount - 1) / 2f;

        float x = (pos.x - half) * cellSize.x;
        float y = (pos.y - half) * cellSize.y;

        transform.position = new Vector3(x, y, 0f);
    }
}
