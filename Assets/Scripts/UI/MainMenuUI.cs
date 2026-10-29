using UnityEngine;

// Botões do MainMenu (ligados pelo OnClick no Inspector).
public class MainMenuUI : MonoBehaviour
{
    public void Play() => SceneLoader.LoadClassSelect();
    public void OpenCredits() => SceneLoader.LoadCredits();
    public void Quit() => SceneLoader.QuitGame();
}
