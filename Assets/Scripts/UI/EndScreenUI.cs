using TMPro;
using UnityEngine;

// Telas de Vitória e de Derrota: resumo da partida e botões.
public class EndScreenUI : MonoBehaviour
{
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private AudioManager.Sfx jingle = AudioManager.Sfx.Victory;

    private void Start()
    {
        Time.timeScale = 1f;
        AudioManager.Play(jingle);
        var gm = GameManager.Instance;
        string className = gm.SelectedClass != null ? gm.SelectedClass.displayName : "Aventureiro";
        string levelName = gm.CurrentLevel > 0 ? $"Fase {gm.CurrentLevel}" : "-";
        summaryText.text = $"{className}  -  Nível {gm.Progress.level}\n{levelName}";
    }

    public void Retry() => SceneLoader.ReloadCurrentLevel();
    public void BackToMenu() => SceneLoader.LoadMainMenu();
}
