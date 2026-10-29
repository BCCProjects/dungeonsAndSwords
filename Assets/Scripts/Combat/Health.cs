using System;
using UnityEngine;

// Vida de jogador ou inimigo. Avisa o Animator (Hurt/Die) e quem estiver ouvindo os eventos.
public class Health : MonoBehaviour
{
    private static readonly int HurtHash = Animator.StringToHash("Hurt");
    private static readonly int DieHash = Animator.StringToHash("Die");

    [SerializeField] private int maxHealth = 10;
    [SerializeField] private float invulnerabilityTime = 0.4f;

    private Animator animator;
    private float invulnerableUntil;

    public int Current { get; private set; }
    public int Max => maxHealth;
    public bool IsDead { get; private set; }
    public float DamageReduction { get; set; } // Resistência do jogador (0,2 = recebe 20% menos)

    public event Action<int, int> HealthChanged; // (atual, máximo)
    public event Action<int> Damaged;            // dano que realmente entrou (piscar, números)
    public event Action Died;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Current = maxHealth;
    }

    public void SetMaxHealth(int value, bool refill)
    {
        maxHealth = value;
        Current = refill ? value : Mathf.Min(Current, value);
        HealthChanged?.Invoke(Current, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || Time.time < invulnerableUntil) return;

        amount = Mathf.Max(1, Mathf.RoundToInt(amount * (1f - DamageReduction)));
        Current = Mathf.Max(0, Current - amount);
        invulnerableUntil = Time.time + invulnerabilityTime;
        HealthChanged?.Invoke(Current, maxHealth);
        Damaged?.Invoke(amount);

        if (Current == 0)
            Die();
        else if (animator != null)
            animator.SetTrigger(HurtHash);
    }

    // Morte imediata (queda num buraco), ignorando a invulnerabilidade.
    public void Kill()
    {
        if (IsDead) return;
        Current = 0;
        HealthChanged?.Invoke(Current, maxHealth);
        Die();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        Current = Mathf.Min(maxHealth, Current + amount);
        HealthChanged?.Invoke(Current, maxHealth);
    }

    private void Die()
    {
        IsDead = true;
        if (animator != null) animator.SetTrigger(DieHash);
        Died?.Invoke();
    }
}
