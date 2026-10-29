using UnityEngine;

// Item que pode cair de um inimigo. O efeito é aplicado ao pegar (sem inventário).
[CreateAssetMenu(fileName = "ItemData", menuName = "Dungeons & Swords/Item Data")]
public class ItemData : ScriptableObject
{
    public enum ItemType { Heal, Experience, Strength, Agility, Resistance }

    public string displayName;
    [Tooltip("Mostrado na tela quando o jogador pega o item.")]
    public string description;
    public Sprite icon;
    public Color color = Color.white;

    [Header("Efeito")]
    public ItemType type;
    [Tooltip("Cura: vida | XP: pontos | Força: dano extra | Agilidade e Resistência: fração (0,3 = 30%)")]
    public float amount;
    [Tooltip("Duração em segundos (0 = efeito imediato).")]
    public float duration;

    [Header("Drop")]
    [Range(0f, 1f)] public float chanceDrop = 0.2f;

    public bool IsTimed => duration > 0f;
}
