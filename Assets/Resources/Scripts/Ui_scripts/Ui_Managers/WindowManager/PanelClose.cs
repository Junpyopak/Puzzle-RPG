using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PanelClose : MonoBehaviour
{
    public GameObject panel;
    public Player player;
    public PlayerMove1 playerMove;
    public GameObject OpenItemBox;
    public UI_GameTimer timer;
    public GameObject DarkImage;
    public GameObject DarkImage1;
    public GameObject DarkImage2;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
        OpenItemBox.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            // 현재 상태 반전
            panel.SetActive(!panel.activeSelf);
            DarkImage.SetActive(panel.activeSelf);
            DarkImage1.SetActive(panel.activeSelf);
            DarkImage2.SetActive(panel.activeSelf);
        }
    }
    public void Close()
    {
        panel.SetActive(!panel.activeSelf);
        DarkImage.SetActive(panel.activeSelf);
        DarkImage1.SetActive(panel.activeSelf);
        DarkImage2.SetActive(panel.activeSelf);
    }
    //public void Exit()
    //{
    //    if (Scenemgr.Instance != null)
    //    {
    //        Scenemgr.Instance.ChangeScene(eSCENE.TITLE);
    //    }
    //    Debug.Log("게임을 끝내겠습니다");
    //    panel.SetActive(!panel.activeSelf);
    //}
    public void Exit()
    {
        int slot = SaveContext.Instance.currentSlot;
        PuzzleBoard board = FindObjectOfType<PuzzleBoard>();
        Turn_Timer turn_Timer = FindObjectOfType<Turn_Timer>();
        PlayerCardManager cardManager = FindObjectOfType<PlayerCardManager>();
        PlayerMove1 move = FindObjectOfType<PlayerMove1>();

        int currentTurn = 1;
        if (turn_Timer != null)
        {
            currentTurn = turn_Timer.TurnCount;
        }
        // 슬롯이 선택된 상태라면 저장
        if (slot >= 0)
        {
            SaveData data = new SaveData
            {
                //level = player.level,
                //playerExp = player.Exp,
                //playerHp = player.Hp,
                playerPosition = player.transform.position,
                playerGridX = playerMove.GridPos.x,
                playerGridY = playerMove.GridPos.y,
                saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                gameTime = timer.GameTime,
                puzzleData = board.GetSaveData(),
                gainedCards = cardManager.GetSaveData(),
                gainedCardIDs = new List<int>(CardGainDataHolder.Instance.Data.gainedCardIDs),

                playerStats = new PlayerStatSaveData
                {
                    level = player.PlayerLevel,
                    exp = player.Exp,
                    needExp = player.NeedExp,
                    maxHp = player.MaxHp,
                    currentHp = player.Hp,
                    attack = player.PlayerATK,
                    defense = player.Defence,
                    moveCount = move.moveCount,
                    moveRemain = move.moveRemain,
                    //은근슬쩍 회복 저장 추가
                    recovery = player.Recovery,
                    recoveryAmount = player.recoveryAmount,
                    recoveryTurnInterval = player.recoveryTurnInterval,
                    recoveryTurnCounter = player.recoveryTurnCounter
                },
                fieldItems = FieldItemManager.Instance.GetSaveData(),
                TurnCount = currentTurn,
                hasBubbleShield = move.isBubbleShield,
                bubbleShieldPercent = move.BubblePowerPercent,
            };
            // ===== 몬스터 저장 추가 =====
            data.monsters.Clear();

            foreach (Monster m in TurnManager.Instance.monsters)
            {
                data.monsters.Add(m.GetSaveData());
            }
            // ===========================
            SaveManager.Save(slot, data);
            Debug.Log($"슬롯 {slot} 저장 완료");
            Debug.Log($"[Exit] Player position before save: {player.transform.position}");

        }
        else
        {
            Debug.LogWarning("선택된 슬롯이 없어 저장하지 않았습니다");
        }

        // 타이틀로 이동
        if (Scenemgr.Instance != null)
        {
            Scenemgr.Instance.ChangeScene(eSCENE.TITLE);
        }

        panel.SetActive(false);
        DarkImage.SetActive(false);
        DarkImage1.SetActive(false);
        DarkImage2.SetActive(false);
    }
    public void OpenItem()
    {
        OpenItemBox.SetActive(true);
    }

}

