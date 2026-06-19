using UnityEngine.UI;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private GameObject chargeObject;
    [SerializeField] private Transform chargeHolder;
    [SerializeField] private GameObject outlineObject;
    [SerializeField] private Image frame;
    [SerializeField] private Image icon;

    [SerializeField] private Sprite[] frames;

    public void Init(CardData card)
    {
        frame.sprite = frames[(int)card.rarity];
        icon.sprite = card.icon;
        for (int i = 0; i < card.charges; i++)
        {
            Instantiate(chargeObject, chargeHolder).SetActive(card.charges > 1);
        }
    }
    public void AddCharge()
    {
        Instantiate(chargeObject, chargeHolder).SetActive(true);
    }

    public void RemoveCharge()
    {
        Destroy(chargeHolder.transform.GetChild(1).gameObject);
    }

    public void SetSelected(bool state)
    {
        outlineObject.SetActive(state);
    }
}
