using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public bool isOpen = false;
    public float cardCloseAnimationDuration = 1.0f;

    [Header("생성된 카드들 관리")]
    private List<GameObject> instantiatedCards = new List<GameObject>();

    [Header("희귀도 확률 설정")]
    [Range(0f, 1f)]
    public float legendChance = 0.2f;

    [Header("등급별 카드 풀")]
    public List<GameObject> generalCardPool;
    public List<GameObject> legendCardPool;

    [Header("카드가 생성될 위치")]
    public Transform[] spawnPoints;

    [Header("플레이어가 획득한 카드 이름 리스트")]
    public List<string> selectCardNames = new List<string>();

    [Header("TwoPick 관련")]
    [SerializeField] public bool twoPick = false;
    [SerializeField] public float twoPickValue = 0f;

    public void CardRarityOpen()
    {
        if (isOpen) return;
        isOpen = true;

        List<GameObject> tempGeneralPool = new List<GameObject>(generalCardPool);
        List<GameObject> tempLegendPool = new List<GameObject>(legendCardPool);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Card.CardRarity rarity = (Random.value < legendChance) ? Card.CardRarity.Legend : Card.CardRarity.General;
            List<GameObject> targetPool = (rarity == Card.CardRarity.Legend) ? tempLegendPool : tempGeneralPool;

            //만약 해당 등급의 카드가 풀에 더 이상 없다면 반대편 풀에서 가져옴
            if (targetPool.Count == 0)
            {
                targetPool = (rarity == Card.CardRarity.Legend) ? tempGeneralPool : tempLegendPool;
                rarity = (rarity == Card.CardRarity.Legend) ? Card.CardRarity.General : Card.CardRarity.Legend;
            }

            if (targetPool.Count > 0)
            {
                int randomIndex = Random.Range(0, targetPool.Count);
                GameObject selectedPrefab = targetPool[randomIndex];

                GameObject newCard = Instantiate(selectedPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
                instantiatedCards.Add(newCard);

                Card cardScript = newCard.GetComponent<Card>();
                //if (cardScript != null) cardScript.rarity = rarity;
                if (cardScript != null)
                {
                    cardScript.rarity = rarity;

                    // CardID 기반 데이터 로드
                    cardScript.LoadData();

                    // cardName과 cardToolTip 세팅
                    if (cardScript.cardData != null)
                    {
                        cardScript.cardName = cardScript.cardData.CardName;
                        cardScript.cardToolTip = cardScript.cardData.Description;
                    }
                }

                //사용한 카드 임시풀에서 제거
                targetPool.RemoveAt(randomIndex);
            }
        }
    }

    //지금은 딱히 안쓰는중
    private GameObject GetRandomCardFromPool(Card.CardRarity rarity)
    {
        if (rarity == Card.CardRarity.Legend)
        {
            return (legendCardPool.Count > 0) ? legendCardPool[Random.Range(0, legendCardPool.Count)] : null; //삼항 연산자
        }
        else
        {
            return (generalCardPool.Count > 0) ? generalCardPool[Random.Range(0, generalCardPool.Count)] : null;
        }
    }

    public void AddSelectCard(GameObject clickedCardInstance)
    {
        // Instantiate 시 붙는 "(Clone)" 문구 제거
        string cleanName = clickedCardInstance.name.Replace("(Clone)", "").Trim();

        selectCardNames.Add(cleanName);

        Card card = clickedCardInstance.GetComponent<Card>();

        if (card != null)
        {
            //도감에 클릭한 카드추가
            CardGainDataHolder.Instance.Data.AddCard(card.cardData.CardID);

            PlayerCardManager pcm = FindObjectOfType<PlayerCardManager>();

            if (pcm != null)
            {
                pcm.GainCard(card.cardData);
            }
        }

        Debug.Log($"플레이어 카드 리스트에 이름 저장 완료: {cleanName}");
    }

    public void CloseOtherCards(GameObject clickedCard)
    {
        foreach (GameObject card in instantiatedCards)
        {
            if (card == null) continue;
            Animator ani = card.GetComponent<Animator>();
            if (ani != null && card != clickedCard) ani.SetTrigger("CardClose");
        }
        StartCoroutine(WaitForAnimationsAndResetRoutine());
    }

    IEnumerator WaitForAnimationsAndResetRoutine()
    {
        yield return new WaitForSeconds(cardCloseAnimationDuration); //카드 닫히는 시간만큼 기다리기
        ResetCardList(); //카드 전체 리스트를 지우는 함수 실행
    }

    public void ResetCardList()
    {
        isOpen = false;
        foreach (GameObject card in instantiatedCards) if (card != null) Destroy(card);
        instantiatedCards.Clear();
    }

    //저장된 이름의 프리팹 가져오기
    public GameObject GetPrefabByName(string cardName)
    {
        // 리스트에서 이름이 일치하는 프리팹 찾기
        GameObject prefab = generalCardPool.Find(p => p.name == cardName);
        if (prefab == null) prefab = legendCardPool.Find(p => p.name == cardName);

        return prefab;
    }

    public void ClearSelectedCards()
    {
        selectCardNames.Clear();   // 선택 카드 이름 초기화
        Debug.Log("새 게임 → CardManager 선택 카드 초기화");
    }
}
