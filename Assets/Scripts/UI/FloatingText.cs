using TMPro;
using UnityEngine;

// Texto que sobe e some no mundo: números de dano e o nome/efeito do item coletado.
public class FloatingText : MonoBehaviour
{
    private const float Lifetime = 1.2f;
    private const float RiseSpeed = 1.2f;

    private TextMeshPro label;
    private float age;

    public static void Spawn(string text, Vector3 position, Color color, float size = 4f)
    {
        var go = new GameObject("FloatingText");
        go.transform.position = position;
        var tmp = go.AddComponent<TextMeshPro>(); // usa a fonte padrão (BoldPixels PT)
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.outlineWidth = 0.25f;
        tmp.outlineColor = Color.black;
        tmp.sortingOrder = 50;
        tmp.rectTransform.sizeDelta = new Vector2(8f, 2f);
        go.AddComponent<FloatingText>().label = tmp;
    }

    private void Update()
    {
        age += Time.deltaTime;
        transform.position += Vector3.up * RiseSpeed * Time.deltaTime;
        Color c = label.color;
        c.a = Mathf.Clamp01(1f - (age - Lifetime * 0.6f) / (Lifetime * 0.4f));
        label.color = c;
        if (age >= Lifetime) Destroy(gameObject);
    }
}
