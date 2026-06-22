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

    [SerializeField] private string dialog;
    [SerializeField] private CardTooltip tooltip;
    private CardData[] rewards = new CardData[3];

    public void Start()
    {
        tooltip.gameObject.SetActive(false);

        if (rarity == CardRarity.NONE) {
            int roll = Random.Range(0, 100);

            if (roll < 5) 
                rarity = CardRarity.RARE;
        }     

        if (rarity != CardRarity.NONE){
            var cardSelection = cardLibrary.GetAllCardsOfRarity(rarity);
            for (int i = 0; i < rewards.Length; i++)
            {
                int index = Random.Range(0, cardSelection.Count);
                rewards[i] = cardSelection[index];
                cardSelection.RemoveAt(index);

                cardOptions[i].sprite = rewards[i].icon;
                cardFrames[i].sprite = frameSprites[(int)rewards[i].rarity];
            }
        } else
        {
            var commonSelection = cardLibrary.GetAllCardsOfRarity(CardRarity.COMMON);
            var uncommonSelection = cardLibrary.GetAllCardsOfRarity(CardRarity.UNCOMMON);

            for (int i = 0; i < rewards.Length; i++)
            {
                var cardSelection = (Random.Range(0, 100) < 80) ? ref commonSelection : ref uncommonSelection;
                int index = Random.Range(0, cardSelection.Count);
                rewards[i] = cardSelection[index];
                cardSelection.RemoveAt(index);

                cardOptions[i].sprite = rewards[i].icon;
                cardFrames[i].sprite = frameSprites[(int)rewards[i].rarity];
            }
        }
        
        node.SetActive(true);
        foreach (var card in cardOptions)
            card.gameObject.SetActive(false);
    }

    public void OpenReward()
    {
        if (string.IsNullOrEmpty(dialog))
            FinishOpen();
        else
            DialogManager.Instance.PlayConversation(dialog, FinishOpen);
            
        void FinishOpen() {
            node.SetActive(false);

            foreach (var card in cardOptions)
                card.gameObject.SetActive(true);
        }
    }

    public void ChooseRewardA()
    {
        FindAnyObjectByType<PlayerController>().AddCardToDeck(rewards[0].id);
        node.SetActive(false);
        foreach (var card in cardOptions)
            card.gameObject.SetActive(false);

        tooltip.gameObject.SetActive(false);
    }

    public void ChooseRewardB()
    {
        FindAnyObjectByType<PlayerController>().AddCardToDeck(rewards[1].id);
        node.SetActive(false);
        foreach (var card in cardOptions)
            card.gameObject.SetActive(false);

        tooltip.gameObject.SetActive(false);
    }

    public void ChooseRewardC()
    {
        FindAnyObjectByType<PlayerController>().AddCardToDeck(rewards[2].id);
        node.SetActive(false);
        foreach (var card in cardOptions)
            card.gameObject.SetActive(false);

        tooltip.gameObject.SetActive(false);
    }

    public void HoverRewardA()
    {
        tooltip.LoadCard(rewards[0].id);

        tooltip.transform.localPosition = new Vector2(-4, tooltip.transform.localPosition.y);
        tooltip.gameObject.SetActive(true);
    }

    public void HoverRewardB()
    {
        tooltip.LoadCard(rewards[1].id);

        tooltip.transform.localPosition = new Vector2(0, tooltip.transform.localPosition.y);
        tooltip.gameObject.SetActive(true);
    }

    public void HoverRewardC()
    {
        tooltip.LoadCard(rewards[2].id);

        tooltip.transform.localPosition = new Vector2(4, tooltip.transform.localPosition.y);
        tooltip.gameObject.SetActive(true);
    }

    public void EndHover()
    {
        tooltip.gameObject.SetActive(false);
    }
}
