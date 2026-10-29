using UnityEngine;

// Câmera que segue o jogador suavemente, presa aos limites do mapa.
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset = new Vector2(0f, 1.5f);
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Limites do mapa")]
    [SerializeField] private bool useBounds;
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Vector2 maxPosition;

    private Vector3 velocity;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        transform.position = GetDesiredPosition();
    }

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.SmoothDamp(transform.position, GetDesiredPosition(), ref velocity, smoothTime);
    }

    private Vector3 GetDesiredPosition()
    {
        Vector3 desired = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        if (useBounds)
        {
            desired.x = Mathf.Clamp(desired.x, minPosition.x, maxPosition.x);
            desired.y = Mathf.Clamp(desired.y, minPosition.y, maxPosition.y);
        }
        return desired;
    }
}
