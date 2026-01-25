using System.Collections.Generic;
using UnityEngine;

public class FieldItemManager : MonoBehaviour
{
    public static FieldItemManager Instance;

    public List<ItemID> aliveItems = new List<ItemID>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(ItemID item)
    {
        if (!aliveItems.Contains(item))
            aliveItems.Add(item);
    }

    public void Unregister(ItemID item)
    {
        aliveItems.Remove(item);
    }

    //저장용 (남아있는 아이템만)
    public List<FieldItemSAVE> GetSaveData()
    {
        List<FieldItemSAVE> list = new List<FieldItemSAVE>();

        foreach (var item in aliveItems)
        {
            if (item == null) continue;

            list.Add(new FieldItemSAVE
            {
                ItemID = item.itemID,
                ItemPos = item.transform.position
            });
        }

        return list;
    }
}
