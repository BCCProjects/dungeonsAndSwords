using System.Collections;
using UnityEngine;

// Chefe final (Demon Slime):
// - de perto, golpe de cutelo (dano no Animation Event AttackHit);
// - de longe, o mesmo golpe lança uma onda de fogo rente ao chão (Animation Event ShootFire): dá para pular por cima;
// - na fase 2 lança duas ondas seguidas.
public class DemonSlimeController : BossBase
{
    private static readonly int FireHash = Animator.StringToHash("FireCleave");

    [Header("Onda de fogo")]
    [SerializeField] private float fireMinDistance = 4.5f;
    [SerializeField] private float fireCooldown = 5f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Projectile firePrefab;
    [SerializeField] private int fireDamage = 10;
    [SerializeField] private int phase2Waves = 2;
    [SerializeField] private float waveInterval = 0.7f;

    private float nextFireTime;

    protected override void Awake()
    {
        base.Awake();
        nextFireTime = Time.time + 2f; // dá um respiro quando o chefe aparece
    }

    protected override bool TryRangedAttack(float distanceToPlayer)
    {
        if (distanceToPlayer < fireMinDistance || Time.time < nextFireTime) return false;
        nextFireTime = Time.time + fireCooldown * CooldownMultiplier;
        animator.SetTrigger(FireHash);
        AudioManager.Play(AudioManager.Sfx.BossCharge);
        return true;
    }

    protected override bool IsBusy() => StateIs("Attack") || StateIs("FireCleave") || StateIs("Hurt");

    // Animation Event do golpe de fogo.
    public void ShootFire()
    {
        if (IsDead) return;
        StartCoroutine(FireWaves(Phase2 ? phase2Waves : 1));
    }

    private IEnumerator FireWaves(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (IsDead) yield break;
            Projectile wave = Instantiate(firePrefab, firePoint.position, Quaternion.identity);
            wave.Launch(new Vector2(FacingDirection, 0f), fireDamage, playerLayer);
            AudioManager.Play(AudioManager.Sfx.BossBeam);
            if (i < count - 1) yield return new WaitForSeconds(waveInterval);
        }
    }
}
