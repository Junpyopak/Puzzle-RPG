using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSpawner : MonoBehaviour
{
    [Header("Board")]
    public PuzzleBoard board;

    [Header("Block Prefabs (Color별)")]
    public PuzzleBlock[] blockPrefabs; // 빨강, 파랑, 노랑 등

    private void Start()
    {
        SpawnAll();
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

        // 가로: 같은 게 이미 2개 연속이면 그 색은 제외
        if (x >= 2)
        {
            PuzzleBlock a = board.blocks[x - 1, y];
            PuzzleBlock b = board.blocks[x - 2, y];

            if (a != null && b != null && a.name == b.name)
            {
                candidates.RemoveAll(p => p.name == a.name);
            }
        }

        // 세로: 같은 게 이미 2개 연속이면 그 색은 제외
        if (y >= 2)
        {
            PuzzleBlock a = board.blocks[x, y - 1];
            PuzzleBlock b = board.blocks[x, y - 2];

            if (a != null && b != null && a.name == b.name)
            {
                candidates.RemoveAll(p => p.name == a.name);
            }
        }

        // 안전장치 (거의 발생 안 함)
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
}







