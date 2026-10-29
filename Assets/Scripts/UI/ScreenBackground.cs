using UnityEngine;

// Fundo animado das telas de menu: escala o sprite para cobrir toda a câmera,
// em qualquer proporção de tela, sem distorcer a imagem.
[RequireComponent(typeof(SpriteRenderer))]
public class ScreenBackground : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Camera cam;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    // Em LateUpdate porque a janela pode mudar de tamanho e o sprite troca a cada quadro da animação.
    private void LateUpdate()
    {
        if (spriteRenderer.sprite == null) return;
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        float viewHeight = cam.orthographicSize * 2f;
        float viewWidth = viewHeight * cam.aspect;
        float scale = Mathf.Max(viewWidth / spriteSize.x, viewHeight / spriteSize.y);
        transform.localScale = new Vector3(scale, scale, 1f);
        transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
    }
}
