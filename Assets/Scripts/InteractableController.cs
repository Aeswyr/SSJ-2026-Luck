using UnityEngine;
using UnityEngine.Events;

public class InteractableController : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private UnityEvent onEnter, onExit;

    [SerializeField] private Collider2D col;
    [SerializeField] private bool singleUse;
    [SerializeField] private bool levelEndInteractable;
    [SerializeField] private bool canInteract = true;
    public bool CanInteract => canInteract;
    void OnTriggerEnter2D(Collider2D collision)
    {
        onEnter?.Invoke();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        onExit?.Invoke();
    }

    public void OnInteract()
    {
        onInteract.Invoke();

        if (singleUse)
            col.enabled = false;

        if (levelEndInteractable)
            GameManager.Instance.OpenAllDoors();
    }

    public void ToggleEnabled(bool enabled)
    {
        col.enabled = enabled;
    }
}
