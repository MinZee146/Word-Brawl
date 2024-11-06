using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonPersistent<UIManager>
{
    [SerializeField] private GameObject _gameOverScreen, _loadingScreen, _powerupsPanel;
    [SerializeField] private TMP_InputField _replaceLetter;

    private TextMeshProUGUI _playerName;

    public void LoadGameScene()
    {
        var asyncLoad = SceneManager.LoadSceneAsync("Gameplay");
        asyncLoad.completed += (operation) => PlayerStatsManager.Instance.LoadNames();

        ToggleLoadingScreen();
        GameManager.Instance.NewGame();
    }

    public void ToggleGameOverScreen()
    {
        _gameOverScreen.SetActive(!_gameOverScreen.activeSelf);
    }

    public void ToggleLoadingScreen()
    {
        _loadingScreen.SetActive(!_loadingScreen.activeSelf);
    }

    public void ToUpper()
    {
        _replaceLetter.text = _replaceLetter.text.ToUpper();
    }

    public void TogglePowerupsPanel()
    {
        _powerupsPanel.SetActive(!_powerupsPanel.activeSelf);
    }
}
