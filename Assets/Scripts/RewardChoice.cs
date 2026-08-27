using System.Collections.Generic;
using UnityEngine;

public class RewardChoice : MonoBehaviour
{
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private LootTable lootTable;

    [SerializeField] private Sprite[] frameSprites;
    [SerializeField] private SpriteRenderer[] cardOptions;
    [SerializeField] private SpriteRenderer[] cardFrames;
    [SerializeField] private GameObject node;

    [SerializeField] private string dialog;
    private CardData[] rewards = new CardData[3];

    public void Start()
    {
        var lootRoll = lootTable.RollReward(rewards.Length);

        var cardsCommon = cardLibrary.GetAllCardsOfRarity(CardRarity.COMMON);
        var cardsUncommon = cardLibrary.GetAllCardsOfRarity(CardRarity.UNCOMMON);
        var cardsRare = cardLibrary.GetAllCardsOfRarity(CardRarity.RARE);
        for (int i = 0; i < rewards.Length; i++)
        {
            ref List<CardData> selection = ref cardsCommon;
            switch (lootRoll.rewards[i])
            {
                case CardRarity.UNCOMMON:
                    selection = ref cardsUncommon;
                    break;
                case CardRarity.RARE:
                    selection = ref cardsRare;
                    break;
            }
            int index = Random.Range(0, selection.Count);
            rewards[i] = selection[index];
            selection.RemoveAt(index);

            cardOptions[i].sprite = rewards[i].icon;
            cardFrames[i].sprite = frameSprites[(int)rewards[i].rarity];
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
        FinalizeRewardChoice();
    }

    public void ChooseRewardB()
    {
        FindAnyObjectByType<PlayerController>().AddCardToDeck(rewards[1].id);
        FinalizeRewardChoice();
    }

    public void ChooseRewardC()
    {
        FindAnyObjectByType<PlayerController>().AddCardToDeck(rewards[2].id);
        FinalizeRewardChoice();
    }

    private void FinalizeRewardChoice()
    {
        node.SetActive(false);
        foreach (var card in cardOptions)
            card.gameObject.SetActive(false);

        FindAnyObjectByType<PlayerHUDController>().HideCardPreview();
    }

    public void HoverRewardA()
    {
        FindAnyObjectByType<PlayerHUDController>().ShowCardPreview(rewards[0].id);
    }

    public void HoverRewardB()
    {
        FindAnyObjectByType<PlayerHUDController>().ShowCardPreview(rewards[1].id);

    }

    public void HoverRewardC()
    {
        FindAnyObjectByType<PlayerHUDController>().ShowCardPreview(rewards[2].id);
    }

    public void EndHover()
    {
        FindAnyObjectByType<PlayerHUDController>().HideCardPreview();
    }
}
