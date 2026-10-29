using UnityEngine;

// Quando um inimigo morre, sorteia os itens possíveis (cada um com seu chanceDrop)
// e solta no máximo um, apoiado no chão abaixo de onde o inimigo caiu.
public class DropSystem : MonoBehaviour
{
    [SerializeField] private ItemData[] possibleDrops;
    [SerializeField] private ItemPickup pickupPrefab;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float heightAboveGround = 0.5f;

    private void OnEnable() => EnemyBase.AnyEnemyDied += OnEnemyDied;
    private void OnDisable() => EnemyBase.AnyEnemyDied -= OnEnemyDied;

    private void OnEnemyDied(EnemyBase enemy)
    {
        foreach (ItemData item in possibleDrops)
        {
            if (Random.value <= item.chanceDrop)
            {
                Spawn(item, enemy.transform.position);
                return;
            }
        }
    }

    // Drop garantido (usado pelo chefe durante a luta): sorteia um dos itens possíveis.
    public void DropRandom(Vector3 position)
    {
        Spawn(possibleDrops[Random.Range(0, possibleDrops.Length)], position);
    }

    private void Spawn(ItemData item, Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 20f, groundLayer);
        if (hit) position = hit.point + Vector2.up * heightAboveGround;

        ItemPickup pickup = Instantiate(pickupPrefab, position, Quaternion.identity);
        pickup.Setup(item);
    }
}
