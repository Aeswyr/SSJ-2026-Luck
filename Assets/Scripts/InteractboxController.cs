using System.Collections.Generic;
using UnityEngine;

public class InteractboxController : MonoBehaviour
{
    [SerializeField] private GameObject prompt;
    List<InteractableController> interactables = new();

    void Start()
    {
        prompt.SetActive(interactables.Count > 0);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        interactables.Add(collision.transform.GetComponent<InteractableController>());
        prompt.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        interactables.Remove(collision.transform.GetComponent<InteractableController>());
        if (interactables.Count <= 0)
            prompt.SetActive(false);
    }

    public void FireInteraction()
    {
        if (interactables.Count > 0)
            interactables[0].OnInteract();
    }
}
