using NUnit.Framework;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private DestroyAfterDelay destroyAfterDelay; 
    private bool doesPierce;
    private Transform owner;
    private HitData payload;
    bool isEnemy;
    
    public ProjectileController SetVelocity(float x, float y = 0)
    {
        rbody.linearVelocityX = x;
        rbody.linearVelocityY = y;
        return this;
    }
    public ProjectileController SetVelocity(Vector2 vel)
    {
        rbody.linearVelocity = vel;
        return this;
    }
    public ProjectileController SetEntityPierce()
    {
        doesPierce = true;
        return this;
    }
    public ProjectileController SetLifetime(float lifetime)
    {
        destroyAfterDelay.Init(lifetime);
        return this;
    }
    public void Init(Transform owner, HitData payload)
    {
        this.owner = owner;
        this.payload = payload;
        isEnemy = !owner.TryGetComponent(out PlayerController enemy);
    }


    public void OnWorldImpact(Collider2D collider)
    {
        Destroy(gameObject);
    }

    public void OnEntityImpact(Collider2D collider)
    {
        if (isEnemy && !collider.transform.parent.TryGetComponent(out PlayerController player))
            return;

        if (collider.transform.parent == owner)
            return;

        var hurtbox = collider.transform.GetComponent<HurtboxController>();
        hurtbox.OnHit(payload);
        if (hurtbox.intangible)
            return;

        if (doesPierce)
            return;

        Destroy(gameObject);
    }
}



public struct HitData
{
    public int baseDamage;
    public int bonusDamage;
    public bool shouldStick;

    public int totalDamage => baseDamage + bonusDamage;
}
