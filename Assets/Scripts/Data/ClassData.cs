using UnityEngine;

// Dados fixos de uma classe jogável (Guerreiro, Arqueiro, Mago).
[CreateAssetMenu(fileName = "ClassData", menuName = "Dungeons & Swords/Class Data")]
public class ClassData : ScriptableObject
{
    [Header("Identificação")]
    public string displayName;
    [TextArea] public string description;

    [Header("Atributos base")]
    public int maxHealth = 20;
    public int baseDamage = 5;
    public float moveSpeed = 5f;
    public float attackCooldown = 0.5f;
    public bool isRanged;

    [Header("Prefab")]
    public GameObject playerPrefab;
}
