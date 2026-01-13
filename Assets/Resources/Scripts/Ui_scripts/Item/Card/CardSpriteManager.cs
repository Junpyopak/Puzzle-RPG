using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class CardSpriteManager : MonoBehaviour
{
    public static CardSpriteManager instance;
    [SerializeField]
    private List<CardSprite> cardSprite;
    private Dictionary<int , CardSprite> spritesDic;
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadCardImage();

        spritesDic = new Dictionary<int , CardSprite>();
        foreach (var v in cardSprite)
        {
            spritesDic[v.CardID] = v;
        }
    }
    public CardSprite GetVisual(int cardId)
    {
        return spritesDic[cardId];
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    void LoadCardImage()
    {
        // Resources/ImageObject 폴더 안의 모든 CardSprite SO 불러오기
        CardSprite[] loadedSO = Resources.LoadAll<CardSprite>("ImageObject");

        if (loadedSO == null || loadedSO.Length == 0)
        {
            Debug.LogWarning("Resources/ImageObject 폴더 안에 불러올 SO가 없습니다.");
            return;
        }

        // CardID 기준으로 순서대로 정렬
        List<CardSprite> sortedList = new List<CardSprite>();

        // CardID 1부터 시작한다고 가정
        int nextID = 1;
        while (sortedList.Count < loadedSO.Length)
        {
            for (int i = 0; i < loadedSO.Length; i++)
            {
                if (loadedSO[i].CardID == nextID)
                {
                    sortedList.Add(loadedSO[i]);
                    nextID++;
                    break; // 다음 ID로 이동
                }
            }
        }

        cardSprite = sortedList;

        Debug.Log($"불러온 CardSprite 수: {cardSprite.Count}");
    }
}
