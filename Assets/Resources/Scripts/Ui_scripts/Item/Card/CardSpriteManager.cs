using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CardSpriteManager : MonoBehaviour
{
    public static CardSpriteManager instance;
    [SerializeField]
    public List<CardSprite> cardSprite;
    private Dictionary<int , CardSprite> spritesDic;
    // Start is called before the first frame update

    public bool IsLoaded { get; private set; } = false;


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
        if (!IsLoaded)
        {
            Debug.LogWarning("CardSpriteManager 아직 로드 중입니다.");
            return null;
        }

        if (spritesDic.TryGetValue(cardId, out var sprite))
            return sprite;

        Debug.LogWarning($"CardID {cardId}를 찾을 수 없습니다.");
        return null;
        //return spritesDic[cardId];
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //void LoadCardImage()
    //{
    //    // Resources/ImageObject 폴더 안의 모든 CardSprite SO 불러오기
    //    CardSprite[] loadedSO = Resources.LoadAll<CardSprite>("ImageObject");

    //    if (loadedSO == null || loadedSO.Length == 0)
    //    {
    //        Debug.LogWarning("Resources/ImageObject 폴더 안에 불러올 SO가 없습니다.");
    //        return;
    //    }

    //    // CardID 기준으로 순서대로 정렬
    //    List<CardSprite> sortedList = new List<CardSprite>();

    //    // CardID 1부터 시작한다고 가정
    //    int nextID = 1;
    //    while (sortedList.Count < loadedSO.Length)
    //    {
    //        for (int i = 0; i < loadedSO.Length; i++)
    //        {
    //            if (loadedSO[i].CardID == nextID)
    //            {
    //                sortedList.Add(loadedSO[i]);
    //                nextID++;
    //                break; // 다음 ID로 이동
    //            }
    //        }
    //    }

    //    cardSprite = sortedList;

    //    Debug.Log($"불러온 CardSprite 수: {cardSprite.Count}");
    //}
    private void LoadCardImage()
    {
        // Label "CardSprite"로 한꺼번에 로드
        Addressables.LoadAssetsAsync<CardSprite>("CardSprite", null).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                cardSprite = handle.Result.OrderBy(x => x.CardID).ToList();

                // 딕셔너리 생성
                spritesDic = new Dictionary<int, CardSprite>();
                foreach (var cs in cardSprite)
                {
                    if (!spritesDic.ContainsKey(cs.CardID))
                        spritesDic[cs.CardID] = cs;
                    else
                        Debug.LogWarning($"중복된 CardID {cs.CardID} 발견");
                }

                IsLoaded = true;
                Debug.Log($"Addressables에서 불러온 CardSprite 수: {cardSprite.Count}");
            }
            else
            {
                Debug.LogError("Addressables CardSprite 로드 실패");
            }
        };
    }

}
