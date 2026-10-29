using UnityEngine;

// Chefe final (Bringer of Death):
// - de perto, golpe largo de foice (dano no Animation Event AttackHit);
// - de longe, conjura a magia da morte sob o jogador (Animation Event CastSpell);
//   um portal abre no chão como aviso e a mão desce logo depois, dá para fugir;
// - na fase 2 conjura três magias de uma vez (no jogador e dos dois lados).
public class BringerController : BossBase
{
    private static readonly int CastHash = Animator.StringToHash("Cast");

    [Header("Magia da morte")]
    [SerializeField] private float castMinDistance = 3.5f;
    [SerializeField] private float castCooldown = 4f;
    [SerializeField] private DeathSpell spellPrefab;
    [SerializeField] private int spellDamage = 12;
    [SerializeField] private int phase2SpellCount = 3;
    [SerializeField] private float spellSpacing = 2.5f;
    [SerializeField] private LayerMask groundLayer;

    private float nextCastTime;

    protected override void Awake()
    {
        base.Awake();
        nextCastTime = Time.time + 2f;
    }

    protected override bool TryRangedAttack(float distanceToPlayer)
    {
        if (distanceToPlayer < castMinDistance || Time.time < nextCastTime) return false;
        nextCastTime = Time.time + castCooldown * CooldownMultiplier;
        animator.SetTrigger(CastHash);
        AudioManager.Play(AudioManager.Sfx.BossCharge);
        return true;
    }

    protected override bool IsBusy() => StateIs("Attack") || StateIs("Cast") || StateIs("Hurt");

    // Animation Event do Cast (repassado pelo BossAnimationEvents).
    public void CastSpell()
    {
        if (IsDead || player == null) return;
        int count = Phase2 ? phase2SpellCount : 1;
        for (int i = 0; i < count; i++)
        {
            float offset = (i - (count - 1) / 2f) * spellSpacing; // 1 magia: no jogador; 3: centro e lados
            Vector2 target = new Vector2(player.position.x + offset, player.position.y + 0.5f);
            RaycastHit2D ground = Physics2D.Raycast(target, Vector2.down, 10f, groundLayer);
            if (ground) target = ground.point;
            DeathSpell spell = Instantiate(spellPrefab, target, Quaternion.identity);
            spell.Init(spellDamage, playerLayer);
        }
        AudioManager.Play(AudioManager.Sfx.BossBeam);
    }
}
