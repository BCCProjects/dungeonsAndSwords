using UnityEngine;

// Item no chão: flutua de leve e aplica o bônus quando o jogador encosta.
[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData item;
    [SerializeField] private float bobHeight = 0.12f;
    [SerializeField] private float bobSpeed = 3f;

    private Vector3 startPosition;

    // Quantos itens ainda estão no chão (a fase só termina quando chega a zero).
    public static int ActiveCount { get; private set; }

    private void OnEnable() => ActiveCount++;
    private void OnDisable() => ActiveCount--;

    public void Setup(ItemData data)
    {
        item = data;
        GetComponent<SpriteRenderer>().sprite = data.icon;
    }

    private void Start() => startPosition = transform.position;

    private void Update()
    {
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.TryGetComponent(out LevelSystem levelSystem))
        {
            levelSystem.ApplyItem(item);
            AudioManager.Play(AudioManager.Sfx.ItemPickup);
            // Deixa claro o que foi pego e o que faz
            FloatingText.Spawn($"{item.displayName}\n<size=70%>{item.description}</size>", other.transform.position + Vector3.up * 2.2f, item.color, 3.5f);
            Destroy(gameObject);
        }
    }
}
