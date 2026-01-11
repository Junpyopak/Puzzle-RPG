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
        //ComboText.enabled = false;
        //boardRect = GetComponent<RectTransform>();
        //blocks = new PuzzleBlock[width, height];
        //InitBoard();
        ComboText.enabled = false;
        boardRect = GetComponent<RectTransform>();

        if (SaveContext.Instance != null && SaveContext.Instance.isLoading)
        {
            //이어하기면 건드리지 마라
            return;
        }

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
            if (block.isDisabled) continue;
            //blocks[block.x, block.y] = null;
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

    //public void BoomPuzzle()
    //{
    //    foreach (PuzzleBlock block in disabledBlocks)
    //    {
    //        if (block != null)
    //            block.gameObject.SetActive(false);
    //        blocks[block.x, block.y] = null;
    //    }

    //    disabledBlocks.Clear();
    //}
    public void BoomPuzzle()
    {
        if (disabledBlocks.Count == 0)
        {
            Debug.Log("BoomPuzzle: 터질 블록 없음");
            return;
        }

        //foreach (PuzzleBlock block in disabledBlocks)
        //{
        //    if (block == null) continue;

        //    int x = block.x;
        //    int y = block.y;

        //    // 보드 데이터에서 제거
        //    if (x >= 0 && x < width && y >= 0 && y < height)
        //        blocks[x, y] = null;

        //    //진짜 삭제
        //    Destroy(block.gameObject);
        //}

        //disabledBlocks.Clear();
        StartCoroutine(BoomRoutine());
    }
    //Load 이후에 disabledBlocks를 다시 채우기 위함
    public void RebuildDisabledList()
    {
        disabledBlocks.Clear();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                PuzzleBlock block = blocks[x, y];
                if (block == null) continue;

                if (block.isDisabled)
                    disabledBlocks.Add(block);
            }
        }

        Debug.Log($"RebuildDisabledList: {disabledBlocks.Count}개 복구됨");
    }
    public PuzzleSaveData GetSaveData()
    {
        PuzzleSaveData data = new PuzzleSaveData();
        data.width = width;
        data.height = height;

        data.puzzleIds = new int[width * height];
        data.disabled = new bool[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                PuzzleBlock block = blocks[x, y];
                if (block != null)
                {
                    data.puzzleIds[index] = block.puzzleId;
                    data.disabled[index] = block.isDisabled;
                }
            }
        }

        return data;
    }
    public void LoadFromData(PuzzleSaveData data)
    {
        int w = Mathf.Min(width, data.width);
        int h = Mathf.Min(height, data.height);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int index = y * data.width + x;

                PuzzleBlock block = blocks[x, y];
                if (block == null) continue;

                block.puzzleId = data.puzzleIds[index];
                block.isDisabled = data.disabled[index];
            }
        }
    }
    //퍼즐블록 위에서 아래로 떨어트리기
    public IEnumerator ApplyGravity_BlockByBlock(float delay = 0.05f)
    {
        while (true)
        {
            bool movedOne = false;

            for (int y = 1; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (blocks[x, y] != null && blocks[x, y - 1] == null)
                    {
                        PuzzleBlock block = blocks[x, y];

                        blocks[x, y] = null;
                        blocks[x, y - 1] = block;

                        block.y = y - 1;

                        yield return StartCoroutine(
                            MoveBlockOneStep(block, GetPosition(x, y - 1))
                        );

                        movedOne = true;
                        yield return new WaitForSeconds(delay);
                        goto NEXT_STEP;
                    }
                }
            }

        NEXT_STEP:
            if (!movedOne)
                break;
        }
    }
    IEnumerator MoveBlockOneStep(PuzzleBlock block, Vector3 target)
    {
        Vector3 start = block.transform.localPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 12f;
            block.transform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }

        block.transform.localPosition = target;
    }

    IEnumerator BoomRoutine()
    {
        foreach (PuzzleBlock block in disabledBlocks)
        {
            if (block == null) continue;

            blocks[block.x, block.y] = null;
            Destroy(block.gameObject);
        }

        disabledBlocks.Clear();

        yield return new WaitForSeconds(0.1f);

        // 블록 하나씩 중력
        yield return StartCoroutine(ApplyGravity_BlockByBlock(0.03f));

        // 새 블록 채우기
        PuzzleSpawner spawner = FindObjectOfType<PuzzleSpawner>();
        yield return new WaitForSeconds(0.05f);
        spawner.FillEmptyBlocks();
    }

}

