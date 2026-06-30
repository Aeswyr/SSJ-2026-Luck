using UnityEngine;

public class CardStickable : MonoBehaviour
{
    [SerializeField] private ParticleSystem cardVisual;
    public int cards
    {
        get; private set;
    }
    private int lastEmit;

    public void StickCard()
    {
        cards++;
        if (cards - lastEmit >= 2)
        {
            lastEmit = cards;
            cardVisual.Emit(1);
        }
        
    }

    public void ClearCards()
    {
        cards = 0;
        lastEmit = 0;
        cardVisual.Clear();
    }
}
