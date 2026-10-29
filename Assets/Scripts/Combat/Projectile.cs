using UnityEngine;

// Projétil em linha reta (flecha, bola de fogo). Causa dano no primeiro alvo e some ao bater no cenário.
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private LayerMask obstacleLayers;

    private int damage;
    private LayerMask targetLayers;

    public void Launch(Vector2 direction, int damageAmount, LayerMask targets)
    {
        damage = damageAmount;
        targetLayers = targets;
        GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * speed;

        if (direction.x < 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;         // ignora áreas de visão e outros sensores
        if (other.usedByEffector) return;    // atravessa plataformas de mão única

        int layerBit = 1 << other.gameObject.layer;
        if ((targetLayers & layerBit) != 0)
        {
            if (other.TryGetComponent(out Health target))
                target.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if ((obstacleLayers & layerBit) != 0)
        {
            Destroy(gameObject);
        }
    }
}
