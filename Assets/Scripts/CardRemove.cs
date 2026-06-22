using UnityEngine;

public class CardRemove : MonoBehaviour
{
    [SerializeField] private string dialog;
    public void OnInteract()
    {
        if (string.IsNullOrEmpty(dialog))
            DeckViewController.Instance.ShowDeckView(true);
        else
        {
            DialogManager.Instance.PlayConversation(dialog, () =>
            {
                DeckViewController.Instance.ShowDeckView(true);
            });
        }
    }
}
