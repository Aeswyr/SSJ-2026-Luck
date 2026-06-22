using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    [Space]
    [SerializeField] private GameObject[] levels;
    private LevelController currentLevel;
    private PlayerController player;
    private int levelIndex = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentLevel = FindAnyObjectByType<LevelController>();
    
        player = Instantiate(playerPrefab, currentLevel.GetSpawn(), Quaternion.identity).GetComponent<PlayerController>();
    }


    public void ToNextLevel(LevelType type)
    {
        LevelType lastLevel = currentLevel.GetLevelType();
        Destroy(currentLevel.gameObject);

        player.ResetToBaseline();


        List<GameObject> levelList = new(levels);
        for (int i = 0; i < levelList.Count; i++)
        {
            int index = Random.Range(i, levelList.Count);
            GameObject level = levelList[index];
            levelList.RemoveAt(index);
            levelList.Insert(0, level);
        }

        foreach (var level in levelList)
        {
            if (level.GetComponent<LevelController>().GetLevelType() == type)
            {
                currentLevel = Instantiate(level).GetComponent<LevelController>();
                break;
            }
        }
        levelIndex++;

        List<LevelType> validNextLevels = new (){LevelType.HEAL, LevelType.TREASURE_DUO, 
                                LevelType.TREASURE_RARE, LevelType.TREASURE_UNCOMMON,
                                LevelType.REMOVE_CARD, LevelType.COMBAT, LevelType.COMBAT,
                                LevelType.COMBAT, LevelType.COMBAT};
        if (levelIndex == 5)
        {
            validNextLevels = new (){LevelType.HEAL};
        } else if (levelIndex == 6)
        {
            validNextLevels = new (){LevelType.BOSS};
        }
        else if (type == lastLevel && type == LevelType.COMBAT)
        {
            validNextLevels = new (){LevelType.HEAL, LevelType.TREASURE_DUO, 
                                LevelType.TREASURE_RARE, LevelType.TREASURE_UNCOMMON,
                                LevelType.REMOVE_CARD};
        } else if (type != LevelType.COMBAT)
        {
            validNextLevels = new (){LevelType.COMBAT};
        }

        foreach (var door in FindObjectsByType<LevelTransition>(FindObjectsSortMode.None))
        {
            int typeIndex = Random.Range(0, validNextLevels.Count);
            door.SetType(validNextLevels[typeIndex]);
            if (validNextLevels.Count > 1)
                validNextLevels.RemoveAt(typeIndex);
        }
    }

    public LevelController GetCurrentLevel()
    {
        return currentLevel;
    }
}
