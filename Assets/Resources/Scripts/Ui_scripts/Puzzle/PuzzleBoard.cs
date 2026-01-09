using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBoard : MonoBehaviour
{
    public int width = 6;
    public int height = 5;

    public Vector2 cellSize = new Vector2(60, 55);
    public Vector2 spacing = new Vector2(3, 5);

    public Text ComboText;
    private int ComboCount = 0;
    public PuzzleBlock[,] blocks;
    private RectTransform boardRect;

    List<PuzzleBlock> disabledBlocks = new List<PuzzleBlock>();

    private void Awake()
    {
        ComboText.enabled = false;
        boardRect = GetComponent<RectTransform>();
        blocks = new PuzzleBlock[width, height];
        InitBoard();
    }
    private void Start()
    {
        OnRectTransformDimensionsChange();
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

    public Vector3 GetPosition(int x, int y)
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

        DisableMatchedBlocks();
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

    void DisableMatchedBlocks()
    {
        HashSet<PuzzleBlock> matched = new HashSet<PuzzleBlock>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                PuzzleBlock center = blocks[x, y];
                if (center == null) continue;

                // 가로
                List<PuzzleBlock> h = GetLineBlocks(center, 1, 0);
                if (h.Count >= 3)
                    matched.UnionWith(h);

                // 세로
                List<PuzzleBlock> v = GetLineBlocks(center, 0, 1);
                if (v.Count >= 3)
                    matched.UnionWith(v);
            }
        }


        if (matched.Count > 0)
        {
            ComboText.enabled = true;
            ComboCount++;
            ComboText.text = "Combo " + ComboCount;
            StartCoroutine(offComboTEXT());
        }
        // 같은 퍼즐 3개 이상 포함된 블럭들만 false
        foreach (PuzzleBlock block in matched)
        {
            blocks[block.x, block.y] = null;
            //block.gameObject.SetActive(false);
            Image img = block.GetComponent<Image>();//매칭된 블록 흐림처리
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, 0.4f);
            block.isDisabled = true;

            disabledBlocks.Add(block);
        }
    }

    List<PuzzleBlock> GetLineBlocks(PuzzleBlock center, int dx, int dy)
    {
        List<PuzzleBlock> result = new List<PuzzleBlock>();
        result.Add(center);

        int x = center.x + dx;
        int y = center.y + dy;
        while (IsSame(center, x, y))
        {
            result.Add(blocks[x, y]);
            x += dx;
            y += dy;
        }

        x = center.x - dx;
        y = center.y - dy;
        while (IsSame(center, x, y))
        {
            result.Add(blocks[x, y]);
            x -= dx;
            y -= dy;
        }

        return result;
    }

    bool IsSame(PuzzleBlock center, int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return false;

        PuzzleBlock target = blocks[x, y];
        if (target == null)
            return false;

        return target.puzzleId == center.puzzleId;
    }

    IEnumerator offComboTEXT()
    {

        yield return new WaitForSeconds(3F);

        ComboText.enabled = false;
    }

    public void BoomPuzzle()
    {
        foreach (PuzzleBlock block in disabledBlocks)
        {
            if (block != null)
                block.gameObject.SetActive(false);
        }

        disabledBlocks.Clear();
    }
}

