using UnityEngine;
using UnityEngine.Events;

public class TriggerCallback : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider2D> action;

    void OnTriggerEnter2D(Collider2D collision)
    {
        action.Invoke(collision);
    }
}
