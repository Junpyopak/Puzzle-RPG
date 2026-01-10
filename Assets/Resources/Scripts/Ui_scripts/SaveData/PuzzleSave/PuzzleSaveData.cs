using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleSaveData
{
    public int width;
    public int height;

    public int[] puzzleIds;// 퍼즐의 id
    public bool[] disabled;// 비활성화 된 퍼즐들
}