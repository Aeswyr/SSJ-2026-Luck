using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private SpriteRenderer levelIcon;
    [SerializeField] private LevelType type;
    [SerializeField] private List<Sprite> levelTypeIcons;
    public void OnInteract()
    {
        GameManager.Instance.ToNextLevel(type);
    }

    public void SetType(LevelType type)
    {
        this.type = type;
        levelIcon.sprite = levelTypeIcons[(int)type];
    }
}


