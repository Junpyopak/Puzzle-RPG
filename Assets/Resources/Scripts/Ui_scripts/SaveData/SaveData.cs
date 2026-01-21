using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData
{
    public int slotIndex;

    public Vector3 playerPosition;
    public int playerHp;
    public int playerExp;

    public string currentScene;
    public string saveTime;
    public float gameTime;//이게 불러올 타이머 시간

    public PuzzleSaveData puzzleData;//퍼즐상태 저장
    
    // 새 필드: 이어하기용 카드 ID 리스트
    public List<int> gainedCardIDs = new List<int>();
}

