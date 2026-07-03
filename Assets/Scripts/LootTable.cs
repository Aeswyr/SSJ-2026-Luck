using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "ScriptableObjects/LootTable")]
public class LootTable : ScriptableObject
{
    [SerializeField] private bool sameRarity;
    [SerializeField] private int uncommonChance, rareChance;

    public LootResult RollReward(int num)
    {
        LootResult res = new();
        if (sameRarity) {
            var roll = Roll();
            for (int i = 0; i < num; i++)
            {
                res.rewards.Add(roll);
            }
        } else
        {
            for (int i = 0; i < num; i++)
            res.rewards.Add(Roll());
        }
        return res;
    }

    private CardRarity Roll()
    {
        var result = Random.Range(0, 100);
        if (result < rareChance)
            return CardRarity.RARE;
        else if (result < rareChance + uncommonChance)
            return CardRarity.UNCOMMON;

        return CardRarity.COMMON;
    }
}

public class LootResult
{
    public List<CardRarity> rewards = new();
}
