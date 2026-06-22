using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[CreateAssetMenu(fileName = "CardLibrary", menuName = "ScriptableObjects/CardLibrary", order = 1)]
public class CardLibrary : ScriptableObject
{
    [SerializeField] private string path;
    [SerializeField] private List<CardData> cards;
    private static Dictionary<CardID, string> cardDescriptions;

    public void Load()
    {
        cardDescriptions = new();
        StreamReader dialogReader = new StreamReader(path);
        dialogReader.ReadLine();// skip title line
        string line;

        while ((line = dialogReader.ReadLine()) != null)
        {
            string[] param = line.Split('\t');
            cardDescriptions.Add((CardID)Enum.Parse(typeof(CardID), param[0]), param[1]);
        }

        dialogReader.Close();
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
        return cardDescriptions[card];
    }
}
