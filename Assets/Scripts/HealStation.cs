using UnityEngine;

public class HealStation : MonoBehaviour
{   
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Sprite usedSprite;
    public void OnInteract()
    {
        var entity = FindAnyObjectByType<PlayerController>().GetComponent<EntityController>();
        if (entity.IsMaxHealth())
            entity.AdjustMaxHealth(1);
        entity.ApplyHealing(2);

        sprite.sprite = usedSprite;
    }
}
