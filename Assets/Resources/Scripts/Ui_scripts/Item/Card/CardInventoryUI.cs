using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInventoryUI : MonoBehaviour
{
    public Transform buffGroup; // BuffGroupImage
    public List<Image> activeSlots = new List<Image>();


    public void AddCard(CardSprite card)
    {
        Image slotImage = null;

        // buffGroup 자식들 순회해서 비활성 슬롯 찾기
        foreach (Transform t in buffGroup)
        {
            Image img = t.GetComponent<Image>();
            if (!img.gameObject.activeSelf) // 빈 슬롯
            {
                slotImage = img;
                break;
            }
        }

        if (slotImage == null)
        {
            Debug.LogWarning("빈 슬롯이 없습니다!");
            return;
        }

        // 카드 이미지 적용 및 슬롯 활성화
        slotImage.sprite = card.EffectSprite;
        slotImage.gameObject.SetActive(true);

        // activeSlots 리스트에 추가
        activeSlots.Add(slotImage);
    }
}
