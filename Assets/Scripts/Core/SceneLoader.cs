using UnityEngine;
using UnityEngine.SceneManagement;

// Ponto único de troca de cenas. Os nomes batem com as cenas do Build Profiles.
public static class SceneLoader
{
    public const string MainMenu = "MainMenu";
    public const string Credits = "Credits";
    public const string ClassSelect = "ClassSelect";
    public const string Victory = "Victory";
    public const string GameOver = "GameOver";
    public const int LastLevel = 3;

    public static void LoadMainMenu()
    {
        GameManager.Instance.ResetProgress();
        Load(MainMenu);
    }

    public static void LoadCredits() => Load(Credits);
    public static void LoadClassSelect() => Load(ClassSelect);
    public static void LoadVictory() => Load(Victory);
    public static void LoadGameOver() => Load(GameOver);

    public static void LoadLevel(int level)
    {
        GameManager.Instance.StartLevel(level);
        Load("Level" + level);
    }

    public static void LoadNextLevel()
    {
        int next = GameManager.Instance.CurrentLevel + 1;
        if (next > LastLevel)
            LoadVictory();
        else
            LoadLevel(next);
    }

    // "Tentar de novo": recarrega a fase com o progresso que o jogador tinha ao entrar nela.
    public static void ReloadCurrentLevel()
    {
        GameManager.Instance.RestoreLevelStartProgress();
        LoadLevel(Mathf.Max(1, GameManager.Instance.CurrentLevel));
    }

    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private static void Load(string sceneName)
    {
        Time.timeScale = 1f; // garante que o pause não "vaze" para a próxima cena
        SceneManager.LoadScene(sceneName);
    }
}
