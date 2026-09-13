using System.Collections.Generic;
using UnityEngine;

public class TomeInteractable : MonoBehaviour
{
    [SerializeField] private CardLibrary cardLibrary;
    public void OnInteract()
    {
        var cards = cardLibrary.GetAllCards(true);

        List<CardID> cardIDs = new();
        foreach (var card in cards)
            cardIDs.Add(card.id);

        DeckViewController.Instance.ShowDeckView(false, cardIDs);
    }
}
