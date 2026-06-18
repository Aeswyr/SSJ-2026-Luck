using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDController : MonoBehaviour
{
    [SerializeField] private Transform cardHolder;
    [SerializeField] private GameObject cardPrefab;

    List<CardController> cards = new();
    int activeIndex;
    public void DrawCard(CardData cardData)
    {
        var card = Instantiate(cardPrefab, cardHolder).GetComponent<CardController>();
        card.AddCharges(cardData.charges);
        card.SetSelected(false);
        cards.Add(card);
    }

    public void DiscardIndex(int index)
    {
        Destroy(cards[index].gameObject);
        cards.RemoveAt(index);
    }

    public void DiscardAll()
    {
        foreach (var card in cards)
            Destroy(card.gameObject);
        
        cards.Clear();
    }

    public void SetIndexSelected(int index)
    {
        if (index < 0)
            return;

        foreach (var card in cards)
            card.SetSelected(false);
            
        activeIndex = index;
        cards[activeIndex].SetSelected(true);
    }

    public void UseCharge()
    {
        cards[activeIndex].RemoveCharge();
    }
}
