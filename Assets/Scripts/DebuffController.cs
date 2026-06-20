using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine.UI;
using UnityEngine;

public class DebuffController : MonoBehaviour
{
    [SerializeField] private Transform debuffHolder;
    [SerializeField] private GameObject debuffObject;
    [SerializeField] private DebuffLibrary debuffLibrary;

    private EntityController entityController;
    private CardStickable stick;
    List<DebuffInstance> debuffs = new();

    void Start()
    {
        entityController = transform.GetComponentInParent<EntityController>();
        stick = transform.GetComponentInParent<CardStickable>();
    }
    public void AddDebuff(DebuffType type)
    {
        var debuff = debuffLibrary.GetDebuff(type);

        GameObject icon = Instantiate(debuffObject, debuffHolder);
        icon.SetActive(true);
        icon.GetComponent<Image>().sprite = debuff.icon;

        DebuffInstance instance = new();
        instance.data = debuff;
        instance.icon = icon;
        instance.expiration = Time.time + debuff.baseDuration;

        debuffs.Add(instance);
    }

    public int GetDebuffCount(DebuffType type)
    {
        int count = 0;
        
        foreach (var debuff in debuffs)
            if (debuff.data.id == type)
                count++;
        
        return count;
    }

    public int RemoveAllDebuff(DebuffType type)
    {
        int count = 0;
        
        for (int i = 0; i < debuffs.Count; i++)
        {
            if (debuffs[i].data.id == type)
            {
                Destroy(debuffs[i].icon);
                debuffs.RemoveAt(i);
                i--;
                count++;
            }
        }

        return count;
    }

    public void OnHitDebuff()
    {
        foreach (var debuff in debuffs)
        {
            switch (debuff.data.id)
            {
                case DebuffType.LOOT:
                    debuff.misc++;
                    if (debuff.misc >= 4)
                    {
                        debuff.misc = 0;
                        FindAnyObjectByType<PlayerController>().DrawCard();
                    }
                    break;
                case DebuffType.COMEDY:
                    debuff.misc++;
                    if (debuff.misc >= 2)
                    {
                        debuff.misc = 0;
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
        for (int i = 0; i < debuffs.Count; i++)
        {
            var debuff = debuffs[i];
            if (debuff.data.id == DebuffType.BLEED)
                if (Time.time > debuff.nextTick) {
                    entityController.ApplyDamage(1 + stick.cards / 3);
                    debuffs[i].nextTick = Time.time + 1;
                }


            if (Time.time > debuff.expiration)
            {
                Destroy(debuff.icon);
                debuffs.RemoveAt(i);
                i--;
            }
        }
    }


    private class DebuffInstance
    {
        public DebuffData data;
        public float expiration;
        public GameObject icon;

        public int misc;
        public float nextTick = 0;
    }
}

public enum DebuffType
{
    MARK, BLEED, LOOT, PAIN, COMEDY, NONE
}

[Serializable] public struct DebuffData
{
    public DebuffType id;
    public Sprite icon;
    public float baseDuration;
}


