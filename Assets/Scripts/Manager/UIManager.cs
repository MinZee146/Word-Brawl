using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonPersistent<UIManager>
{
    [SerializeField] private GameObject _gameOverScreen, _loadingScreen;

    public void LoadGame()
    {
        SceneManager.LoadScene("Gameplay");
        ToggleLoadingScreen();
    }

    public void ToggleGameOverScreen()
    {
        _gameOverScreen.SetActive(!_gameOverScreen.activeSelf);
    }

    public void ToggleLoadingScreen()
    {
        _loadingScreen.SetActive(!_loadingScreen.activeSelf);
    }
}
