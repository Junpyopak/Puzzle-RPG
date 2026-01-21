using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardGainData", menuName = "Game/CardGainData")]
public class CardGainData : ScriptableObject
{
    public List<int> gainedCardIDs = new List<int>();

    public void AddCard(int cardId)
    {
        if (!gainedCardIDs.Contains(cardId))
            gainedCardIDs.Add(cardId);
    }

    public void Clear()
    {
        gainedCardIDs.Clear();
    }
}
