using System.Collections;
using UnityEngine;

// Regra de fim de fase: derrotar todos os inimigos.
// Antes do painel "Avançar / Voltar ao menu", o jogador termina as escolhas de level up,
// coleta os itens que ficaram no chão e tem alguns segundos para ver que a fase acabou.
// Na fase final, limpar a fase faz o chefe aparecer.
public class LevelManager : MonoBehaviour
{
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private UIManager ui;
    [Tooltip("Segundos entre 'Fase concluída!' e o painel de próxima fase.")]
    [SerializeField] private float completeDelay = 2.5f;

    [Header("Fase final")]
    [Tooltip("Chefe desativado na cena; aparece quando os outros inimigos morrem. Vazio nas fases 1 e 2.")]
    [SerializeField] private GameObject boss;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private float victoryDelay = 3f;

    private int remainingEnemies;
    private bool enemiesCleared;
    private bool completed;
    private float readyTime; // tempo (sem pausa) desde que tudo ficou pronto

    private void OnEnable() => EnemyBase.AnyEnemyDied += OnEnemyDied;
    private void OnDisable() => EnemyBase.AnyEnemyDied -= OnEnemyDied;

    private void Start()
    {
        // Fase aberta direto no editor: avisa o GameManager em qual fase estamos.
        if (GameManager.Instance.CurrentLevel != levelNumber)
            GameManager.Instance.StartLevel(levelNumber);

        // Conta só os inimigos ativos: o chefe começa desativado e não entra na conta.
        remainingEnemies = FindObjectsByType<EnemyBase>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
    }

    private void OnEnemyDied(EnemyBase enemy)
    {
        if (boss != null && enemy.gameObject == boss)
        {
            StartCoroutine(VictoryAfterDelay());
            return;
        }

        remainingEnemies--;
        if (remainingEnemies > 0) return;

        if (boss != null)
        {
            boss.SetActive(true);
            ui.ShowBossBar(boss.GetComponent<Health>(), boss.GetComponent<EnemyBase>().Data.displayName);
            if (bossMusic != null) AudioManager.PlayMusic(bossMusic);
        }
        else enemiesCleared = true;
    }

    private void Update()
    {
        if (!enemiesCleared || completed) return;

        // Espera o painel de level up fechar e os drops serem coletados.
        if (Time.timeScale == 0f) return;
        if (ItemPickup.ActiveCount > 0)
        {
            readyTime = 0f;
            ui.ShowLevelMessage($"Inimigos derrotados!\nColete os itens ({ItemPickup.ActiveCount})");
            return;
        }

        // Tudo pronto: mostra o aviso por alguns segundos antes do painel.
        ui.ShowLevelMessage("Fase concluída!");
        readyTime += Time.deltaTime;
        if (readyTime < completeDelay) return;

        completed = true;
        ui.ShowLevelComplete();
    }

    private IEnumerator VictoryAfterDelay()
    {
        ui.HideBossBar();
        ui.ShowLevelMessage($"{boss.GetComponent<EnemyBase>().Data.displayName} foi derrotado!");
        yield return new WaitForSeconds(victoryDelay);
        SceneLoader.LoadVictory();
    }
}
