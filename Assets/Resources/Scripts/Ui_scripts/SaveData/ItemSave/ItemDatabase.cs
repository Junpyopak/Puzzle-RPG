using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemEntry
{
    public string itemID;
    public GameObject prefab;
}

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public List<ItemEntry> items = new List<ItemEntry>();

    Dictionary<string, GameObject> itemDict;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        itemDict = new Dictionary<string, GameObject>();
        foreach (var item in items)
        {
            itemDict[item.itemID] = item.prefab;
        }
    }

    public GameObject GetItemPrefab(string itemID)
    {
        if (itemDict.TryGetValue(itemID, out var prefab))
            return prefab;

        Debug.LogError($"ItemDatabase¿¡ ¾ø´Â itemID: {itemID}");
        return null;
    }
}
