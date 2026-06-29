using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatLibrary", menuName = "ScriptableObjects/CombatLibrary")]
public class CombatLibrary : ScriptableObject
{
    [SerializeField] private List<EnemyData> enemies;
    [SerializeField] private List<CombatScenario> scenarios;

    public List<CombatScenario> GetScenariosForDifficulty(CombatDifficulty difficulty)
    {
        List<CombatScenario> list = new();

        foreach (var scenario in scenarios)
            if (scenario.difficulty == difficulty)
                list.Add(scenario);

        return list;
    }

    public GameObject GetEnemyPrefab(EnemyType type)
    {
        foreach (var enemy in enemies)
            if (enemy.type == type)
                return enemy.prefab;
        return null;
    }
}

[Serializable] public struct CombatScenario
{
    public CombatDifficulty difficulty;
    public List<CombatSpawn> spawns;
}
[Serializable] public struct CombatSpawn
{
    public EnemyType type;
    public int index;
}

[Serializable] public struct EnemyData
{
    public EnemyType type;
    public GameObject prefab;
}
public enum EnemyType
{
    ANGLER, HOLLOW, SALAMANDER, IMP
}

public enum CombatDifficulty
{
    EASY, MEDIUM, HARD, VERYHARD, NONE
}