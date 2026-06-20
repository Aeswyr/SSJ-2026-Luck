using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardLibrary", menuName = "ScriptableObjects/CardLibrary", order = 1)]
public class CardLibrary : ScriptableObject
{
    [SerializeField] private List<CardData> cards;

    public CardData GetCard(int index)
    {
        return cards[index];
    }

    public CardData GetCard(CardID id)
    {
        return cards[(int)id];
    }

    public List<CardData> GetAllCardsOfRarity(CardRarity rarity)
    {
        List<CardData> selected = new();
        foreach (var card in cards)
            if (card.rarity == rarity)
                selected.Add(card);
        return selected;
    }
}
