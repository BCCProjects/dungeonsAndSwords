using System.Collections;
using UnityEngine;

// Mostra que o golpe entrou: o personagem pisca e o número do dano sobe da cabeça dele.
[RequireComponent(typeof(Health))]
public class HitFeedback : MonoBehaviour
{
    [SerializeField] private Color flashColor = new Color(1f, 0.25f, 0.25f);
    [SerializeField] private float flashTime = 0.12f;
    [SerializeField] private float numberHeight = 1.6f;
    [SerializeField] private Color numberColor = Color.white;
    [SerializeField] private AudioManager.Sfx hitSound = AudioManager.Sfx.Hit;

    private SpriteRenderer[] renderers;
    private Color[] originalColors;
    private Coroutine flashRoutine;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++) originalColors[i] = renderers[i].color;
        GetComponent<Health>().Damaged += OnDamaged;
    }

    private void OnDamaged(int amount)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(Flash());
        FloatingText.Spawn(amount.ToString(), transform.position + Vector3.up * numberHeight, numberColor);
        AudioManager.Play(hitSound);
    }

    private IEnumerator Flash()
    {
        for (int i = 0; i < renderers.Length; i++) renderers[i].color = flashColor;
        yield return new WaitForSeconds(flashTime);
        for (int i = 0; i < renderers.Length; i++) renderers[i].color = originalColors[i];
    }
}
