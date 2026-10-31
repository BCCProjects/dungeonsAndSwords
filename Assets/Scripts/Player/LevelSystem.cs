using System;
using UnityEngine;

// XP, nível e atributos do jogador. Os números ficam no GameManager (PlayerProgress),
// por isso o que o jogador conquista numa fase continua valendo na próxima.
// Os atributos finais = classe + level up + poções ativas (PlayerEffects).
[RequireComponent(typeof(PlayerController), typeof(PlayerCombat), typeof(Health))]
public class LevelSystem : MonoBehaviour
{
    public enum Attribute { Health, Strength, Agility, Resistance }

    [Header("XP")]
    [SerializeField] private int baseXpToLevel = 20;
    [SerializeField] private int extraXpPerLevel = 15;

    [Header("Ganho por escolha")]
    [SerializeField] private int healthPerLevel = 5;
    [SerializeField] private int strengthPerLevel = 2;
    [SerializeField] private float agilityPerLevel = 0.1f;      // +10% de velocidade
    [SerializeField] private float resistancePerLevel = 0.08f;  // -8% de dano recebido

    [Header("Agilidade no movimento")]
    [Tooltip("Quanto do bônus de velocidade vai para o movimento (0,5 = metade: +10% no ataque vira +5% andando).")]
    [SerializeField] private float moveSpeedShare = 0.5f;
    [SerializeField] private float maxMoveSpeedMultiplier = 1.35f;

    [Header("Limites")]
    [SerializeField] private float maxSpeedMultiplier = 2f;
    [SerializeField] private float maxDamageReduction = 0.6f;

    private PlayerController controller;
    private PlayerCombat combat;
    private Health health;
    private PlayerEffects effects;
    private int pendingLevelUps;

    public event Action<int, int, int> XPChanged; // (xp atual, xp para o próximo nível, nível)
    public event Action<int> LevelUpReady;       // abre o painel de escolha
    public event Action StatsChanged;            // HUD atualiza dano, agilidade e resistência
    public event Action<ItemData> ItemCollected; // HUD mostra o que o item fez

    private PlayerProgress Progress => GameManager.Instance.Progress;
    public int Level => Progress.level;
    public int XpToNextLevel => baseXpToLevel + (Progress.level - 1) * extraXpPerLevel;
    public string ClassName => controller.ClassData.displayName;
    public int Damage => combat.Damage;
    public float Speed => combat.SpeedMultiplier; // 1,25 = 25% mais rápido
    public float Resistance => health.DamageReduction;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        combat = GetComponent<PlayerCombat>();
        health = GetComponent<Health>();
        effects = GetComponent<PlayerEffects>();
    }

    private void OnEnable() => EnemyBase.AnyEnemyDied += OnEnemyDied;
    private void OnDisable() => EnemyBase.AnyEnemyDied -= OnEnemyDied;

    private void Start()
    {
        ApplyStats(true);
        XPChanged?.Invoke(Progress.xp, XpToNextLevel, Progress.level);
    }

    private void OnEnemyDied(EnemyBase enemy) => AddXP(enemy.Data.xpReward);

    public void AddXP(int amount)
    {
        if (amount <= 0) return;
        Progress.xp += amount;
        while (Progress.xp >= XpToNextLevel)
        {
            Progress.xp -= XpToNextLevel;
            Progress.level++;
            pendingLevelUps++;
        }
        XPChanged?.Invoke(Progress.xp, XpToNextLevel, Progress.level);

        if (pendingLevelUps > 0 && Time.timeScale > 0f)
            RequestChoice();
    }

    // Pausa o jogo e pede para a UI mostrar as opções.
    private void RequestChoice()
    {
        Time.timeScale = 0f;
        AudioManager.Play(AudioManager.Sfx.LevelUp);
        LevelUpReady?.Invoke(Progress.level);
    }

    public void ChooseAttribute(Attribute attribute)
    {
        switch (attribute)
        {
            case Attribute.Health:
                Progress.bonusMaxHealth += healthPerLevel;
                ApplyStats(false);
                health.Heal(healthPerLevel);
                break;
            case Attribute.Strength:
                Progress.bonusDamage += strengthPerLevel;
                break;
            case Attribute.Agility:
                Progress.speedMultiplier = Mathf.Min(maxSpeedMultiplier, Progress.speedMultiplier + agilityPerLevel);
                break;
            case Attribute.Resistance:
                Progress.damageReduction = Mathf.Min(maxDamageReduction, Progress.damageReduction + resistancePerLevel);
                break;
        }
        ApplyStats(false);

        pendingLevelUps--;
        if (pendingLevelUps > 0) RequestChoice();
        else Time.timeScale = 1f;
    }

    // Item coletado: cura e XP são imediatos; poções entram como efeito com tempo.
    public void ApplyItem(ItemData item)
    {
        switch (item.type)
        {
            case ItemData.ItemType.Heal: health.Heal(Mathf.RoundToInt(item.amount)); break;
            case ItemData.ItemType.Experience: AddXP(Mathf.RoundToInt(item.amount)); break;
            default: effects.Add(item); break;
        }
        ItemCollected?.Invoke(item);
    }

    public void RefreshStats() => ApplyStats(false);

    private void ApplyStats(bool refillHealth)
    {
        health.SetMaxHealth(controller.ClassData.maxHealth + Progress.bonusMaxHealth, refillHealth);
        combat.DamageBonus = Progress.bonusDamage + effects.BonusDamage;
        float speed = Mathf.Min(maxSpeedMultiplier, Progress.speedMultiplier + effects.SpeedBonus);
        combat.SpeedMultiplier = speed;
        controller.MoveSpeedMultiplier = Mathf.Min(maxMoveSpeedMultiplier, 1f + (speed - 1f) * moveSpeedShare);
        health.DamageReduction = Mathf.Min(maxDamageReduction, Progress.damageReduction + effects.DamageReduction);
        StatsChanged?.Invoke();
    }
}
