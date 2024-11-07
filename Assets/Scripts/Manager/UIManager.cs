using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIManager : SingletonPersistent<UIManager>
{
    [SerializeField]
    private GameObject _gameOverScreen, _loadingScreen, _powerUpsPanel, _settingsPanel,
                    _revealWordPanel, _replaceTilePanel;
    [SerializeField] private GameObject _toggleSFXButton, _toggleMusicButton;
    [SerializeField] private Sprite _sfxOn, _sfxOff, _musicOn, _musicOff;
    [SerializeField] private TMP_InputField _replaceLetter;
    [SerializeField] private TextMeshProUGUI _revealText;

    private bool _isInspectingBoard, _isInteractable = true;
    public bool IsInspectingBoard
    {
        get => _isInspectingBoard;
        set => _isInspectingBoard = value;
    }
    public bool IsInteractable
    {
        get => _isInteractable;
        set => _isInteractable = value;
    }

    public void Initialize()
    {
        UpdateSettingsUI();
    }

    public void LoadGameScene()
    {
        ToggleLoadingScreen();
        GameManager.Instance.NewGame();

        Addressables.LoadSceneAsync("Assets/Scenes/Gameplay.unity").Completed += handle =>
        {
            PlayerStatsManager.Instance.LoadNames();
        };
    }

    public void ToggleLoadingScreen()
    {
        _loadingScreen.SetActive(!_loadingScreen.activeSelf);
    }

    public void ToUpper()
    {
        _replaceLetter.text = _replaceLetter.text.ToUpper();
    }

    public void ToggleGameOverScreen()
    {
        _isInteractable = !_isInteractable;
        _gameOverScreen.SetActive(!_gameOverScreen.activeSelf);
    }

    public void TogglePowerUpsPanel()
    {
        _isInteractable = !_isInteractable;
        _powerUpsPanel.SetActive(!_powerUpsPanel.activeSelf);
    }

    public void ToggleSettingsScreen()
    {
        _isInteractable = !_isInteractable;
        _settingsPanel.SetActive(!_settingsPanel.activeSelf);
    }

    public void ToggleInspectBoard()
    {
        _isInspectingBoard = !_isInspectingBoard;
        TogglePowerUpsPanel();
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

    public void SetRevealedText(string text)
    {
        _revealText.text = $"The longest word available is {text.ToUpper()}";
    }

    public void ToggleRevealWordPopUp()
    {
        _revealWordPanel.SetActive(!_revealWordPanel.activeSelf);
    }

    public void ToggleTileReplacePopUp()
    {
        _replaceTilePanel.SetActive(!_replaceTilePanel.activeSelf);
    }
}
