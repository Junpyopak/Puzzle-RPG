using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSlot : MonoBehaviour
{
    public Image cardImage;
    private bool isEmpty = true;
    public TMP_Text cardNameText;
    public TMP_Text cardInfoText;

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

    public void SetCard(Sprite sprite,CardBaseData data)
    {
       cardData = data;   //데이터 저장
        cardImage.sprite = sprite;
        // 원본 크기로 설정
        cardImage.SetNativeSize();

        // 원하는 만큼 확대 (예: 2배)
        cardImage.rectTransform.localScale = Vector3.one * 2.0f;

        cardImage.color = Color.white;
        if (cardNameText != null)
            cardNameText.text = data.CardName;

        if (cardInfoText != null)
            cardInfoText.text = data.Description;

        isEmpty = false;
    }

}
