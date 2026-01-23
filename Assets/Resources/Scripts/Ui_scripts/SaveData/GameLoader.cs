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

        Player player = FindObjectOfType<Player>();
        player.ApplySaveData(data);

        // 타이머 복구 (이게 핵심)
        UI_GameTimer timer = FindObjectOfType<UI_GameTimer>();
        if (timer != null)
        {
            timer.SetTime(data.gameTime);
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

        if (SaveContext.Instance.isLoading)
        {
            // 기본 배치 아이템 제거
            foreach (var item in FindObjectsOfType<ItemID>())
            {
                Destroy(item.gameObject);
            }

            // 저장된 아이템 재생성
            if (data.fieldItems != null && ItemDatabase.Instance != null)
            {
                foreach (var savedItem in data.fieldItems)
                {
                    GameObject prefab =
                        ItemDatabase.Instance.GetItemPrefab(savedItem.ItemID);

                    if (prefab == null)
                    {
                        Debug.LogWarning($"프리팹 없음: {savedItem.ItemID}");
                        continue;
                    }

                    Instantiate(
                        prefab,
                        savedItem.ItemPos,
                        Quaternion.identity
                    );
                }
            }
        }
    }
}
