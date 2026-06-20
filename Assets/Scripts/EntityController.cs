using UnityEngine;

public class EntityController : MonoBehaviour
{
    [SerializeField] private DebuffController debuffs;
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
        
        Debug.Log($"base: {hitData.baseDamage} bonus: {hitData.bonusDamage}");

        ApplyDamage(hitData.totalDamage);
    }

    public void ApplyDamage(int amount)
    {
        VFXManager.Instance.CreateToast(amount.ToString(), transform.position + new Vector3(Random.Range(-0.75f, 0.75f), 1.5f + Random.Range(0, 0.75f)), Color.darkRed);
    }
}
