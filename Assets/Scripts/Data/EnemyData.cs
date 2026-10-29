using UnityEngine;

// Atributos de um tipo de inimigo (Goblin, Esqueleto, Cogumelo, Olho Voador...).
[CreateAssetMenu(fileName = "EnemyData", menuName = "Dungeons & Swords/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string displayName;

    [Header("Combate")]
    public int maxHealth = 12;
    public int damage = 2;
    public float attackRange = 1f;
    public float attackCooldown = 1.2f;

    [Header("Movimento")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float visionRange = 6f;

    [Header("Recompensa")]
    public int xpReward = 10;
}
