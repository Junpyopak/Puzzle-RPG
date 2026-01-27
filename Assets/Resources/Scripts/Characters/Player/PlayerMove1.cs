using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMove1 : MonoBehaviour
{
    public float Movecell = 1f;
    public bool hasMoved = false;
    SpriteRenderer spriteRenderer;
    Vector2Int gridPos;
    void Start()
    {
        int center = Grid15x15.Instance.gridCount / 2;
        gridPos = new Vector2Int(center, center);

        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(InitPosition());
    }
    IEnumerator InitPosition()
    {
        //Grid가 cellWorldSize 계산할 때까지 대기
        yield return new WaitForEndOfFrame();

        int center = Grid15x15.Instance.gridCount / 2;
        gridPos = new Vector2Int(center, center);

        SnapToCell();
    }
    void Update()
    {
        if (hasMoved) return;
        HandleMove();
    }

    void HandleMove()
    {
        int x = 0;
        int y = 0;

        if (Input.GetKeyDown(KeyCode.W)) y = 1;
        else if (Input.GetKeyDown(KeyCode.S)) y = -1;
        else if (Input.GetKeyDown(KeyCode.A)) x = -1;
        else if (Input.GetKeyDown(KeyCode.D)) x = 1;

        //대각 (QEZC)
        else if (Input.GetKeyDown(KeyCode.Q)) { x = -1; y = 1; }
        else if (Input.GetKeyDown(KeyCode.E)) { x = 1; y = 1; }
        else if (Input.GetKeyDown(KeyCode.Z)) { x = -1; y = -1; }
        else if (Input.GetKeyDown(KeyCode.C)) { x = 1; y = -1; }
        else return;

        gridPos += new Vector2Int(x, y);

        int max = Grid15x15.Instance.gridCount - 1;
        gridPos.x = Mathf.Clamp(gridPos.x, 0, max);
        gridPos.y = Mathf.Clamp(gridPos.y, 0, max);

        if (x < 0) spriteRenderer.flipX = true;
        else if (x > 0) spriteRenderer.flipX = false;

        SnapToCell();
        hasMoved = true;
    }
    void SnapToCell()
    {
        Vector2 cellSize = Grid15x15.Instance.cellWorldSize;
        int gridCount = Grid15x15.Instance.gridCount;
        float half = (gridCount - 1) / 2f;

        float x = (gridPos.x - half) * cellSize.x;
        float y = (gridPos.y - half) * cellSize.y;

        transform.position = new Vector3(x, y, transform.position.z);
    }

    // 턴 끝날 때 호출
    public void ResetMove()
    {
        hasMoved = false;
    }
}
