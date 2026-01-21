using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    public Image cardImage;
    private bool isEmpty = true;

    private Color32 EmptyColor = new Color32(130, 130, 130, 212);

    // 카드 데이터 저장
    public CardBaseData cardData;

    private void Awake()
    {
        if (cardImage == null)
            cardImage = GetComponent<Image>();

        cardImage.color = EmptyColor;
        isEmpty = true;
        cardData = null;
    }
    public bool IsEmpty()
    {
        return isEmpty;
    }

    public void SetCard(Sprite sprite)
    {
        cardImage.sprite = sprite;
        cardImage.color = Color.white;
        isEmpty = false;
    }

}
