using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMove1 : MonoBehaviour
{
    public float Movecell = 1f;
    public bool isPlayerTurn = true;
    SpriteRenderer spriteRenderer;
    public Vector2Int GridPos => gridPos;
    public Vector2Int gridPos;
    public int moveCount = 1;
    public int moveRemain;

    [Header("Bubble Shield")]
    public GameObject bubbleShieldPrefab;

    private List<GameObject> activeBubbles = new List<GameObject>();
    public bool isBubbleShield = false;
    public int BubblePower ;
    public float BubblePowerPercent = 0f;
    void Start()
    {
        moveRemain = moveCount;
        int center = Grid15x15.Instance.gridCount / 2;
        gridPos = new Vector2Int(center, center);

        spriteRenderer = GetComponent<SpriteRenderer>();
        // 이동 횟수 초기화
        moveRemain = moveCount;
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
    { // 테스트용: B키를 누르면 버블 생성
        if (Input.GetKeyDown(KeyCode.B))
        {
            // 예시: 레벨 1 기준 CSV float 퍼센트
            float testPercent = 0.03f; // 3%
            CreateBubbleShield(testPercent);

            Debug.Log($"[테스트] B 눌러 버블 생성! 데미지 퍼센트: {testPercent * 100}%");
        }
        if (!isPlayerTurn) return;
        // 이동 횟수 다 쓰면 이동 불가
        if (moveRemain <= 0) return;
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
        moveRemain--;
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
        moveRemain = moveCount;
    }
    public void AddMoveCount(int amount)
    {
        moveCount += amount;
        moveRemain = moveCount;
    }
    public void LoadGridPosition(Vector2Int savedGridPos)
    {
        gridPos = savedGridPos;
        SnapToCell();
    }

    public void CreateBubbleShield(float damagePercen)
    {
        // 이미 있으면 생성 안함
        if (activeBubbles.Count > 0)
            return;
        Vector3 spawnPos = transform.position + new Vector3(0f, 1f, 0f); // y = 1만큼 위로
        // 중앙에 1개만 생성
        GameObject bubble = Instantiate(bubbleShieldPrefab, spawnPos, Quaternion.identity);

        // 스케일 변경
        bubble.transform.localScale = new Vector3(3f, 3f, 1f);

        // 버블 초기화
        BubbleShield shield = bubble.GetComponent<BubbleShield>();
        shield.Init(this, Vector2Int.zero, damagePercen); // offsetDir = 0,0 (중앙)

        activeBubbles.Add(bubble);
        isBubbleShield = true;

        BubblePowerPercent = damagePercen; //저장용
    }
}
