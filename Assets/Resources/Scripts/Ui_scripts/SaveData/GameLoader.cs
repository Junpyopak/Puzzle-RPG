using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public static void NewGame(int slot)
    {
        SaveContext.Instance.currentSlot = slot;
        SaveContext.Instance.isLoading = false;
        SceneManager.LoadScene("GameScene");
    }

    public static void LoadGame(int slot)
    {
        SaveContext.Instance.currentSlot = slot;
        SaveContext.Instance.isLoading = true;
        SceneManager.LoadScene(
            SaveManager.Load(slot).currentScene
        );
    }

    void Start()
    {
        int slot = SaveContext.Instance.currentSlot;

        if (slot < 0) return;

        SaveData data = SaveManager.Load(slot);
        if (data == null) return;

        // 씬에 원래 배치된 아이템 전부 제거
        foreach (var item in FindObjectsOfType<ItemID>())
        {
            Destroy(item.gameObject);
        }

        //저장된 아이템만 다시 생성
        foreach (var saved in data.fieldItems)
        {
            GameObject prefab = ItemDatabase.Instance.GetItemPrefab(saved.ItemID);
            if (prefab == null) continue;

            GameObject go = Instantiate(prefab, saved.ItemPos, Quaternion.identity);
            FieldItemManager.Instance.Register(go.GetComponent<ItemID>());
        }
        Player player = FindObjectOfType<Player>();
        player.ApplySaveData(data);
        Debug.Log($"[Load] Player position after Apply: {player.transform.position}");

        // 타이머 복구 (이게 핵심)
        UI_GameTimer timer = FindObjectOfType<UI_GameTimer>();
        if (timer != null)
        {
            timer.SetTime(data.gameTime);
        }
        Turn_Timer turn = FindObjectOfType<Turn_Timer>();
        if (turn != null && data != null)
        {
            // 저장된 라운드로 복구
            turn.TurnCount = data.TurnCount;

            // UI 텍스트 갱신
            string roundWord = UnityEngine.Localization.Settings.LocalizationSettings.StringDatabase.GetLocalizedString("Btn_Language", "Round");
            turn.EnemyTurnText.text = $"{roundWord} : {turn.TurnCount}";
        }
        PuzzleSpawner spawner = FindObjectOfType<PuzzleSpawner>();
        PuzzleBoard board = FindObjectOfType<PuzzleBoard>();

        if (spawner != null && data.puzzleData != null)
        {
            board.width = data.puzzleData.width;
            board.height = data.puzzleData.height;

            spawner.SpawnFromSaveData(data.puzzleData);
            board.RebuildDisabledList();
        }

    }

    

}
