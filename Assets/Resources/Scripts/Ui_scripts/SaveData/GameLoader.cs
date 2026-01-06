using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public static void NewGame(int slot)
    {
        SaveContext.Instance.currentSlot = slot;
        SceneManager.LoadScene("GameScene");
    }

    public static void LoadGame(int slot)
    {
        SaveContext.Instance.currentSlot = slot;
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
    }
}
