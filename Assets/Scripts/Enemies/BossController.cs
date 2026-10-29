using System.Collections;
using UnityEngine;

// Chefe final (Dark Knight), com padrão de ataque próprio:
// - persegue o jogador andando;
// - de perto, golpe de espada (dano no Animation Event AttackHit);
// - de longe, carrega e lança uma bola de energia (Animation Event ShootBeam);
// - abaixo de metade da vida entra na fase 2: anda mais rápido e ataca com mais frequência.
public class BossController : EnemyBase
{
    private static readonly int BeamHash = Animator.StringToHash("BeamAttack");
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int DissolveId = Shader.PropertyToID("_Dissolve");

    [Header("Chefe")]
    [SerializeField] private float meleeRange = 2.2f;
    [SerializeField] private float beamMinDistance = 4f;
    [SerializeField] private float beamCooldown = 5f;
    [SerializeField] private Transform beamPoint;
    [SerializeField] private Projectile beamPrefab;
    [SerializeField] private int beamDamage = 6;
    [SerializeField] private float hurtCooldown = 2.5f; // nem todo golpe interrompe o chefe

    [Header("Fase 2")]
    [SerializeField, Range(0f, 1f)] private float phase2Threshold = 0.5f;
    [SerializeField] private float phase2SpeedMultiplier = 1.5f;
    [SerializeField] private float phase2CooldownMultiplier = 0.6f;

    [Header("Loot durante a luta")]
    [Tooltip("Itens soltos ao longo da luta: um a cada parte igual de vida perdida.")]
    [SerializeField] private int lootDrops = 3;

    [Header("Morte")]
    [SerializeField] private float dissolveTime = 2.5f;

    private float nextBeamTime;
    private float nextHurtTime;
    private bool phase2;
    private int dropsGiven;
    private DropSystem dropSystem;

    private float CooldownMultiplier => phase2 ? phase2CooldownMultiplier : 1f;
    protected override float AttackCooldown => data.attackCooldown * CooldownMultiplier;

    protected override void Awake()
    {
        base.Awake();
        health.HealthChanged += OnHealthChanged;
        health.Died += () =>
        {
            animator.SetTrigger(DieHash);
            StartCoroutine(Dissolve());
        };
        nextBeamTime = Time.time + 2f; // dá um respiro quando o chefe aparece
        dropSystem = FindFirstObjectByType<DropSystem>();
    }

    private void Update()
    {
        if (IsDead) return;
        animator.SetFloat(SpeedHash, Mathf.Abs(rb.linearVelocity.x));
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        if (!PlayerInSight || IsBusy())
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
            TryAttack();
        }
        else if (distance >= beamMinDistance && Time.time >= nextBeamTime)
        {
            Stop();
            nextBeamTime = Time.time + beamCooldown * CooldownMultiplier;
            animator.SetTrigger(BeamHash);
            AudioManager.Play(AudioManager.Sfx.BossCharge);
        }
        else
        {
            float speed = data.chaseSpeed * (phase2 ? phase2SpeedMultiplier : 1f);
            rb.linearVelocity = new Vector2(Mathf.Sign(toPlayer) * speed, rb.linearVelocity.y);
        }
    }

    // Animation Event do BeamAttack (repassado pelo BossAnimationEvents).
    public void ShootBeam()
    {
        if (IsDead) return;
        // Mira no peito do jogador (a bola sai da altura da cabeça do chefe).
        Vector2 direction = player != null
            ? (Vector2)(player.position + Vector3.up * 0.8f - beamPoint.position)
            : new Vector2(FacingDirection, 0f);
        AudioManager.Play(AudioManager.Sfx.BossBeam);
        Projectile beam = Instantiate(beamPrefab, beamPoint.position, Quaternion.identity);
        beam.Launch(direction, beamDamage, playerLayer);
    }

    private void OnHealthChanged(int current, int max)
    {
        if (!phase2 && current <= max * phase2Threshold) phase2 = true;

        // Solta um item a cada parte da vida perdida (ex.: 3 itens = em 75%, 50% e 25%).
        int earned = Mathf.Min(lootDrops, Mathf.FloorToInt((1f - current / (float)max) * (lootDrops + 1)));
        while (dropsGiven < earned && current > 0 && dropSystem != null)
        {
            dropsGiven++;
            dropSystem.DropRandom(transform.position + Vector3.up);
        }

        if (!IsDead && current < max && Time.time >= nextHurtTime && !IsBusy())
        {
            nextHurtTime = Time.time + hurtCooldown;
            animator.SetTrigger(HurtHash);
        }
    }

    // Enquanto ataca, carrega o raio ou sente o golpe, o chefe não anda.
    private bool IsBusy()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        return state.IsName("Attack") || state.IsName("BeamAttack") || state.IsName("Hurt");
    }

    private void Stop() => rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

    // Morte: o corpo se desfaz usando o efeito de dissolver do material do pacote.
    private IEnumerator Dissolve()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        var block = new MaterialPropertyBlock();
        for (float t = 0f; t < dissolveTime; t += Time.deltaTime)
        {
            block.SetFloat(DissolveId, 1f - t / dissolveTime);
            foreach (SpriteRenderer r in renderers) r.SetPropertyBlock(block);
            yield return null;
        }
    }
}
