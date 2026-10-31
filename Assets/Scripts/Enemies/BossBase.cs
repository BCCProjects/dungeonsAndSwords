using UnityEngine;

// Base comum dos chefes: persegue o jogador, ataca de perto, tem um ataque de longe próprio,
// entra na fase 2 com metade da vida, solta loot durante a luta e só às vezes sente o golpe.
// Golpe começado vai até o fim: apanhar no meio do ataque não cancela a animação.
// Cada chefe implementa o ataque de longe (TryRangedAttack) e diz quando está ocupado (IsBusy).
public abstract class BossBase : EnemyBase
{
    protected static readonly int HurtHash = Animator.StringToHash("Hurt");
    protected static readonly int DieHash = Animator.StringToHash("Die");

    [Header("Chefe")]
    [SerializeField] protected float meleeRange = 2.2f;
    [SerializeField] private float hurtCooldown = 4f; // nem todo golpe interrompe o chefe
    [Tooltip("Tempo perto do jogador antes do primeiro golpe: dá chance de reagir quando ele chega.")]
    [SerializeField] private float meleeWindup = 0f;

    [Header("Fase 2")]
    [SerializeField, Range(0f, 1f)] private float phase2Threshold = 0.5f;
    [SerializeField] private float phase2SpeedMultiplier = 1.5f;
    [SerializeField] private float phase2CooldownMultiplier = 0.6f;

    [Header("Loot durante a luta")]
    [Tooltip("Itens soltos ao longo da luta: um a cada parte igual de vida perdida.")]
    [SerializeField] private int lootDrops = 3;

    // O Animator só entra no estado de ataque no quadro seguinte ao trigger; nesse intervalo o chefe já conta como ocupado.
    private const float ActionStartLock = 0.25f;

    private float nextHurtTime;
    private float actionLockUntil;
    private float inMeleeRangeSince = -1f;
    private int dropsGiven;
    private DropSystem dropSystem;

    protected bool Phase2 { get; private set; }
    protected float CooldownMultiplier => Phase2 ? phase2CooldownMultiplier : 1f;
    protected override float AttackCooldown => data.attackCooldown * CooldownMultiplier;

    protected override void Awake()
    {
        base.Awake();
        health.PlayHurtAnimation = false; // quem decide o Hurt é o OnHealthChanged
        health.HealthChanged += OnHealthChanged;
        health.Died += OnBossDied;
        dropSystem = FindFirstObjectByType<DropSystem>();
    }

    protected virtual void OnBossDied() => animator.SetTrigger(DieHash);

    private void Update()
    {
        if (IsDead) return;
        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        if (!PlayerInSight || Busy)
        {
            Stop();
            return;
        }

        float toPlayer = player.position.x - transform.position.x;
        Face(toPlayer);
        float distance = Mathf.Abs(toPlayer);

        if (distance <= meleeRange && Mathf.Abs(player.position.y - transform.position.y) < 2.5f)
        {
            Stop();
            if (inMeleeRangeSince < 0f) inMeleeRangeSince = Time.time;
            if (Time.time - inMeleeRangeSince >= meleeWindup && TryAttack()) BeginAction();
        }
        else if (TryRangedAttack(distance))
        {
            BeginAction();
            inMeleeRangeSince = -1f;
            Stop();
        }
        else if (distance < SameColumnDistance)
        {
            inMeleeRangeSince = -1f;
            Stop(); // jogador logo acima, fora do alcance: espera embaixo em vez de tremer
        }
        else
        {
            inMeleeRangeSince = -1f;
            float speed = data.chaseSpeed * (Phase2 ? phase2SpeedMultiplier : 1f);
            rb.linearVelocity = new Vector2(Mathf.Sign(toPlayer) * speed, rb.linearVelocity.y);
        }
    }

    // Começa o ataque de longe se estiver pronto. Retorna true se começou.
    protected abstract bool TryRangedAttack(float distanceToPlayer);

    // Enquanto ataca ou sente o golpe, o chefe não anda.
    protected abstract bool IsBusy();

    private bool Busy => Time.time < actionLockUntil || IsBusy();

    // Um Hurt que ficou pendente não pode cortar o golpe que está começando.
    private void BeginAction()
    {
        actionLockUntil = Time.time + ActionStartLock;
        animator.ResetTrigger(HurtHash);
    }

    private void OnHealthChanged(int current, int max)
    {
        if (!Phase2 && current <= max * phase2Threshold) Phase2 = true;

        // Solta um item a cada parte da vida perdida (ex.: 3 itens = em 75%, 50% e 25%).
        int earned = Mathf.Min(lootDrops, Mathf.FloorToInt((1f - current / (float)max) * (lootDrops + 1)));
        while (dropsGiven < earned && current > 0 && dropSystem != null)
        {
            dropsGiven++;
            dropSystem.DropRandom(transform.position + Vector3.up);
        }

        if (!IsDead && current < max && Time.time >= nextHurtTime && !Busy)
        {
            nextHurtTime = Time.time + hurtCooldown;
            animator.SetTrigger(HurtHash);
        }
    }

    protected bool StateIs(string stateName) => animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);

    protected void Stop() => rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
}
