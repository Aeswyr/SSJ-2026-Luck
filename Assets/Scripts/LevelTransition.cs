using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private SpriteRenderer levelIcon;
    [SerializeField] private SpriteRenderer door;
    [SerializeField] private LevelType type;
    [SerializeField] private InteractableController interact;
    [SerializeField] private List<Sprite> levelTypeIcons;
    [SerializeField] private Sprite openSprite, closedSprite;
    public void OnInteract()
    {
        GameManager.Instance.ToNextLevel(type);
    }

    public void SetType(LevelType type)
    {
        this.type = type;
        levelIcon.sprite = levelTypeIcons[(int)type];
    }

    public void SetOpen()
    {
        interact.ToggleEnabled(true);
        door.sprite = openSprite;
    }

    public void SetClosed()
    {
        interact.ToggleEnabled(false);
        door.sprite = closedSprite;
    }
}


