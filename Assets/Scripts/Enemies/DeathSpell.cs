using System.Collections.Generic;
using UnityEngine;

// Magia do Bringer of Death: o portal abre no chão (aviso), a mão desce e acerta quem estiver ali.
// O dano e o fim da magia vêm de Animation Events do próprio clip.
public class DeathSpell : MonoBehaviour
{
    [SerializeField] private Vector2 hitSize = new Vector2(1.2f, 2.6f);
    [SerializeField] private Vector2 hitOffset = new Vector2(0f, 1.3f);

    private int damage;
    private LayerMask targets;
    private readonly List<Collider2D> hits = new List<Collider2D>();

    public void Init(int damageAmount, LayerMask targetLayers)
    {
        damage = damageAmount;
        targets = targetLayers;
    }

    // Animation Event: quadro em que a mão chega ao chão.
    public void SpellHit()
    {
        var filter = new ContactFilter2D { useTriggers = false };
        filter.SetLayerMask(targets);
        Physics2D.OverlapBox((Vector2)transform.position + hitOffset, hitSize, 0f, filter, hits);
        foreach (Collider2D hit in hits)
            if (hit.TryGetComponent(out Health target))
                target.TakeDamage(damage);
    }

    // Animation Event: último quadro.
    public void SpellEnd() => Destroy(gameObject);

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.6f, 0.2f, 1f);
        Gizmos.DrawWireCube((Vector2)transform.position + hitOffset, hitSize);
    }
}
