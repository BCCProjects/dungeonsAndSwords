using UnityEngine;

// Cria o jogador da classe escolhida no ponto de spawn da fase.
public class PlayerSpawner : MonoBehaviour
{
    [Tooltip("Usada quando a fase é aberta direto no editor, sem passar pela ClassSelect.")]
    [SerializeField] private ClassData defaultClass;
    [SerializeField] private CameraFollow cameraFollow;

    private void Awake()
    {
        ClassData chosen = GameManager.Instance.SelectedClass != null ? GameManager.Instance.SelectedClass : defaultClass;
        GameObject player = Instantiate(chosen.playerPrefab, transform.position, Quaternion.identity);
        player.name = "Player";

        // Cada pacote tem o pivot num lugar diferente: alinha os pés ao ponto de spawn.
        float feetY = player.GetComponent<Collider2D>().bounds.min.y;
        player.transform.position += Vector3.up * (transform.position.y - feetY);

        if (cameraFollow != null)
            cameraFollow.SetTarget(player.transform);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + Vector3.up * 0.75f, new Vector3(0.8f, 1.5f, 0f));
    }
}
