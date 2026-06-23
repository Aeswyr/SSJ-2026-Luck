using UnityEngine;
using UnityEngine.Events;

public class EntityController : MonoBehaviour
{
    [SerializeField] private DebuffController debuffs;
    [SerializeField] private int maxHp;
    [SerializeField] private UnityEvent<int> onHit, onHealthChange;
    [SerializeField] private UnityEvent onDeath;
    private int hp;

    public void Start()
    {
        hp = maxHp;
    }
    public void OnHit(HitData hitData)
    {
        
        if (hitData.shouldStick && transform.TryGetComponent(out CardStickable stick))
        {
            stick.StickCard();
        }

        debuffs.OnHitDebuff();

        hitData.bonusDamage += 1 * debuffs.GetDebuffCount(DebuffType.PAIN);
        int markCount = debuffs.RemoveAllDebuff(DebuffType.MARK);
        hitData.bonusDamage += 3 * markCount + (int)(hitData.baseDamage * 0.5f * markCount);

        ApplyDamage(hitData.totalDamage);
    }

    public void ApplyDamage(int amount)
    {
        VFXManager.Instance.CreateToast(amount.ToString(), transform.position + new Vector3(Random.Range(-0.75f, 0.75f), 1.5f + Random.Range(0, 0.75f)), Color.darkRed);
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
        onHealthChange?.Invoke(hp);
    }

    public int GetMaxHealth()
    {
        return maxHp;
    }
}
