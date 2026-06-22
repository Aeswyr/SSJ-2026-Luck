using UnityEngine;

public class Conversable : MonoBehaviour
{
    [SerializeField] private string conversationKey;
    public void OnInteract()
    {
        DialogManager.Instance.PlayConversation(conversationKey);
    }
}
