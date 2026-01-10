using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleSpawner : MonoBehaviour
{
    [Header("Board")]
    public PuzzleBoard board;

    [Header("Block Prefabs (Color별)")]
    public PuzzleBlock[] blockPrefabs; // 빨강, 파랑, 노랑 등

    private void Start()
    {
        //if (SaveContext.Instance.isLoading && SaveContext.Instance.currentSaveData != null)
        //{
        //    PuzzleSaveData data = SaveContext.Instance.currentSaveData.puzzleData;
        //    if (data != null)
        //    {
        //        // 기존 블록이 있는지 없는지 체크 없이 데이터 기반 생성
        //        SpawnFromSaveData(data);
        //        return;
        //    }
        //}

        //// 새 게임
        //SpawnAll();
        //if (board.blocks == null || board.blocks.Length == 0)
        //{
        //    board.blocks = new PuzzleBlock[board.width, board.height];
        //}

        //// 이어하기이면 기존 블록 그대로 보여주고 스폰하지 않음
        //if (SaveContext.Instance.isLoading)
        //{
        //    Debug.Log("이어하기 모드: 기존 블록 그대로 사용");
        //    return;
        //}

        ////새 게임이면 블록 스폰
        //// SpawnAll();
        ///
        if (board.blocks == null || board.blocks.Length == 0)
            board.blocks = new PuzzleBlock[board.width, board.height];

        // 슬롯 선택 모드에 따라 분기
        if (SlotSelectContext.Instance.mode == SlotSelectMode.Continue)
        {
            // 이어하기 → 기존 블록만 표시
            Debug.Log("이어하기 모드: 기존 블록만 보여줌");
            ShowExistingBlocks();
            return;
        }

        // 새 게임 → SpawnAll
        Debug.Log("새 게임 모드: 블록 생성");
        SpawnAll();


    }

    private void ShowExistingBlocks()
    {
        for (int y = 0; y < board.height; y++)
        {
            for (int x = 0; x < board.width; x++)
            {
                PuzzleBlock block = board.blocks[x, y];
                if (block != null)
                {
                    block.gameObject.SetActive(true);

                    RectTransform rect = block.GetComponent<RectTransform>();
                    rect.sizeDelta = board.cellSize;
                    rect.anchoredPosition = board.GetPosition(x, y);

                    Image img = block.GetComponent<Image>();
                    if (img != null)
                    {
                        Color c = img.color; // 기존 색상 그대로 유지
                        img.color = new Color(c.r, c.g, c.b, block.isDisabled ? 0.4f : 1f);
                    }
                }
            }
        }
    }

    public void SpawnAll()
    {
        int w = board.width;
        int h = board.height;

        board.blocks = new PuzzleBlock[w, h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                //SpawnBlock(x, y);
                SpawnBlock_MaxTwo(x, y);
            }
        }
    }

    //void SpawnBlock(int x, int y)
    //{
    //    // 랜덤 프리팹 선택
    //    PuzzleBlock prefab = blockPrefabs[Random.Range(0, blockPrefabs.Length)];

    //    PuzzleBlock block = Instantiate(prefab, board.transform);

    //    // 보드 정보 세팅
    //    block.board = board;
    //    block.x = x;
    //    block.y = y;

    //    // 보드 배열에 등록
    //    board.blocks[x, y] = block;

    //    // 위치 & 사이즈
    //    RectTransform rect = block.GetComponent<RectTransform>();
    //    rect.sizeDelta = board.cellSize;
    //    rect.anchoredPosition = board.GetPosition(x, y);
    //}
    /// 좌/하 방향만 검사해서 3매치가 생기지 않는 프리팹만 선택
    void SpawnBlock_MaxTwo(int x, int y)
    {
        List<PuzzleBlock> candidates = new List<PuzzleBlock>(blockPrefabs);

        // 가로 검사 (왼쪽 2칸)
        if (x >= 2)
        {
            PuzzleBlock a = board.blocks[x - 1, y];
            PuzzleBlock b = board.blocks[x - 2, y];

            if (a != null && b != null && a.puzzleId == b.puzzleId)
            {
                candidates.RemoveAll(p => p.puzzleId == a.puzzleId);
            }
        }

        // 세로 검사 (아래 2칸)
        if (y >= 2)
        {
            PuzzleBlock a = board.blocks[x, y - 1];
            PuzzleBlock b = board.blocks[x, y - 2];

            if (a != null && b != null && a.puzzleId == b.puzzleId)
            {
                candidates.RemoveAll(p => p.puzzleId == a.puzzleId);
            }
        }

        // 안전장치
        if (candidates.Count == 0)
            candidates.AddRange(blockPrefabs);

        PuzzleBlock prefab = candidates[Random.Range(0, candidates.Count)];
        PuzzleBlock block = Instantiate(prefab, board.transform);

        block.board = board;
        block.x = x;
        block.y = y;

        board.blocks[x, y] = block;

        RectTransform rect = block.GetComponent<RectTransform>();
        rect.sizeDelta = board.cellSize;
        rect.anchoredPosition = board.GetPosition(x, y);
    }
    //public void SpawnFromSaveData(PuzzleSaveData data)
    //{
    //    int w = data.width;
    //    int h = data.height;

    //    board.blocks = new PuzzleBlock[w, h];

    //    for (int y = 0; y < h; y++)
    //    {
    //        for (int x = 0; x < w; x++)
    //        {
    //            int index = y * w + x;

    //            int puzzleId = data.puzzleIds[index];
    //            if (puzzleId < 0)
    //                continue;

    //            PuzzleBlock prefab = System.Array.Find(
    //                blockPrefabs, p => p.puzzleId == puzzleId);

    //            if (prefab == null)
    //            {
    //                Debug.LogError($"퍼즐 프리팹 없음: {puzzleId}");
    //                continue;
    //            }

    //            PuzzleBlock block = Instantiate(prefab, board.transform);

    //            block.board = board;
    //            block.x = x;
    //            block.y = y;
    //            block.puzzleId = puzzleId;
    //            block.isDisabled = data.disabled[index];

    //            board.blocks[x, y] = block;

    //            RectTransform rect = block.GetComponent<RectTransform>();
    //            rect.sizeDelta = board.cellSize;
    //            rect.anchoredPosition = board.GetPosition(x, y);

    //            // 비활성화 복구
    //            Image img = block.GetComponent<Image>();
    //            Color c = img.color;

    //            if (block.isDisabled)
    //                img.color = new Color(c.r, c.g, c.b, 0.4f);
    //            else
    //                img.color = new Color(c.r, c.g, c.b, 1f);
    //        }
    //    }
    //}
    //public void SpawnFromSaveData(PuzzleSaveData data)
    //{
    //    board.blocks = new PuzzleBlock[data.width, data.height];

    //    for (int y = 0; y < data.height; y++)
    //    {
    //        for (int x = 0; x < data.width; x++)
    //        {
    //            int index = y * data.width + x;

    //            int puzzleId = data.puzzleIds[index];
    //            bool disabled = data.disabled[index];

    //            PuzzleBlock prefab =
    //                System.Array.Find(blockPrefabs, p => p.puzzleId == puzzleId);

    //            PuzzleBlock block = Instantiate(prefab, board.transform);

    //            block.board = board;
    //            block.x = x;
    //            block.y = y;
    //            block.isDisabled = disabled;

    //            board.blocks[x, y] = block;

    //            RectTransform rect = block.GetComponent<RectTransform>();
    //            rect.sizeDelta = board.cellSize;
    //            rect.anchoredPosition = board.GetPosition(x, y);

    //            if (disabled)
    //            {
    //                Image img = block.GetComponent<Image>();
    //                Color c = img.color;
    //                img.color = new Color(c.r, c.g, c.b, 0.4f);
    //            }
    //        }
    //    }
    //}
    public void SpawnFromSaveData(PuzzleSaveData data)
    {
        int w = data.width;
        int h = data.height;

        // 배열 초기화 제거 → 기존 블록 그대로 활용
        // board.blocks = new PuzzleBlock[w, h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int index = y * w + x;
                int puzzleId = data.puzzleIds[index];
                bool disabled = data.disabled[index];

                PuzzleBlock existing = board.blocks[x, y];

                if (existing != null)
                {
                    // 기존 블록이 있으면 데이터만 덮어쓰기
                    existing.puzzleId = puzzleId;
                    existing.isDisabled = disabled;

                    Image img = existing.GetComponent<Image>();
                    Color c = img.color;
                    img.color = disabled ? new Color(c.r, c.g, c.b, 0.4f) : new Color(c.r, c.g, c.b, 1f);
                }
                else
                {
                    // 기존 블록이 없으면 새로 생성
                    PuzzleBlock prefab = System.Array.Find(blockPrefabs, p => p.puzzleId == puzzleId);
                    PuzzleBlock block = Instantiate(prefab, board.transform);

                    block.board = board;
                    block.x = x;
                    block.y = y;
                    block.isDisabled = disabled;

                    RectTransform rect = block.GetComponent<RectTransform>();
                    rect.sizeDelta = board.cellSize;
                    rect.anchoredPosition = board.GetPosition(x, y);

                    if (disabled)
                    {
                        Image img = block.GetComponent<Image>();
                        Color c = img.color;
                        img.color = new Color(c.r, c.g, c.b, 0.4f);
                    }

                    board.blocks[x, y] = block;
                }
            }
        }
    }


}







