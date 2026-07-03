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
    [SerializeField] private CardTooltip tooltip;
    private CardData[] rewards = new CardData[3];

    public void Start()
    {
        tooltip.gameObject.SetActive(false);

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
