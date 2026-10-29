using TMPro;
using UnityEngine;

// Card de uma classe na ClassSelect: preenche os textos a partir do ClassData.
public class ClassCardUI : MonoBehaviour
{
    [SerializeField] private ClassData classData;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statsText;

    private void Start()
    {
        nameText.text = classData.displayName;
        statsText.text =
            $"{classData.description}\n\n" +
            $"Vida: {classData.maxHealth}\n" +
            $"Dano: {classData.baseDamage}\n" +
            $"Ataque: {(classData.isRanged ? "à distância" : "corpo a corpo")}\n" +
            $"Recarga: {classData.attackCooldown:0.##}s";
    }
}
