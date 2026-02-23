using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleBoard : MonoBehaviour
{
    public int width = 6;
    public int height = 5;

    public Vector2 cellSize = new Vector2(60, 55);
    public Vector2 spacing = new Vector2(3, 5);
    [Header("콤보 공격력")]
    public Player player; // 인스펙터에서 연결
    public Text ComboText;
    private int ComboCount = 0;
    private float tempATKMultiplier = 1f; // 콤보 배율
    private int originalATK; // 턴 시작 시 저장할 공격력
    public int comboAtk = 0;  // 콤보 공격력 변수

    public PuzzleBlock[,] blocks;
    private RectTransform boardRect;
    List<PuzzleBlock> disabledBlocks = new List<PuzzleBlock>();

    [Header("Control State")]
    public bool isPlayerTurn = true;
    public bool isPlayerDead = false;

    public AudioClip puzzlePopSound;
    public AudioClip puzzleSwapSound;
    public AudioClip puzzleDownSound;


    private void Awake()
    {
        //ComboText.enabled = false;
        //boardRect = GetComponent<RectTransform>();
        //blocks = new PuzzleBlock[width, height];
        //InitBoard();
        ComboText.enabled = false;
        boardRect = GetComponent<RectTransform>();
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("씬에 Player가 없습니다!");
        }
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
        if (!CanControl())
        {
            // 원위치 복귀
            a.transform.localPosition = GetPosition(a.x, a.y);
            b.transform.localPosition = GetPosition(b.x, b.y);
            return;
        }

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

        GameManager.Instance.SoundMgr.SoundPlay("sfx","퍼즐스왑 사운드",puzzleSwapSound);
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

    //void DisableMatchedBlocks()
    //{
    //    HashSet<PuzzleBlock> matched = new HashSet<PuzzleBlock>();

    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int y = 0; y < height; y++)
    //        {
    //            PuzzleBlock center = blocks[x, y];
    //            if (center == null) continue;

    //            // 가로
    //            List<PuzzleBlock> h = GetLineBlocks(center, 1, 0);
    //            if (h.Count >= 3)
    //                matched.UnionWith(h);

    //            // 세로
    //            List<PuzzleBlock> v = GetLineBlocks(center, 0, 1);
    //            if (v.Count >= 3)
    //                matched.UnionWith(v);
    //        }
    //    }

    //    if (matched.Count > 0)
    //    {
    //        ComboText.enabled = true;
    //        ComboCount++;
    //        ComboText.text = "Combo " + ComboCount;

    //        tempATKMultiplier = GetComboMultiplier(ComboCount);

    //        // PlayerATK를 직접 현재 값 기준으로 곱함
    //        player.PlayerATK = Mathf.Max(1, Mathf.RoundToInt(player.PlayerATK * tempATKMultiplier));
    //        //콤보 시작 시 원래 공격력 저장

    //        // 턴 시작 시 기본 공격력 저장

    //        Debug.Log($"콤보 {ComboCount} → PlayerATK {player.PlayerATK}");

    //        StartCoroutine(offComboTEXT());
    //    }

    //    // 같은 퍼즐 3개 이상 포함된 블럭들만 false
    //    foreach (PuzzleBlock block in matched)
    //    {
    //        if (block.isDisabled) continue;
    //        //blocks[block.x, block.y] = null;
    //        //block.gameObject.SetActive(false);
    //        Image img = block.GetComponent<Image>();//매칭된 블록 흐림처리
    //        Color c = img.color;
    //        img.color = new Color(c.r, c.g, c.b, 0.4f);
    //        block.isDisabled = true;

    //        disabledBlocks.Add(block);
    //    }
    //}
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

        //if (matched.Count > 0)
        //{
        //    ComboText.enabled = true;
        //    ComboCount++;
        //    ComboText.text = "Combo " + ComboCount;

        //    tempATKMultiplier = GetComboMultiplier(ComboCount);

        //    // baseATK 기준으로 곱하기 → 누적 안 됨
        //    player.PlayerATK = Mathf.Max(1, Mathf.RoundToInt(player.baseATK * tempATKMultiplier));

        //    Debug.Log($"콤보 {ComboCount} → PlayerATK {player.PlayerATK}");

        //    StartCoroutine(offComboTEXT());
        //}

        if (matched.Count > 0)
        {
            ComboText.enabled = true;
            ComboCount++;
            ComboText.text = "Combo " + ComboCount;

            tempATKMultiplier = GetComboMultiplier(ComboCount);


            if (originalATK == 0)
                originalATK = player.PlayerATK;
            //항상 턴 시작 시 공격력 기준으로 계산 (누적 방지)
            player.PlayerATK = Mathf.Max(1, Mathf.RoundToInt(originalATK * tempATKMultiplier));

            Debug.Log($"콤보 {ComboCount} → PlayerATK {player.PlayerATK}");

            StartCoroutine(offComboTEXT());
        }


        // 같은 퍼즐 3개 이상 포함된 블럭들만 false
        foreach (PuzzleBlock block in matched)
        {
            if (block.isDisabled) continue;
            Image img = block.GetComponent<Image>();
            Color c = img.color;
            img.color = new Color(c.r, c.g, c.b, 0.4f);
            block.isDisabled = true;

            disabledBlocks.Add(block);
        }
    }
    float GetComboMultiplier(int comboCount)
    {
        if (comboCount <= 3) return 1.5f;
        if (comboCount <= 6) return 3f;
        if (comboCount <= 9) return 4.5f;
        return 6f;
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
        //이미 disable 된 블록은 매칭 불가
        if (target.isDisabled || center.isDisabled)
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
            GameManager.Instance.SoundMgr.SoundPlay("sfx","퍼즐다운 사운드",puzzleDownSound);

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
            GameManager.Instance.SoundMgr.SoundPlay("sfx","퍼즐팝 사운드",puzzlePopSound);
            Destroy(block.gameObject);
        }

        disabledBlocks.Clear();
        ComboCount = 0;
        yield return new WaitForSeconds(0.1f);

        // 블록 하나씩 중력
        yield return StartCoroutine(ApplyGravity_BlockByBlock(0.03f));

        // 새 블록 채우기
        PuzzleSpawner spawner = FindObjectOfType<PuzzleSpawner>();
        yield return new WaitForSeconds(0.05f);
        spawner.FillEmptyBlocks();
    }
    public IEnumerator BoomAndThen(System.Action onComplete)
    {
        yield return StartCoroutine(BoomRoutine());

        onComplete?.Invoke();
    }
    public bool HasMatchedBlocks()
    {
        return disabledBlocks.Count > 0;
    }
    public bool CanControl()
    {
        return isPlayerTurn && !isPlayerDead;
    }


    //public void OnTurnStart()
    //{
    //    ComboCount = 0;
    //    tempATKMultiplier = 1f;

    //    if (player != null)
    //        player.PlayerATK = player.baseATK; // 기본 공격력으로 초기화

    //    Debug.Log($"턴 시작 → PlayerATK 초기화: {player.PlayerATK}");
    //}
    public void OnTurnStart()
    {
        ComboCount = 0;
        tempATKMultiplier = 1f;

        if (player != null)
        {
            originalATK = player.PlayerATK; // 현재 공격력 저장 (카드/레벨 반영된 값)
        }

        Debug.Log($"턴 시작 → originalATK 저장: {originalATK}");
    }

    //// 턴 종료 시
    //public void OnTurnEnd()
    //{
    //    ComboCount = 0;
    //    tempATKMultiplier = 1f;

    //    if (player != null)
    //        player.PlayerATK = player.baseATK; // 턴 종료 후 공격력 원복
    //}
    public void OnTurnEnd()
    {
        ComboCount = 0;
        tempATKMultiplier = 1f;

        if (player != null)
        {
            player.PlayerATK = originalATK; // 턴 시작 시 공격력으로 복원
            originalATK = 0;
        }
    }
}

