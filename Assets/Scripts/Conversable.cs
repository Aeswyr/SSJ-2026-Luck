using UnityEngine;
using UnityEngine.Events;

public class Conversable : MonoBehaviour
{
    [SerializeField] private string conversationKey;
    [SerializeField] private UnityEvent callback;
    public void OnInteract()
    {
        DialogManager.Instance.PlayConversation(conversationKey, () => {callback?.Invoke();});
    }
}
