using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string GetPath(int slot)
    {
        return Path.Combine(
            Application.persistentDataPath,
            $"save_slot_{slot}.json"
        );
    }

    // 저장 파일 존재 여부
    public static bool HasSave(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    // 저장
    public static void Save(int slot, SaveData data)
    {
        string path = GetPath(slot);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log($"[SaveManager] Saved slot {slot} : {path}");
    }

    // 불러오기
    public static SaveData Load(int slot)
    {
        string path = GetPath(slot);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveManager] No save file in slot {slot}");
            return null;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        Debug.Log($"[SaveManager] Loaded slot {slot}");
        return data;
    }

    // 삭제 (새 게임 덮어쓰기용)
    public static void Delete(int slot)
    {
        string path = GetPath(slot);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveManager] Deleted slot {slot}");
        }
    }
}

