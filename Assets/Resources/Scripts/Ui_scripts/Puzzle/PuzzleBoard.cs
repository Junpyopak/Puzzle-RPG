using UnityEngine;

public class PuzzleBoard : MonoBehaviour
{
    public int width = 6;
    public int height = 5;

    public Vector2 cellSize = new Vector2(60, 55);
    public Vector2 spacing = new Vector2(3, 5);

    public PuzzleBlock[,] blocks;
    private RectTransform boardRect;

    private void Awake()
    {
        boardRect = GetComponent<RectTransform>();
        blocks = new PuzzleBlock[width, height];
        InitBoard();
    }

    void InitBoard()
    {
        int i = 0;

        foreach (Transform child in transform)
        {
            PuzzleBlock block = child.GetComponent<PuzzleBlock>();
            if (block == null)
                continue;

            int x = i % width;
            int y = i / width;

            blocks[x, y] = block;

            block.board = this;
            block.x = x;
            block.y = y;

            block.transform.localPosition = GetPosition(x, y);

            i++;
        }
    }

    Vector3 GetPosition(int x, int y)
    {
        float startX = -(width - 1) * (cellSize.x + spacing.x) / 2f;
        float startY = -(height - 1) * (cellSize.y + spacing.y) / 2f;

        return new Vector3(
            startX + x * (cellSize.x + spacing.x),
            startY + y * (cellSize.y + spacing.y),
            0f
        );
    }

    public void TrySwap(PuzzleBlock a, PuzzleBlock b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        // 자기 자신 클릭 → 무시
        if (dx == 0 && dy == 0)
            return;

        // 상 / 하 / 좌 / 우 / 대각선 까지 1칸만 허용
        if (dx > 1 || dy > 1)
        {
            // 원래 자리로 되돌리기
            a.transform.localPosition = GetPosition(a.x, a.y);
            b.transform.localPosition = GetPosition(b.x, b.y);
            return;
        }

        //스왑
        blocks[a.x, a.y] = b;
        blocks[b.x, b.y] = a;

        int tx = a.x;
        int ty = a.y;

        a.x = b.x;
        a.y = b.y;

        b.x = tx;
        b.y = ty;

        a.transform.localPosition = GetPosition(a.x, a.y);
        b.transform.localPosition = GetPosition(b.x, b.y);
    }
    void OnRectTransformDimensionsChange()
    {
        if (blocks == null) return;

        CalculateCellSize();

        foreach (var block in blocks)
        {
            if (block == null) continue;

            RectTransform rect = block.GetComponent<RectTransform>();
            rect.sizeDelta = cellSize;
            rect.anchoredPosition = GetPosition(block.x, block.y);
        }
    }
    void CalculateCellSize()
    {
        Vector2 boardSize = boardRect.rect.size;

        float cellW = (boardSize.x - spacing.x * (width - 1)) / width;
        float cellH = (boardSize.y - spacing.y * (height - 1)) / height;

        cellSize = new Vector2(cellW, cellH);
    }
}

