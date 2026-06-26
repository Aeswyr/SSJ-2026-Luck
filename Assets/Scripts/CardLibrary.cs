using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CardLibrary", menuName = "ScriptableObjects/CardLibrary", order = 1)]
public class CardLibrary : ScriptableObject
{
    [SerializeField] private List<CardData> cards;
    private static Dictionary<CardID, string> cardDescriptions;

    public void Load()
    {
        cardDescriptions = new();

        TextAsset text = Resources.Load<TextAsset>("SSJ_Luck_26 - CardDescriptions");
        List<string> textLines = new(text.text.Split('\n'));
        textLines.RemoveAt(0);// skip title line
        string line;

        while (textLines.Count > 0)
        {
            line = textLines[0];
            string[] param = line.Split('\t');
            cardDescriptions.Add((CardID)Enum.Parse(typeof(CardID), param[0]), param[1]);
            textLines.RemoveAt(0);
        }
    }

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

    public string GetCardDescription(CardID card)
    {
        if (cardDescriptions.ContainsKey(card))
            return cardDescriptions[card];
        return "TEXT NOT FOUND";
    }
}
