using UnityEngine;

public class CardStickable : MonoBehaviour
{
    public int cards
    {
        get; private set;
    }

    public void StickCard()
    {
        cards++;
    }

    public void ClearCards()
    {
        cards = 0;
    }
}
