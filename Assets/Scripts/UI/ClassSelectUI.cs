using UnityEngine;

// Tela de escolha de classe: guarda a escolha no GameManager e inicia a Level1.
public class ClassSelectUI : MonoBehaviour
{
    public void SelectClass(ClassData classData)
    {
        GameManager.Instance.SelectClass(classData);
        SceneLoader.LoadLevel(1);
    }

    public void Back() => SceneLoader.LoadMainMenu();
}
