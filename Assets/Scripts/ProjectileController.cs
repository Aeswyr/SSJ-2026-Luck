using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private DestroyAfterDelay destroyAfterDelay; 
    private bool doesPierce;
    private Transform owner;
    private HitData payload;
    bool isEnemy;
    public List<HurtboxController> collisions = new();
    
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

    public ProjectileController SetFlip(bool flip)
    {
        spriteRenderer.flipX = flip;
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
        VFXManager.Instance.CreateVFX(VFXType.HITSPARK_SMALL, transform.position, rbody.linearVelocityX < 0);
        Destroy(gameObject);
    }

    public void OnEntityImpact(Collider2D collider)
    {
        if (isEnemy && !collider.transform.parent.TryGetComponent(out PlayerController player))
            return;

        if (collider.transform.parent == owner)
            return;

        var hurtbox = collider.transform.GetComponent<HurtboxController>();
        if (collisions.Contains(hurtbox))
            return;

        collisions.Add(hurtbox);
        hurtbox.OnHit(payload);
        
        if (hurtbox.TouchDisabled())
            return;

        if (!isEnemy){
            VFXManager.Instance.CreateVFX(VFXType.HITSPARK_SMALL, transform.position, spriteRenderer.flipX);
        }

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
    public OnHitCallback preDamageCallback;
    public OnHitCallback postDamageCallback;
    public int totalDamage => baseDamage + bonusDamage;

    public delegate void OnHitCallback(ref HitData hitData, EntityController entity);
}
