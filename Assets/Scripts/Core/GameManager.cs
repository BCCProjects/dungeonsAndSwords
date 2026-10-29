using UnityEngine;

// Objeto persistente entre cenas: guarda a classe escolhida e o progresso da partida.
public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            // Cria sob demanda, para que qualquer cena possa ser testada direto no Play.
            if (instance == null)
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            return instance;
        }
    }

    private PlayerProgress levelStartProgress = new PlayerProgress();

    public ClassData SelectedClass { get; private set; }
    public int CurrentLevel { get; private set; }
    public PlayerProgress Progress { get; private set; } = new PlayerProgress();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SelectClass(ClassData classData)
    {
        SelectedClass = classData;
    }

    // Ao entrar numa fase, guarda uma cópia do progresso para o "Tentar de novo".
    public void StartLevel(int level)
    {
        CurrentLevel = level;
        levelStartProgress = Progress.Clone();
    }

    // "Tentar de novo": volta ao progresso que o jogador tinha ao entrar na fase.
    public void RestoreLevelStartProgress()
    {
        Progress = levelStartProgress.Clone();
    }

    // Chamado ao voltar ao menu: a partida recomeça do zero.
    public void ResetProgress()
    {
        SelectedClass = null;
        CurrentLevel = 0;
        Progress = new PlayerProgress();
        levelStartProgress = new PlayerProgress();
    }
}
