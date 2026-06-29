using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private LevelType levelType;
    [SerializeField] private Vector2 spawn;
    [SerializeField] private GameObject levelObjects;
    [SerializeField] private Vector2 levelBounds;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private List<GameObject> rewards;
    [SerializeField] private List<ParallaxInfo> parallaxes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnParent.gameObject.SetActive(false);

        foreach (var cam in FindObjectsByType<CameraFollow>(FindObjectsSortMode.None))
        {
            cam.SetBounds(levelBounds);
        }

        ParallaxManager.Instance.SetParallax(parallaxes);
    }

    public Vector2 GetSpawn()
    {
        return spawn;
    }

    public Transform GetObjectParent()
    {
        return levelObjects.transform;
    }

    public LevelType GetLevelType()
    {
        return levelType;    
    }

    public List<Vector3> GetSpawns()
    {
        List<Vector3> positions = new();

        foreach (Transform child in spawnParent)
        {
            positions.Add(child.position);
        }

        return positions;
    }


    public void ToggleRewardsEnabled(bool enabled)
    {
        foreach (var obj in rewards)
            obj?.SetActive(enabled);
    }
}

public enum LevelType
{
    INTRO, COMBAT, BOSS, HEAL, TREASURE_DUO, TREASURE_RARE, TREASURE_UNCOMMON, REMOVE_CARD
}
