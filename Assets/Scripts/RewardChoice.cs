using System.Collections.Generic;
using UnityEngine;

public class RewardChoice : MonoBehaviour
{
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private LootTable lootTable;

    [SerializeField] private SpriteRenderer[] cardOptions;
    [SerializeField] private SpriteRenderer[] cardFrames;
    [SerializeField] private GameObject node;

    [SerializeField] private string dialog;

    [SerializeField] private GameObject vfxPrefab;
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
            cardFrames[i].sprite = cardLibrary.GetFrame(rewards[i].rarity);
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
        FinalizeRewardChoice(0);
    }

    public void ChooseRewardB()
    {
        FinalizeRewardChoice(1);
    }

    public void ChooseRewardC()
    {
        FinalizeRewardChoice(2);
    }

    private void FinalizeRewardChoice(int index)
    {
        FindAnyObjectByType<PlayerController>().AddCardToDeck(rewards[index].id);

        for (int i = 0; i < cardFrames.Length; i++)
        {
            var vfx = Instantiate(vfxPrefab).GetComponent<CardSelectVFX>(); 
            vfx.transform.position = cardFrames[i].transform.position;
            vfx.Init(rewards[i].id, i == index);
        }
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
