using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Ataque do jogador. O botão só dispara o trigger Attack; o dano (ou o projétil)
// sai no Animation Event do clip de ataque: MeleeHit() ou ShootProjectile().
[RequireComponent(typeof(PlayerController), typeof(Health))]
public class PlayerCombat : MonoBehaviour
{
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");

    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Corpo a corpo")]
    [SerializeField] private float meleeRadius = 0.8f;

    [Header("À distância")]
    [SerializeField] private Projectile projectilePrefab;

    [Header("Morte")]
    [SerializeField] private float gameOverDelay = 1.5f;

    private PlayerController controller;
    private Health health;
    private Animator animator;
    private ClassData classData;
    private float nextAttackTime;
    private readonly List<Collider2D> hits = new List<Collider2D>();

    public int DamageBonus { get; set; }                // somado pelos itens
    public float SpeedMultiplier { get; set; } = 1f;    // Agilidade: recarga menor e animação mais rápida
    public int Damage => classData.baseDamage + DamageBonus;
    private float Cooldown => classData.attackCooldown / SpeedMultiplier;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        health = GetComponent<Health>();
        animator = GetComponent<Animator>();
        classData = controller.ClassData;
    }

    private void OnEnable()
    {
        attackAction.action.Enable();
        attackAction.action.performed += OnAttackPressed;
        health.Died += HandleDeath;
    }

    private void OnDisable()
    {
        attackAction.action.performed -= OnAttackPressed;
        health.Died -= HandleDeath;
    }

    private void OnAttackPressed(InputAction.CallbackContext context)
    {
        if (health.IsDead || Time.timeScale == 0f || Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + Cooldown;
        animator.SetFloat(AttackSpeedHash, SpeedMultiplier); // o estado Attack usa esse parâmetro como velocidade
        animator.SetTrigger(AttackHash);
        if (projectilePrefab == null) AudioManager.Play(AudioManager.Sfx.Attack); // golpe corpo a corpo
    }

    // Animation Event do Guerreiro: aplica o dano no momento do golpe.
    public void MeleeHit()
    {
        var filter = new ContactFilter2D { useTriggers = false };
        filter.SetLayerMask(enemyLayer);
        Physics2D.OverlapCircle(attackPoint.position, meleeRadius, filter, hits);

        foreach (Collider2D hit in hits)
            if (hit.TryGetComponent(out Health target))
                target.TakeDamage(Damage);
    }

    // Animation Event do Arqueiro e do Mago: dispara o projétil.
    public void ShootProjectile()
    {
        AudioManager.Play(AudioManager.Sfx.Shoot);
        Projectile projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
        projectile.Launch(new Vector2(Mathf.Sign(transform.localScale.x), 0f), Damage, enemyLayer);
    }

    private void HandleDeath()
    {
        controller.CanMove = false;
        StartCoroutine(GameOverAfterDelay());
    }

    private IEnumerator GameOverAfterDelay()
    {
        yield return new WaitForSeconds(gameOverDelay);
        SceneLoader.LoadGameOver();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, meleeRadius);
    }
}
