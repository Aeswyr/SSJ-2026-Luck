using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject playerPrefab;
    [Space]
    [SerializeField] private GameObject[] levels;
    [SerializeField] private CombatLibrary encounters;
    private LevelController currentLevel;
    private PlayerController player;
    private int levelIndex = 0;
    private int enemyCount;

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
            var levelCon = level.GetComponent<LevelController>();
            if (levelCon.GetLevelType() == type)
            {
                player.transform.position = levelCon.GetSpawn();

                currentLevel = Instantiate(level).GetComponent<LevelController>();
                break;
            }
        }
        levelIndex++;

        List<LevelType> validNextLevels = new (){LevelType.HEAL, LevelType.TREASURE_DUO, 
                                LevelType.TREASURE_RARE, LevelType.TREASURE_UNCOMMON,
                                LevelType.REMOVE_CARD, LevelType.COMBAT, LevelType.COMBAT,
                                LevelType.COMBAT, LevelType.COMBAT};
        if (levelIndex == 8)
        {
            validNextLevels = new (){LevelType.HEAL, LevelType.REMOVE_CARD};
        } else if (levelIndex == 9)
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
            if (validNextLevels.Count != 1)
                validNextLevels.RemoveAt(typeIndex);
        }

        CombatDifficulty difficulty;
        switch (levelIndex)
        {
            case 1:
            case 2:
                difficulty = CombatDifficulty.EASY;
                break;
            case 3:
            case 4:
            case 5:
            case 6:
                difficulty = CombatDifficulty.MEDIUM;
                break;
            case 7:
            case 8:
                difficulty = CombatDifficulty.HARD;
                break;
            case 9:
            case 10:
            default:
                difficulty = CombatDifficulty.NONE;
                break;
        }

        foreach (var door in FindObjectsByType<LevelTransition>(FindObjectsSortMode.None))
            door.SetClosed();
        currentLevel.ToggleRewardsEnabled(false);

        var spawns = currentLevel.GetSpawns();
        if (spawns.Count == 0) 
            return;
            
        var scenarios = encounters.GetScenariosForDifficulty(difficulty);
        var enemies = scenarios[Random.Range(0, scenarios.Count)];

        foreach (var enemy in enemies.spawns)
            Instantiate(encounters.GetEnemyPrefab(enemy.type), spawns[enemy.index], Quaternion.identity, currentLevel.GetObjectParent());
    
        enemyCount = enemies.spawns.Count;


    }

    public LevelController GetCurrentLevel()
    {
        return currentLevel;
    }

    public void OnEnemyKilled()
    {
        enemyCount--;

        if (enemyCount <= 0)
            currentLevel.ToggleRewardsEnabled(true);
    }

    public void OpenAllDoors()
    {
        foreach (var door in FindObjectsByType<LevelTransition>(FindObjectsSortMode.None))
            door.SetOpen();
    }
}
