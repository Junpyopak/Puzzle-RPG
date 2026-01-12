using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance;
    public List<CardBaseData> cardList = new();
    //List<CardBaseData> cardList = new List<CardBaseData>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadCardBase();
    }

    void LoadCardBase()
    {
        TextAsset csv = Resources.Load<TextAsset>("Data/CardBase");

        if (csv == null)
        {
            Debug.LogError("Resources/Data/CardBase.csv 를 찾을 수 없습니다!");
            return;
        }

        string[] lines = csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] cols = lines[i].Split(',');

            CardBaseData data = new CardBaseData
            {
                CardID = int.Parse(cols[0]),
                CardName = cols[1],
                CardType = cols[2],
                Description = cols[3],
                Grade = cols[4],
                MaxLevel = int.Parse(cols[5])
            };

            cardList.Add(data);
        }

        Debug.Log($"카드 로드 완료: {cardList.Count}장");
    }
}
