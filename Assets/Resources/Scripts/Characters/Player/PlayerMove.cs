using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Grid Settings")]
    public float cellSize = 1f;

    [Header("Prefabs")]
    public GameObject moveTilePrefab;
    public GameObject selectTilePrefab;

    [Header("Obstacle")]
    public LayerMask obstacleLayer;

    private List<GameObject> moveTiles = new List<GameObject>();
    private HashSet<Vector2Int> moveableGrids = new HashSet<Vector2Int>();

    private GameObject selectTile;

    private Vector2Int currentGridPos;
    private Vector2Int selectedGridPos;

    private bool selectMode = false;

    void Start()
    {
        UpdateGridPos();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            ToggleSelectMode();

        if (!selectMode) return;

        HandleSelectInput();

        if (Input.GetKeyDown(KeyCode.Return))
            ConfirmMove();
    }

    void ToggleSelectMode()
    {
        selectMode = !selectMode;

        if (selectMode)
        {
            selectedGridPos = currentGridPos;
            ShowMoveableTiles();
            UpdateSelectTile();
        }
        else
        {
            ClearAllTiles();
        }
    }
    // 선택 이동
    //void HandleSelectInput()
    //{
    //    Vector2Int dir = Vector2Int.zero;

    //    if (Input.GetKeyDown(KeyCode.W)) dir.y += 1;
    //    if (Input.GetKeyDown(KeyCode.S)) dir.y -= 1;
    //    if (Input.GetKeyDown(KeyCode.A)) dir.x -= 1;
    //    if (Input.GetKeyDown(KeyCode.D)) dir.x += 1;


    //    Debug.Log("dir.y = " + dir.y);
    //    Debug.Log("dir.x = " + dir.x);

    //    if (dir == Vector2Int.zero) return;
    //    Vector2Int nextGrid = selectedGridPos + dir;

    //    //이동 가능 칸 안에 있을 때만 이동
    //    if (moveableGrids.Contains(nextGrid))
    //    {
    //        selectedGridPos = nextGrid;
    //        UpdateSelectTile();
    //    }
    //}
    void HandleSelectInput()
    {
        bool w = Input.GetKey(KeyCode.W);
        bool a = Input.GetKey(KeyCode.A);
        bool s = Input.GetKey(KeyCode.S);
        bool d = Input.GetKey(KeyCode.D);

        bool inputTriggered =
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.D);

        if (!inputTriggered) return;

        // 기존 이동 계산 (절대 막지 않음)
        Vector2Int dir = Vector2Int.zero;

        if (w) dir.y += 1;
        if (s) dir.y -= 1;
        if (a) dir.x -= 1;
        if (d) dir.x += 1;

        if (dir == Vector2Int.zero) return;

        dir.x = Mathf.Clamp(dir.x, -1, 1);
        dir.y = Mathf.Clamp(dir.y, -1, 1);

        Vector2Int nextGrid = selectedGridPos + dir;

        // 현재 선택 위치가 대각선인가?
        Vector2Int relative = selectedGridPos - currentGridPos;

        if (relative == Vector2Int.zero)
        {
            if (w && d)
                nextGrid = currentGridPos + new Vector2Int(1, 1);
            else if (w && a)
                nextGrid = currentGridPos + new Vector2Int(-1, 1);
            else if (s && d)
                nextGrid = currentGridPos + new Vector2Int(1, -1);
            else if (s && a)
                nextGrid = currentGridPos + new Vector2Int(-1, -1);
        }
        bool isDiagonal =
            Mathf.Abs(relative.x) == 1 &&
            Mathf.Abs(relative.y) == 1;

        //// WA / WD + 대각선이면 결과만 보정
        //if (isDiagonal && w && (a || d))
        //{
        //    // "항상 자신의 반대 대각선"
        //    nextGrid = currentGridPos - relative;
        //}
        //아래칸에서 W → 맨 위칸
        if (relative == Vector2Int.down && w)
        {
            nextGrid = currentGridPos + Vector2Int.up;
        }
        if (relative == Vector2Int.up && s)
        {
            nextGrid = currentGridPos + Vector2Int.down;
        }
        if (relative == Vector2Int.left && d)
        {
            nextGrid = currentGridPos + Vector2Int.right;
        }
        if (relative == Vector2Int.right && a)
        {
            nextGrid = currentGridPos + Vector2Int.left;
        }
        // 최종 적용
        if (moveableGrids.Contains(nextGrid))
        {
            selectedGridPos = nextGrid;
            UpdateSelectTile();
        }
    }


    // 이동 확정
    void ConfirmMove()
    {
        if (!moveableGrids.Contains(selectedGridPos)) return;

        transform.position = GridToWorld(selectedGridPos);
        currentGridPos = selectedGridPos;

        ClearAllTiles();
        selectMode = false;
    }

    // 이동 가능 칸 표시 (8방향)
    void ShowMoveableTiles()
    {
        ClearMoveTiles();
        moveableGrids.Clear();

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int grid = currentGridPos + new Vector2Int(x, y);
                Vector3 worldPos = GridToWorld(grid);

                if (IsWalkable(worldPos))
                {
                    moveableGrids.Add(grid);
                    SpawnMoveTile(worldPos);
                }
            }
        }
    }

    // ===============================
    // 유틸
    // ===============================
    void UpdateGridPos()
    {
        currentGridPos = WorldToGrid(transform.position);
    }

    Vector2Int WorldToGrid(Vector3 world)
    {
        return new Vector2Int(
            Mathf.RoundToInt(world.x / cellSize),
            Mathf.RoundToInt(world.y / cellSize)
        );
    }

    Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(grid.x * cellSize, grid.y * cellSize, 0);
    }

    bool IsWalkable(Vector3 pos)
    {
        Collider2D hit = Physics2D.OverlapBox(
            pos,
            Vector2.one * cellSize * 0.8f,
            0,
            obstacleLayer
        );
        return hit == null;
    }

    void SpawnMoveTile(Vector3 pos)
    {
        GameObject tile = Instantiate(moveTilePrefab, pos, Quaternion.identity);
        moveTiles.Add(tile);
    }

    void UpdateSelectTile()
    {
        if (selectTile == null)
            selectTile = Instantiate(selectTilePrefab);

        selectTile.transform.position = GridToWorld(selectedGridPos);
    }

    void ClearMoveTiles()
    {
        foreach (var t in moveTiles)
            Destroy(t);
        moveTiles.Clear();
    }

    void ClearAllTiles()
    {
        ClearMoveTiles();
        moveableGrids.Clear();

        if (selectTile != null)
            Destroy(selectTile);
    }
}
