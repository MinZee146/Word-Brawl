using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIManager : SingletonPersistent<UIManager>
{
    [SerializeField] private GameObject _menuUI, _gameOverPanel, _powerUpsPanel, _settingsPanel;
    [SerializeField] private GameObject _opponentPowerUp, _revealWordPanel, _replaceTilePanel;
    [SerializeField] private GameObject _toggleSFXButton, _toggleMusicButton, _inspectButton, _okReplaceButton, _okRevealButton;
    [SerializeField] private Sprite _sfxOn, _sfxOff, _musicOn, _musicOff;
    [SerializeField] private TMP_InputField _replaceLetter;
    [SerializeField] private TextMeshProUGUI _revealText, _descriptionPowerUp;
    [SerializeField] private TextMeshProUGUI _playerName, _opponentName, _playerBestWord, _opponentBestWord;
    [SerializeField] private Image _imagePowerUp;

    private bool _isInspectingBoard, _isInteractable = true;
    private string _inspectPanel;

    public string InspectPanel => _inspectPanel;
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
        _playerName.text = PlayerPrefs.GetString("Username");
    }

    public void SetName(string name)
    {
        _playerName.text = name;
    }

    #region LoadScene
    public void LoadGameScene()
    {
        ToggleLoadingScreen();
        GameManager.Instance.NewGame();

        Addressables.LoadSceneAsync("Assets/Scenes/Gameplay.unity").Completed += handle =>
        {
            PlayerStatsManager.Instance.LoadNames();
        };
    }

    public void LoadMenuScene()
    {
        Addressables.LoadSceneAsync("Assets/Scenes/Main Menu.unity");
        ToggleGameOverPanel();
        ToggleLoadingScreen();
    }
    #endregion

    #region TogglePanel
    public void ToggleLoadingScreen()
    {
        _menuUI.SetActive(!_menuUI.activeSelf);
    }

    public void ToggleGameOverPanel()
    {
        LoadStats();
        _isInteractable = !_isInteractable;
        _gameOverPanel.SetActive(!_gameOverPanel.activeSelf);
    }

    public void TogglePowerUpsPanel()
    {
        _isInteractable = !_isInteractable;
        _powerUpsPanel.SetActive(!_powerUpsPanel.activeSelf);
    }

    public void ToggleSettingsPanel()
    {
        _isInteractable = !_isInteractable;
        _settingsPanel.SetActive(!_settingsPanel.activeSelf);
    }

    public void ToggleInspectPowerUps()
    {
        _isInspectingBoard = !_isInspectingBoard;
        _inspectPanel = "PowerUps";

        TogglePowerUpsPanel();
    }

    public void ToggleInspectReplace()
    {
        _isInspectingBoard = !_isInspectingBoard;
        _inspectPanel = "Replace";

        ToggleTileReplacePopUp();
    }
    #endregion

    #region SettingsUI
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
    #endregion

    #region PowerUpUI
    public void UpdateOpponentPowerUp(string description, Sprite sprite)
    {
        _descriptionPowerUp.text = description;
        _imagePowerUp.sprite = sprite;
    }

    public void ToggleOpponentPowerUpPanel()
    {
        _isInteractable = !_isInteractable;
        _opponentPowerUp.SetActive(!_opponentPowerUp.activeSelf);
    }

    public void SetRevealedText(string text)
    {
        _revealText.text = $"The longest word available is <color=#FF2222>{text.ToUpper()}</color>";
    }

    public void ToggleRevealWordPopUp()
    {
        _isInteractable = !_isInteractable;

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            _okRevealButton.SetActive(true);
        }
        else
        {
            _okRevealButton.SetActive(false);
        }

        _revealWordPanel.SetActive(!_revealWordPanel.activeSelf);
    }

    public void OnReplaceLetterChanged()
    {
        _replaceLetter.text = _replaceLetter.text.ToUpper();

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            ToggleInspectAndOKReplace();
        }
        else
        {
            _okReplaceButton.SetActive(false);
            _inspectButton.SetActive(false);
        }
    }

    public void ToggleTileReplacePopUp()
    {
        _isInteractable = !_isInteractable;

        if (!GameFlowManager.Instance.IsPlayerTurn)
        {
            _replaceLetter.readOnly = true;
            _okReplaceButton.SetActive(false);
            _inspectButton.SetActive(false);
        }
        else
        {
            _replaceLetter.readOnly = false;
            _inspectButton.SetActive(true);
        }

        _replaceLetter.text = "";
        _replaceTilePanel.SetActive(!_replaceTilePanel.activeSelf);
    }

    public void SetReplaceText(char letter)
    {
        _replaceLetter.text = letter.ToString();
    }

    public void ConfirmReplace()
    {
        Board.Instance.ReplaceSelectingTileWith(_replaceLetter.text[0]);
        Board.Instance.ClearHandleTileReplaceListeners();
        ToggleTileReplacePopUp();
        Notifier.Instance.OnTurnChanged();
    }

    private void ToggleInspectAndOKReplace()
    {
        if (string.IsNullOrWhiteSpace(_replaceLetter.text))
        {
            _inspectButton.SetActive(true);
            _okReplaceButton.SetActive(false);
        }
        else
        {
            _inspectButton.SetActive(false);
            _okReplaceButton.SetActive(true);
        }
    }
    #endregion

    public void LoadStats()
    {
        _playerBestWord.text = $"Best word: {PlayerStatsManager.Instance.GetPlayerBestWord()}";
        _opponentBestWord.text = $"Best word: {PlayerStatsManager.Instance.GetOpponentBestWord()}";
    }
}
