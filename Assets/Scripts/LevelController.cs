using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private Vector2 spawn;
    [SerializeField] private GameObject levelObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var player = FindAnyObjectByType<PlayerController>();
        if (player != null)
            player.transform.position = spawn;
    }

    public Vector2 GetSpawn()
    {
        return spawn;
    }

    public Transform GetObjectParent()
    {
        return levelObjects.transform;
    }
}
