using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Card/CardVisual")]
public class CardSprite :ScriptableObject
{
    public int CardID;
    public Sprite Sprite;

    [Header("카드 획득 이펙트 이미지")]
    public Sprite EffectSprite; // 상단에 표시할 이펙트용
}
