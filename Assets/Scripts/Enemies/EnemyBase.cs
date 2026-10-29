using System;
using System.Collections.Generic;
using UnityEngine;

// Base comum dos inimigos: vida, visão do jogador, ataque por Animation Event e morte.
// EnemyGround e EnemyFlying só decidem como se mover.
[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
public abstract class EnemyBase : MonoBehaviour
{
    protected static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    // Avisado quando qualquer inimigo morre (XP e drops escutam aqui).
    public static event Action<EnemyBase> AnyEnemyDied;

    [SerializeField] protected EnemyData data;
    [SerializeField] protected Transform attackPoint;
    [SerializeField] protected float attackRadius = 0.7f;
    [SerializeField] protected LayerMask playerLayer;
    [SerializeField] private float corpseTime = 1.5f;

    [Header("Sons")]
    [SerializeField] private AudioManager.Sfx attackSound = AudioManager.Sfx.EnemyAttack;
    [SerializeField] private AudioManager.Sfx deathSound = AudioManager.Sfx.EnemyDeath;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Health health;
    protected Transform player;
    private float nextAttackTime;
    private readonly List<Collider2D> hits = new List<Collider2D>();

    public EnemyData Data => data;
    protected bool PlayerInSight => player != null;
    protected bool IsDead => health.IsDead;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>(); // no chefe, o Animator fica no filho "Root"
        health = GetComponent<Health>();
        health.SetMaxHealth(data.maxHealth, true);
        health.Died += HandleDeath;
    }

    // Área de visão: um CircleCollider2D trigger no filho "Vision".
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) player = other.transform;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) player = null;
    }

    protected virtual bool PlayerInAttackRange()
    {
        if (player == null) return false;
        Vector2 toPlayer = player.position - transform.position;
        return Mathf.Abs(toPlayer.x) <= data.attackRange && Mathf.Abs(toPlayer.y) <= 1.5f;
    }

    protected virtual float AttackCooldown => data.attackCooldown;

    protected void TryAttack()
    {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + AttackCooldown;
        animator.SetTrigger(AttackHash);
        AudioManager.Play(attackSound);
    }

    // Animation Event no quadro do golpe.
    public void AttackHit()
    {
        if (IsDead) return;
        var filter = new ContactFilter2D { useTriggers = false };
        filter.SetLayerMask(playerLayer);
        Physics2D.OverlapCircle(attackPoint.position, attackRadius, filter, hits);
        foreach (Collider2D hit in hits)
            if (hit.TryGetComponent(out Health target))
                target.TakeDamage(data.damage);
    }

    protected void Face(float direction)
    {
        if (Mathf.Abs(direction) < 0.01f) return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        transform.localScale = scale;
    }

    protected float FacingDirection => Mathf.Sign(transform.localScale.x);

    private void HandleDeath()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
            col.enabled = false;
        animator.SetFloat(SpeedHash, 0f);
        AudioManager.Play(deathSound);
        AnyEnemyDied?.Invoke(this);
        Destroy(gameObject, corpseTime);
        enabled = false;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
