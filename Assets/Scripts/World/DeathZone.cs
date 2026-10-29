using UnityEngine;

// Área abaixo do mapa: quem cair num buraco morre.
[RequireComponent(typeof(Collider2D))]
public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;
        if (other.TryGetComponent(out Health health))
            health.Kill();
    }
}
