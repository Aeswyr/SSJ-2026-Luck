using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffLibrary", menuName = "ScriptableObjects/BuffLibrary", order = 1)]
public class BuffLibrary : ScriptableObject
{
    [SerializeField] private List<BuffData> buffs;

    public BuffData GetBuff(BuffType id)
    {
        return buffs[(int)id];
    }
}
