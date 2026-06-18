using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private GameObject chargeObject;
    [SerializeField] private Transform chargeHolder;
    [SerializeField] private GameObject outlineObject;

    public void AddCharges(int amt)
    {
        for (int i = 0; i < amt; i++)
        {
            Instantiate(chargeObject, chargeHolder).SetActive(true);
        }
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
