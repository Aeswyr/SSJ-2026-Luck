using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class BuffController : MonoBehaviour
{
    [SerializeField] private Transform buffHolder;
    [SerializeField] private GameObject buffObject;
    [SerializeField] private BuffLibrary buffLibrary;

    private EntityController entityController;
    private CardStickable stick;
    List<BuffInstance> buffs = new();

    void Start()
    {
        entityController = transform.GetComponentInParent<EntityController>();
        stick = transform.parent.GetComponentInChildren<CardStickable>();
    }
    public void AddBuff(BuffType type,int stacksBonus = 0, int stacksOverride = -1)
    {
        var buff = buffLibrary.GetBuff(type);

        GameObject icon = null;
        BuffInstance instance = null;
        foreach (var b in buffs)
        {
            if (b.data.id == type)
            {
                icon = b.icon;
                instance = b;
                break;
            }
        }

        if (icon == null) {
            icon = Instantiate(buffObject, buffHolder);
            icon.SetActive(true);
            icon.GetComponent<Image>().sprite = buff.icon;

            instance = new();
            instance.data = buff;
            instance.icon = icon;
            instance.stacks = 0;

            buffs.Add(instance);
        }

        instance.stacks += stacksOverride != -1 ? stacksOverride : buff.baseStacks;
        icon.GetComponentInChildren<TextMeshProUGUI>().text = instance.stacks.ToString();
    }

    // returns true if buff was removed
    public bool SpendBuffStack(BuffType type)
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            BuffInstance b = buffs[i];
            if (b.data.id == type)
            {
                b.stacks--;
                if (b.stacks > 0)
                    b.icon.GetComponentInChildren<TextMeshProUGUI>().text = b.stacks.ToString();
                else
                {
                    buffs.RemoveAt(i);
                    Destroy(b.icon);
                    return true;
                }
                break;
            }
        }
        return false;
    }

    public int GetBuffStacks(BuffType type)
    {
        int count = 0;
        
        foreach (var buff in buffs)
            if (buff.data.id == type)
                count = buff.stacks;
        
        return count;
    }

    public int GetBuffCount(bool excludeBuffs = false)
    {
        if (excludeBuffs)
        {
            int count = 0;
            foreach (var buff in buffs)
                if (buff.data.isDebuff)
                    count += buff.stacks;
            return count;
        }
        return buffs.Count;
    }

    public int RemoveAllBuff(BuffType type)
    {
        int count = 0;
        
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].data.id == type)
            {
                count += buffs[i].stacks;
                Destroy(buffs[i].icon);
                buffs.RemoveAt(i);
                i--;
            }
        }

        return count;
    }

    public int RemoveAllBuff()
    {
        int count = 0;
        
        for (int i = 0; i < buffs.Count; i++)
        {
            Destroy(buffs[i].icon);
            buffs.RemoveAt(i);
            i--;
            count++;
        }

        return count;
    }

    public void OnHitBuff()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            var buff = buffs[i];
            switch (buff.data.id)
            {
                case BuffType.LOOT:
                    buff.misc++;
                    if (buff.misc >= 2)
                    {
                        buff.misc = 0;
                        FindAnyObjectByType<PlayerController>().DrawCard();
                        if (SpendBuffStack(BuffType.LOOT))
                            i--;
                    }
                    break;
                case BuffType.COMEDY:
                    buff.misc++;
                    if (buff.misc >= 2)
                    {
                        buff.misc = 0;
                        FindAnyObjectByType<PlayerController>().AddCardToHand(CardID.JOKER);
                        if (SpendBuffStack(BuffType.COMEDY))
                            i--;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    void FixedUpdate()
    {
        for (int i = 0; i < buffs.Count; i++)
        {
            var buff = buffs[i];
            if (buff.data.id == BuffType.BLEED)
                if (Time.time > buff.nextTick) {
                    entityController.ApplyDamage(1 + stick.cards / 3);
                    buffs[i].nextTick = Time.time + 1;
                    if (SpendBuffStack(BuffType.BLEED))
                        i--;
                }
        }
    }


    private class BuffInstance
    {
        public BuffData data;
        public GameObject icon;

        public int misc;
        public float nextTick = 0;
        public int stacks;
    }
}

public enum BuffType
{
    MARK, BLEED, LOOT, PAIN, COMEDY, EMPOWER, GUILT, NONE
}

[Serializable] public struct BuffData
{
    public BuffType id;
    public Sprite icon;
    public int baseStacks;
    public bool isDebuff;
}


