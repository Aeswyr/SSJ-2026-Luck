using UnityEngine;
using UnityEngine.Events;

public class EntityController : MonoBehaviour
{
    [SerializeField] private BuffController buffs;
    [SerializeField] private int maxHp;
    [SerializeField] private UnityEvent<int> onHit, onHealthChange, onMaxHealthChange;
    [SerializeField] private UnityEvent onDeath;
    private CardStickable stick;
    private int hp;

    public void Start()
    {
        hp = maxHp;
        stick = transform.GetComponentInChildren<CardStickable>();

        onMaxHealthChange.Invoke(maxHp);
    }
    public void OnHit(HitData hitData)
    {
        
        if (hitData.shouldStick)
            ApplyStick();

        buffs.OnHitBuff();

        if (hitData.preDamageCallback != null)
            hitData.preDamageCallback(ref hitData, this);

        hitData.bonusDamage += 1 * buffs.GetBuffStacks(BuffType.PAIN);
        int markCount = buffs.RemoveAllBuff(BuffType.MARK);
        hitData.bonusDamage += 3 * markCount + (int)(hitData.baseDamage * 0.5f * markCount);

        ApplyDamage(hitData.totalDamage);

        if (hitData.postDamageCallback != null)
            hitData.postDamageCallback(ref hitData, this);
    }

    public BuffController GetBuffController()
    {
        return buffs;
    }

    public void ApplyStick()
    {
        if (stick != null)
            stick.StickCard();
    }

    public void ApplyDamage(int amount)
    {
        VFXManager.Instance.CreateToast(amount.ToString(), transform.position
                                + new Vector3(Random.Range(-0.75f, 0.75f), 3f
                                + Random.Range(0, 0.75f)), Color.darkRed,
                                amount < 5 ? 24 : (amount < 15 ? 32 : 16 * Mathf.Clamp((amount - 15) / 35, 0, 1) + 32 ));
        
        if (hp <= 0)
        {
            return;
        }
        
        hp -= amount;
        hp = Mathf.Max(hp, 0);
        onHit?.Invoke(hp);
        onHealthChange?.Invoke(hp);

        if (hp <= 0)
        {
            onDeath.Invoke();
        }
    }

    public void ApplyHealing (int amount)
    {
        hp += amount;
        if (hp > maxHp)
        {
            hp = maxHp;
        }
        onHealthChange?.Invoke(hp);
    }

    public void AdjustMaxHealth(int amount)
    {
        maxHp += amount;
        if (hp > maxHp)
        {
            hp = maxHp;
        }
        onMaxHealthChange.Invoke(maxHp);
    }

    public bool IsMaxHealth()
    {
        return hp == maxHp;
    }

    public int GetMaxHealth()
    {
        return maxHp;
    }
}
