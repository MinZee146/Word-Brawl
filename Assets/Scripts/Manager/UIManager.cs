using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : SingletonPersistent<UIManager>
{
    [SerializeField] private GameObject _gameOverScreen, _loadingScreen, _powerupsPanel, _settingsPanel;
    [SerializeField] private GameObject _toggleSFXButton, _toggleMusicButton;
    [SerializeField] private Sprite _sfxOn, _sfxOff, _musicOn, _musicOff;
    [SerializeField] private TMP_InputField _replaceLetter;

    public void Initialize()
    {
        UpdateSettingsUI();
    }

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

    public void ToggleSettingsScreen()
    {
        _settingsPanel.SetActive(!_settingsPanel.activeSelf);
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
        var isSfxOn = _toggleSFXButton.GetComponent<Image>().sprite == _sfxOn;
        _toggleSFXButton.GetComponent<Image>().sprite = isSfxOn ? _sfxOff : _sfxOn;

        PlayerPrefs.SetInt("IsSFXOn", isSfxOn ? 0 : 1);
        PlayerPrefs.Save();
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
        var isMusicOn = _toggleMusicButton.GetComponent<Image>().sprite == _musicOn;
        _toggleMusicButton.GetComponent<Image>().sprite = isMusicOn ? _musicOff : _musicOn;

        PlayerPrefs.SetInt("IsMusicOn", isMusicOn ? 0 : 1);
        PlayerPrefs.Save();
    }

    private void UpdateSettingsUI()
    {
        var sfxState = PlayerPrefs.GetInt("IsSFXOn", 1);
        _toggleSFXButton.GetComponent<Image>().sprite = sfxState == 1 ? _sfxOn : _sfxOff;

        var musicState = PlayerPrefs.GetInt("IsMusicOn", 1);
        _toggleMusicButton.GetComponent<Image>().sprite = musicState == 1 ? _musicOn : _musicOff;
    }
}
