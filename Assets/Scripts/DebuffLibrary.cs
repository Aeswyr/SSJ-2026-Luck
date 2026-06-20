using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DebuffLibrary", menuName = "ScriptableObjects/DebuffLibrary", order = 1)]
public class DebuffLibrary : ScriptableObject
{
    [SerializeField] private List<DebuffData> debuffs;

    public DebuffData GetDebuff(DebuffType id)
    {
        return debuffs[(int)id];
    }
}
