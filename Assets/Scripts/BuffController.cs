using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

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
    public void AddBuff(BuffType type)
    {
        var buff = buffLibrary.GetBuff(type);

        GameObject icon = Instantiate(buffObject, buffHolder);
        icon.SetActive(true);
        icon.GetComponent<Image>().sprite = buff.icon;

        BuffInstance instance = new();
        instance.data = buff;
        instance.icon = icon;
        instance.expiration = Time.time + buff.baseDuration;

        buffs.Add(instance);
    }

    public int GetBuffCount(BuffType type)
    {
        int count = 0;
        
        foreach (var buff in buffs)
            if (buff.data.id == type)
                count++;
        
        return count;
    }

    public int RemoveAllBuff(BuffType type)
    {
        int count = 0;
        
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].data.id == type)
            {
                Destroy(buffs[i].icon);
                buffs.RemoveAt(i);
                i--;
                count++;
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
        foreach (var buff in buffs)
        {
            switch (buff.data.id)
            {
                case BuffType.LOOT:
                    buff.misc++;
                    if (buff.misc >= 4)
                    {
                        buff.misc = 0;
                        FindAnyObjectByType<PlayerController>().DrawCard();
                    }
                    break;
                case BuffType.COMEDY:
                    buff.misc++;
                    if (buff.misc >= 2)
                    {
                        buff.misc = 0;
                        FindAnyObjectByType<PlayerController>().AddCardToHand(CardID.JOKER);
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
                }


            if (Time.time > buff.expiration)
            {
                Destroy(buff.icon);
                buffs.RemoveAt(i);
                i--;
            }
        }
    }


    private class BuffInstance
    {
        public BuffData data;
        public float expiration;
        public GameObject icon;

        public int misc;
        public float nextTick = 0;
    }
}

public enum BuffType
{
    MARK, BLEED, LOOT, PAIN, COMEDY, EMPOWER, NONE
}

[Serializable] public struct BuffData
{
    public BuffType id;
    public Sprite icon;
    public float baseDuration;
    public bool isDebuff;
}


