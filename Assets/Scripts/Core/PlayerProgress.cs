// Tudo que o jogador conquista e leva de uma fase para a outra.
[System.Serializable]
public class PlayerProgress
{
    public int level = 1;
    public int xp;

    // Atributos comprados no level up
    public int bonusMaxHealth;               // Vida
    public int bonusDamage;                  // Força
    public float cooldownMultiplier = 1f;    // Agilidade (menor = ataca mais rápido)
    public float damageReduction;            // Resistência (0,2 = recebe 20% menos dano)

    public PlayerProgress Clone() => (PlayerProgress)MemberwiseClone();
}
