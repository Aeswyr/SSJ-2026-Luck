using System.Collections.Generic;
using UnityEngine;

public class RewardChoice : MonoBehaviour
{
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private CardRarity rarity;

    [SerializeField] private Sprite[] frameSprites;
    [SerializeField] private SpriteRenderer[] cardOptions;
    [SerializeField] private SpriteRenderer[] cardFrames;
    [SerializeField] private GameObject node;

    [SerializeField] private bool useDialog;
    private CardData[] rewards = new CardData[3];

    public void Start()
    {
        int roll = Random.Range(0, 100);

        if (roll < 50) 
            rarity = CardRarity.COMMON;
        else if (roll < 85) 
            rarity = CardRarity.UNCOMMON;
        else 
            rarity = CardRarity.RARE;
        
        foreach (var frame in cardFrames)
            frame.sprite = frameSprites[(int)rarity];

        var cardSelection = cardLibrary.GetAllCardsOfRarity(rarity);
        for (int i = 0; i < rewards.Length; i++)
        {
            int index = Random.Range(0, cardSelection.Count);
            rewards[i] = cardSelection[index];
            cardSelection.RemoveAt(index);

            cardOptions[i].sprite = rewards[i].icon;
        }
        
        node.SetActive(true);
        foreach (var card in cardOptions)
            card.gameObject.SetActive(false);
    }

    public void OpenReward()
    {
        if (useDialog)
            DialogManager.Instance.PlayConversation($"cardselecthi{Random.Range(1, 5)}");

        node.SetActive(false);

        foreach (var card in cardOptions)
            card.gameObject.SetActive(true);
    }

    public void ChooseRewardA()
    {
        FindAnyObjectByType<PlayerController>().AddCardToDeck(rewards[0].id);
        node.SetActive(false);
        foreach (var card in cardOptions)
            card.gameObject.SetActive(false);

        if (useDialog)
            DialogManager.Instance.PlayConversation($"cardselectbye{Random.Range(1, 5)}");
    }

    public void ChooseRewardB()
    {
        FindAnyObjectByType<PlayerController>().AddCardToDeck(rewards[1].id);
        node.SetActive(false);
        foreach (var card in cardOptions)
            card.gameObject.SetActive(false);

        if (useDialog)
            DialogManager.Instance.PlayConversation($"cardselectbye{Random.Range(1, 5)}");
    }

    public void ChooseRewardC()
    {
        FindAnyObjectByType<PlayerController>().AddCardToDeck(rewards[2].id);
        node.SetActive(false);
        foreach (var card in cardOptions)
            card.gameObject.SetActive(false);

        if (useDialog)
            DialogManager.Instance.PlayConversation($"cardselectbye{Random.Range(1, 5)}");
    }
}
