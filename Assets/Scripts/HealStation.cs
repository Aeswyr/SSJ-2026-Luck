using UnityEngine;

public class HealStation : MonoBehaviour
{
    public void OnInteract()
    {
        var player = FindAnyObjectByType<PlayerController>();
    }
}
