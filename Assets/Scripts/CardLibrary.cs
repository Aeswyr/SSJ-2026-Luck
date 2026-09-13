using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "CardLibrary", menuName = "ScriptableObjects/CardLibrary", order = 1)]
public class CardLibrary : ScriptableObject
{
    [SerializeField] private List<Sprite> cardFrames;
    [SerializeField] private List<CardData> cards;
    private static Dictionary<CardID, CardDescriptionData> cardDescriptions;
    private static Dictionary<KeywordID, KeywordData> keywords;
    public void Load()
    {
        LoadKeywords();
        LoadDescriptions();
    }

    private void LoadKeywords()
    {
        keywords = new();

        TextAsset text = Resources.Load<TextAsset>("SSJ_Luck_26 - Keywords");
        List<string> textLines = new(text.text.Split('\n'));
        textLines.RemoveAt(0);// skip title line
        string line;

        while (textLines.Count > 0)
        {
            line = textLines[0];
            string[] param = line.Split('\t');

            keywords.Add((KeywordID)Enum.Parse(typeof(KeywordID), param[0]), new KeywordData
            {
                name = param[1],
                description = param[2],
                color = param[3]
            });
            textLines.RemoveAt(0);
        }
    }

    private void LoadDescriptions()
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
            cardDescriptions.Add((CardID)Enum.Parse(typeof(CardID), param[0]), ParseCardDescription(param[1]));
            textLines.RemoveAt(0);
        }
    }

    private CardDescriptionData ParseCardDescription(string raw)
    {
        CardDescriptionData data = new()
        {
            description = raw,
            keywords = new()
        };

        foreach (var keyword in keywords)
        {
            string target = $"[{keyword.Value.name}]";
            if (data.description.Contains(target))
            {
                data.keywords.Add(keyword.Key);
                data.description = data.description.Replace(target, $"<{keyword.Value.color.Trim()}>{keyword.Value.name}</color>");
            }
        }

        return data;
    }

    public Sprite GetFrame(CardRarity rarity)
    {
        return cardFrames[(int)rarity];
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

    public CardDescriptionData GetCardDescription(CardID card)
    {
        if (cardDescriptions.ContainsKey(card))
            return cardDescriptions[card];
        return default;
    }

    public string GetKeywordDisplayName(KeywordID id)
    {
        return $"<{keywords[id].color.Trim()}>{keywords[id].name}</color>";
    }

    public string GetKeywordText(KeywordID id)
    {
        return keywords[id].description;
    }

    public List<CardData> GetAllCards(bool unlockedOnly = false) {
        return cards;
    }

    public struct KeywordData
    {
        public string name;
        public string description;
        public string color;
    }
}
