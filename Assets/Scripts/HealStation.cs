using UnityEngine;

public class HealStation : MonoBehaviour
{   
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite usedSprite;
    public void OnInteract()
    {
        var player = FindAnyObjectByType<PlayerController>();
        player.GetComponent<EntityController>().ApplyHealing(1);

        sprite.sprite = usedSprite;
    }
}
