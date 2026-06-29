using UnityEngine;
using System;

using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private HurtboxController hurtbox;
    [SerializeField] private VFXType deathFX;
    [SerializeField] private float preferredRange, speed;
    [SerializeField] private bool flee;
    [SerializeField] private float attackCooldown, specialCooldown;
    [SerializeField] private int attackRange, specialRange;

    [Space]
    [Header("attack prefabs")]
    [SerializeField] private GameObject attackPrefab;
    private float nextAttack, nextSpecial;
    private PlayerController target;
    private bool acting;
    private int facing;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextAttack = Time.time + Random.Range(0, attackCooldown);
        nextSpecial = Time.time + Random.Range(0, specialCooldown);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (target != null)
        {
            
            float dist = Vector2.Distance(transform.position, target.transform.position);

            if (!acting) {
                facing = (int)Mathf.Sign(target.transform.position.x - transform.position.x);
                spriteRenderer.flipX = facing >= 0;

                if (Time.time > nextSpecial && dist < specialRange)
                {
                    animator.Play("special");
                    acting = true;
                    nextSpecial = Time.time + specialCooldown * Random.Range(0.8f, 1.2f);

                    rbody.linearVelocityX = 0;
                } else if (Time.time > nextAttack && dist < attackRange)
                {
                    animator.Play("attack");
                    acting = true;
                    nextAttack = Time.time + attackCooldown * Random.Range(0.8f, 1.2f);

                    rbody.linearVelocityX = 0;
                } else {
                    rbody.linearVelocityX = dist > preferredRange 
                        ? facing * speed : (flee ? (dist < preferredRange - 1 ? facing * -speed : 0) : 0);
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("move"))
                    {
                        animator.Play("move");
                    }
                }
            }
        } else
        {
            target = FindAnyObjectByType<PlayerController>();
        }
    }

    public void OnHPEmpty()
    {
        VFXManager.Instance.CreateVFX(deathFX, transform.position, spriteRenderer.flipX, duration: 60, renderBehind: true);
        Destroy(gameObject);
        
    }

    public void EndAction()
    {
        acting = false;
        hurtbox.invincible = false;
        animator.Play("idle");
    }

    public void FireAttack_WingDemon()
    {
        var attack = Instantiate(attackPrefab, transform.position + new Vector3(facing * 2, 2), Quaternion.identity, GameManager.Instance.GetCurrentLevel().GetObjectParent());
        attack.GetComponent<ProjectileController>()
					.SetVelocity(facing * 16)
                    .SetFlip(facing == -1)
                    .SetLifetime(3f)
					.Init(transform, new HitData()
                    {
                        baseDamage = 1
                    });
    
    }

    public void FireSpecial_WingDemon()
    {
        rbody.linearVelocityX = -facing * 18;
    }

    public void FireAttack_AnglerDemon()
    {
        var attack = Instantiate(attackPrefab, transform.position + new Vector3(facing * 8, 0f), Quaternion.identity, GameManager.Instance.GetCurrentLevel().GetObjectParent());
        attack.GetComponent<ProjectileController>()
					.SetVelocity(facing * 0.5f)
                    .SetFlip(facing == -1)
                    .SetEntityPierce()
                    .SetLifetime(0.55f)
					.Init(transform, new HitData()
                    {
                        baseDamage = 1
                    });
    }

    public void FireSpecial_AnglerDemon()
    {
        hurtbox.invincible = true;
    }

    public void OnGuard_AnglerDemon()
    {
        VFXManager.Instance.CreateToast("block", transform.position + new Vector3(Random.Range(-0.75f, 0.75f), 5f + Random.Range(0, 0.75f)), Color.gray);
    }

    public void FireAttack_SalamanderDemon()
    {
        var attack = Instantiate(attackPrefab, transform.position + new Vector3(facing * 4, 1f), Quaternion.identity);
        attack.GetComponent<ProjectileController>()
            .SetEntityPierce()
            .SetLifetime(0.2f)
            .Init(transform, new HitData()
            {
                baseDamage = 1
            });
    }

    public void FireSpecial_SalamanderDemon()
    {
        rbody.linearVelocityX = facing * 32;
                var attack = Instantiate(attackPrefab, transform);
        attack.transform.localPosition = new Vector3(facing, 2);
        attack.GetComponent<ProjectileController>()
            .SetEntityPierce()
            .SetLifetime(0.7f)
            .Init(transform, new HitData()
            {
                baseDamage = 1
            });
    }


    public void FireAttack_Hollow()
    {
        var attack = Instantiate(attackPrefab, transform);
        attack.transform.localPosition = new Vector3(facing * 1.5f , 2);
        attack.GetComponent<ProjectileController>()
            .SetEntityPierce()
            .SetLifetime(0.3f)
            .Init(transform, new HitData()
            {
                baseDamage = 1
            });
    }
}

[Serializable] public struct EnemyAttackData
{
    public string tag;
    public float cooldown;
    public float range;
    public GameObject[] attackPrefab;
} 
