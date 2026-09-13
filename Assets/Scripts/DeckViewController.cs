using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewController : Singleton<DeckViewController>
{
    [SerializeField] private CardLibrary cardLibrary;
    [SerializeField] private GameObject viewParent;
    [SerializeField] private GameObject removeButton;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private CardDisplay card;

    private bool isRemoval;
    private CardID selectedCard;


    void Start()
    {
        viewParent.SetActive(false);
        removeButton.SetActive(false);
    }

    public void ShowDeckView(bool canRemoveCard, List<CardID> deckOverride = null)
    {
        Time.timeScale = 0;

        isRemoval = canRemoveCard;
        title.text = isRemoval ? "Choose a Card to Remove" : "Your Deck";

        PopulateDeck(deckOverride);

        removeButton.SetActive(false);
        viewParent.SetActive(true);
    }

    private void PopulateDeck(List<CardID> deckOverride)
    {
        for (int i = cardParent.childCount - 1; i >= 0; i--)
        {
            Destroy(cardParent.GetChild(i).gameObject);
        }

        var player = FindAnyObjectByType<PlayerController>();

        var cards = deckOverride != null ? deckOverride : player.GetBaselineDeck();
        cards.Sort(SortByRarity);
        int SortByRarity(CardID a, CardID b)
        {
            if (cardLibrary.GetCard(a).rarity < cardLibrary.GetCard(b).rarity)
                return -1;
            if (cardLibrary.GetCard(a).rarity > cardLibrary.GetCard(b).rarity)
                return 1;
            return 0;
        }

        foreach (var id in cards) {
            var card = Instantiate(cardPrefab, cardParent);
            card.GetComponent<CardController>().Init(cardLibrary.GetCard(id));
            card.GetComponent<Button>().onClick.AddListener(delegate {OnCardPressed(id);});
        }

        card.SetCard(cards[0]);
    }

    private void OnCardPressed(CardID id)
    {
        selectedCard = id;
        
        if (isRemoval)
        {
            removeButton.SetActive(true);
        }

        card.SetCard(id);
    }

    public void OnRemovePressed()
    {
        var player = FindAnyObjectByType<PlayerController>();

        player.RemoveCardFromDeck(selectedCard);

        Close();
    }

    public void Close()
    {
        viewParent.SetActive(false);
        Time.timeScale = 1;

        InputHandler.Instance.FlushBuffer();
    }
}
