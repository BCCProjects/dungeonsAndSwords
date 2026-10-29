using UnityEngine;

// Camada de fundo com parallax: quanto menor o fator, mais "longe" ela parece.
// A imagem é desenhada em modo Tiled (3 larguras) e se reposiciona para nunca acabar.
[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("0 = acompanha a câmera (bem longe), 1 = parada no mundo (bem perto)")]
    [SerializeField, Range(0f, 1f)] private float parallaxFactor = 0.5f;
    [SerializeField] private float verticalOffset;

    private Transform cam;
    private float startX;
    private float tileWidth;

    private void Start()
    {
        cam = Camera.main.transform;
        startX = transform.position.x;
        tileWidth = GetComponent<SpriteRenderer>().bounds.size.x / 3f;
    }

    private void LateUpdate()
    {
        float travelled = cam.position.x * (1f - parallaxFactor); // quanto a camada "anda junto"
        float relative = cam.position.x * parallaxFactor;         // quanto a câmera se afastou dela

        transform.position = new Vector3(startX + travelled, cam.position.y + verticalOffset, transform.position.z);

        // Passou de uma largura inteira: pula a camada para frente/trás (efeito infinito).
        if (relative > startX + tileWidth) startX += tileWidth;
        else if (relative < startX - tileWidth) startX -= tileWidth;
    }
}
