using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonPersistent<UIManager>
{
    [SerializeField] private GameObject _gameOverScreen;
    public void LoadGame()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void ToggleGameOverScreen()
    {
        _gameOverScreen.SetActive(!_gameOverScreen.activeSelf);
    }
}
