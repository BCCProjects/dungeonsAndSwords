using System.Collections;
using UnityEngine;

// Chefe da fase 3 (Dark Knight):
// - de perto, golpe de espada (dano no Animation Event AttackHit);
// - de longe, carrega e lança uma bola de energia (Animation Event ShootBeam);
// - ao morrer, o corpo se desfaz (efeito de dissolver do material do pacote).
public class BossController : BossBase
{
    private static readonly int BeamHash = Animator.StringToHash("BeamAttack");
    private static readonly int DissolveId = Shader.PropertyToID("_Dissolve");

    [Header("Bola de energia")]
    [SerializeField] private float beamMinDistance = 4f;
    [SerializeField] private float beamCooldown = 5f;
    [SerializeField] private Transform beamPoint;
    [SerializeField] private Projectile beamPrefab;
    [SerializeField] private int beamDamage = 6;

    [Header("Morte")]
    [SerializeField] private float dissolveTime = 2.5f;

    private float nextBeamTime;

    protected override void Awake()
    {
        base.Awake();
        nextBeamTime = Time.time + 2f; // dá um respiro quando o chefe aparece
    }

    protected override bool TryRangedAttack(float distanceToPlayer)
    {
        if (distanceToPlayer < beamMinDistance || Time.time < nextBeamTime) return false;
        nextBeamTime = Time.time + beamCooldown * CooldownMultiplier;
        animator.SetTrigger(BeamHash);
        AudioManager.Play(AudioManager.Sfx.BossCharge);
        return true;
    }

    protected override bool IsBusy() => StateIs("Attack") || StateIs("BeamAttack") || StateIs("Hurt");

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

    protected override void OnBossDied()
    {
        base.OnBossDied();
        StartCoroutine(Dissolve());
    }

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
