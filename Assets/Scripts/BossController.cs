using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class BossController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rbody;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private HurtboxController hurtbox;
    [SerializeField] private string closingDialog;
    [SerializeField] EntityController entity;
    [SerializeField] List<EnemyAttackData> attacks;
    [SerializeField] float attackDelay;
    [SerializeField] float sequenceDelay;
    private float nextAttack;
    private List<EnemyAttackData> attackSequence = new();
    PlayerHUDController hud;
    private PlayerController target;
    private bool acting;
    private int facing;
    private bool started = false;
    void Start()
    {
        hud = FindAnyObjectByType<PlayerHUDController>();
        hurtbox.disabled = true;

    }

    public void StartBattle()
    {
        hud.ToggleBossHealth(true);
        hud.updateBossHealth(entity.GetMaxHealth(), entity.GetMaxHealth());

        hurtbox.disabled = false;
        
        NewSequence();
        nextAttack = attackDelay;

        started = true;
    }

    void FixedUpdate()
    {
        if (!started)
            return;

        if (target != null)
        {
            
            if (!acting)
            {
                rbody.linearVelocityX = 0;

                facing = (int)Mathf.Sign(target.transform.position.x - transform.position.x);
                spriteRenderer.flipX = facing >= 0;
            
                if (Time.time > nextAttack)
                {
                    animator.Play(attackSequence[0].tag);
                    attackSequence.RemoveAt(0);
                    acting = true;
                }
            }
        } else 
            target = FindAnyObjectByType<PlayerController>();
    }

    public void EndAction()
    {
        nextAttack = Time.time + attackDelay;

        if (attackSequence.Count == 0)
            NewSequence();

        acting = false;
        hurtbox.invincible = false;
        animator.Play("idle");
    }

    public void OnHit(int hp)
    {
        hud.updateBossHealth(hp, entity.GetMaxHealth());
    }
    public void OnDeath()
    {
        rbody.linearVelocityX = 0;
        animator.Play("felled");
        acting = true;
        nextAttack = Time.time + 100000;

        if (string.IsNullOrEmpty(closingDialog))
        {
            ShowVictory();
        } else
        {
            DialogManager.Instance.PlayConversation(closingDialog, () => ShowVictory());
        }

        void ShowVictory()
        {
            animator.Play("dead");
            FindAnyObjectByType<PlayerController>().ToggleInputLock(true);
            hud.ShowWinScreen();
        }
    }

    public void NewSequence()
    {
        nextAttack = Time.time + sequenceDelay;

        attackSequence.Clear();

        for (int i = 0; i < attacks.Count; i++)
        {
            attackSequence.Insert(Random.Range(0, attackSequence.Count), attacks[i]);    
        }
    }

    public void Attack_CROC_LEAP() //0
    {
        StartCoroutine(LeapSequence());
        IEnumerator LeapSequence()
        {
            hurtbox.invincible = true;
            rbody.bodyType = RigidbodyType2D.Kinematic;
            spriteRenderer.sortingLayerName = "Projectile";
            transform.DOJump(new (transform.position.x, -12), 14, 1, 1f);
            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(0.5f);
            transform.position = new(target.transform.position.x, transform.position.y);
            yield return new WaitForSeconds(0.3f);
            var attack = Instantiate(attacks[1].attackPrefab[0], transform);
            attack.GetComponent<ProjectileController>()
                .SetEntityPierce()
                .SetLifetime(0.7f)
                .Init(transform, new HitData()
                {
                    baseDamage = 1
                });
            transform.DOJump(new (transform.position.x, 2), 4, 1, 1f);
            yield return new WaitForSeconds(1.1f);
            transform.position = new(transform.position.x, 2);
            rbody.bodyType = RigidbodyType2D.Dynamic;
            spriteRenderer.sortingLayerName = "Entity";
        }

    }

    public void Attack_CROC_STOMP() //3
    {
        StartCoroutine(StompSequence());
        IEnumerator StompSequence()
        {
            yield return new WaitForSeconds(0.3f);
            transform.DOJump(transform.position, 3, 1, 0.6f);
            yield return new WaitForSeconds(0.6f);
            transform.position = new(transform.position.x, 2);
            for (int i = 0; i < 3; i++)
            {
                GameObject proj1L = Instantiate(attacks[3].attackPrefab[0], transform.position + i * facing * 8 * Vector3.right + facing * 4 * Vector3.right, Quaternion.identity, GameManager.Instance.GetCurrentLevel().GetObjectParent());
                proj1L.GetComponent<ProjectileController>()
                    .SetVelocity(facing * 0.5f)
                    .SetFlip(facing == -1)
                    .SetEntityPierce()
                    .SetLifetime(0.55f)
                    .Init(transform, new ()
                    {
                        baseDamage = 1
                    });
                GameObject proj1R = Instantiate(attacks[3].attackPrefab[0], transform.position + i * -facing * 8 * Vector3.right + -facing * 4 * Vector3.right, Quaternion.identity, GameManager.Instance.GetCurrentLevel().GetObjectParent());
                proj1R.GetComponent<ProjectileController>()
                    .SetVelocity(facing * 0.5f)
                    .SetFlip(facing == 1)
                    .SetEntityPierce()
                    .SetLifetime(0.55f)
                    .Init(transform, new ()
                    {
                        baseDamage = 1
                    });
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
    public void Attack_CROC_DASH() //1
    {
        rbody.linearVelocityX = facing * 32;

        var attack = Instantiate(attacks[1].attackPrefab[0], transform);
        attack.GetComponent<ProjectileController>()
            .SetEntityPierce()
            .SetLifetime(0.7f)
            .Init(transform, new HitData()
            {
                baseDamage = 1
            });
    }
    public void Attack_CROC_SHOT() //2
    {
        StartCoroutine(ShotSequence());
        IEnumerator ShotSequence()
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 angle = new Vector2(facing, i * 0.1f).normalized;
                GameObject proj1 = Instantiate(attacks[2].attackPrefab[0], transform.position + 3 * Vector3.up + facing * angle, Quaternion.identity, GameManager.Instance.GetCurrentLevel().GetObjectParent());
                proj1.GetComponent<ProjectileController>()
                    .SetVelocity(24 * angle)
                    .SetFlip(facing == 1)
                    .SetEntityPierce()
                    .SetLifetime(2f)
                    .Init(transform, new ()
                    {
                        baseDamage = 1
                    });
                yield return new WaitForSeconds(0.20f);
            }
        }
    }
}
