using UnityEngine;
using UnityEngine.Events;

public class InteractableController : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteract;
    void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    public void OnInteract()
    {
        onInteract.Invoke();
    }

}
