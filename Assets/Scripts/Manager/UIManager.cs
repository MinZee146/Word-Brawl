using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using MEC;
using System;

public class UIManager : SingletonPersistent<UIManager>
{
    [SerializeField] private GameObject _menuUI, _gameOverPanel, _powerUpsPanel, _settingsPanel, _phaseChangePanel, _hintRunsOutPanel, _gameOverBG, _powerUpsBG, _settingsBG, _phaseChangeBG, _hintRunsOutBG;
    [SerializeField] private GameObject _opponentPowerUpPanel, _revealWordPanel, _replaceTilePanel, _opponentPowerUpBG, _revealWordBG, _replaceTileBG, _loadingBG;
    [SerializeField] private GameObject _toggleSFXButton, _toggleMusicButton, _inspectButton, _okReplaceButton, _okRevealButton;
    [SerializeField] private Sprite _sfxOn, _sfxOff, _musicOn, _musicOff;
    [SerializeField] private TMP_InputField _replaceLetter;
    [SerializeField] private TextMeshProUGUI _revealText, _descriptionPowerUp;
    [SerializeField] private TextMeshProUGUI _playerName, _opponentName, _playerBestWord, _opponentBestWord;
    [SerializeField] private Image _imagePowerUp;
    [SerializeField] private CanvasGroup _canvasGroup;

    private AsyncOperationHandle<SceneInstance> _sceneMenuHandle;
    private AsyncOperationHandle<SceneInstance> _sceneGameplayHandle;

    private bool _isInspectingBoard, _isInteractable = true;
    private string _inspectPanel;

    public string InspectPanel => _inspectPanel;
    public string CurrentOpponent => _opponentName.text;
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
        NameRegister.Instance.Initialize();
        NameRandomizer.Instance.Initialize();
        LoadingAnimation.Instance.Initialize();
        UpdateSettingsUI();
    }

    public void LoadNames()
    {
        _playerName.text = PlayerPrefs.GetString("Username");
        _opponentName.text = NameRandomizer.Instance.GetRandomOpponent();
    }

    #region LoadScene
    public void LoadGameScene()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        LoadingAnimation.Instance.AnimationLoading(0.5f, () =>
        {
            _sceneGameplayHandle = Addressables.LoadSceneAsync("Assets/Scenes/Gameplay.unity");
            _sceneGameplayHandle.Completed += handle =>
            {
                if (_sceneMenuHandle.IsValid())
                {
                    Addressables.UnloadSceneAsync(_sceneMenuHandle, true);
                }

                LoadNames();

                GameManager.Instance.NewGame();
                PlayerStatsManager.Instance.LoadNames();
                HintCounter.Instance.FetchHintPref();
                PopUpsPool.Instance.Instantiate();

                ToggleLoadingScreen();
                LoadingAnimation.Instance.AnimationLoaded(0.5f, 0.25f);
            };
        });
    }

    public void LoadMenuScene()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        ToggleGameOverPanel();

        LoadingAnimation.Instance.AnimationLoading(0.5f, () =>
        {
            _sceneMenuHandle = Addressables.LoadSceneAsync("Assets/Scenes/Main Menu.unity");
            _sceneMenuHandle.Completed += handle =>
            {
                if (_sceneGameplayHandle.IsValid())
                {
                    Addressables.UnloadSceneAsync(_sceneGameplayHandle, true);
                }

                ToggleLoadingScreen();
                LoadingAnimation.Instance.AnimationLoaded(0.5f, 0.25f);
            };
        });
    }
    #endregion

    #region TogglePanel
    public void ToggleLoadingScreen()
    {
        _menuUI.SetActive(!_menuUI.activeSelf);
    }

    private void PanelAnimation(GameObject panel, GameObject background, Action onOpen = null, Action onClose = null)
    {
        Board.Instance.IsDragging = false;
        _canvasGroup.blocksRaycasts = false;

        panel.transform.DOKill();
        if (!background.activeSelf)
        {
            onOpen?.Invoke();
            background.SetActive(true);
            panel.SetActive(true);

            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                _canvasGroup.blocksRaycasts = true;
            });
        }
        else
        {
            panel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                onClose?.Invoke();
                panel.SetActive(false);
                background.SetActive(false);
                _canvasGroup.blocksRaycasts = true;
            });
        }
    }

    public void ToggleGameOverPanel()
    {
        PanelAnimation(_gameOverPanel, _gameOverBG);
    }

    public void TogglePowerUpsPanel()
    {
        PanelAnimation(_powerUpsPanel, _powerUpsBG);
    }

    public void ToggleSettingsPanel()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");

        PanelAnimation(_settingsPanel, _settingsBG);
    }

    public void TogglePhaseChangePanel()
    {
        PanelAnimation(_phaseChangePanel, _phaseChangeBG, onClose: () =>
        {
            LoadingAnimation.Instance.AnimationLoading(0.5f, () =>
            {
                Board.Instance.NewGame();
                GameFlowManager.Instance.NextTurn();
                LoadingAnimation.Instance.AnimationLoaded(0.5f, 0f);
            });
        });
    }

    public void ToggleHintsRunOutPanel()
    {
        PanelAnimation(_hintRunsOutPanel, _hintRunsOutBG);
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
        PanelAnimation(_opponentPowerUpPanel, _opponentPowerUpBG);
    }

    public void SetRevealedText(string text)
    {
        _revealText.text = $"The longest word available is\n<color=#FF2222>{text.ToUpper()}</color>";
    }

    public void ToggleRevealWordPopUp()
    {
        PanelAnimation(_revealWordPanel, _revealWordBG, onOpen: () =>
        {
            if (GameFlowManager.Instance.IsPlayerTurn)
            {
                _okRevealButton.SetActive(true);
            }
            else
            {
                _okRevealButton.SetActive(false);
            }
        });
    }

    public void OnReplaceLetterChanged()
    {
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
        PanelAnimation(_replaceTilePanel, _replaceTileBG, onOpen: () =>
        {
            _replaceLetter.text = "";

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
        });
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
        _inspectButton.transform.DOKill();
        _okReplaceButton.transform.DOKill();

        if (string.IsNullOrWhiteSpace(_replaceLetter.text))
        {
            _inspectButton.SetActive(true);
            _inspectButton.transform.DOScale(Vector3.one, 0.15f);
            _okReplaceButton.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => _okReplaceButton.SetActive(false));
        }
        else
        {
            _okReplaceButton.SetActive(true);
            _okReplaceButton.transform.DOScale(Vector3.one, 0.15f);
            _inspectButton.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => _inspectButton.SetActive(false));
        }
    }
    #endregion

    #region PlayerStats
    public void LoadStats()
    {
        _playerBestWord.text = $"Best word: {PlayerStatsManager.Instance.GetPlayerBestWord()}";
        _opponentBestWord.text = $"Best word: {PlayerStatsManager.Instance.GetOpponentBestWord()}";
    }

    public void SetName(string name)
    {
        _playerName.text = name;
    }

    public bool CheckCanInteractBoard()
    {
        if (_gameOverBG.activeSelf || _settingsBG.activeSelf || _powerUpsBG.activeSelf || _phaseChangeBG.activeSelf || _hintRunsOutBG.activeSelf
        || _opponentPowerUpBG.activeSelf || _revealWordBG.activeSelf || _replaceTileBG.activeSelf || _loadingBG.activeSelf
        || !_isInteractable)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region TextPopUps
    public void InstantiatePopUps(string word)
    {
        switch (word.Length)
        {
            case 3:
                PopUpsPool.Instance.SpawnFromPool("Great");
                break;
            case 4:
                PopUpsPool.Instance.SpawnFromPool("Amazing");
                break;
            case 5:
            case 6:
                PopUpsPool.Instance.SpawnFromPool("Fabulous");
                break;
            case 7:
            case 8:
                PopUpsPool.Instance.SpawnFromPool("Spectacular");
                break;
            default:
                Debug.LogWarning("No pop-up available for the given word length.");
                break;
        }
    }

    public IEnumerator<float> ShowPopUp(string SelectedWord)
    {
        if (GameFlowManager.Instance.IsPlayerTurn && SelectedWord.Length > 2)
        {
            if (SelectedWord == WordFinder.Instance.BestWord)
            {
                BestWordPopUp($"{SelectedWord} ({WordFinder.Instance.BestScore})");
            }

            InstantiatePopUps(SelectedWord);
            yield return Timing.WaitForSeconds(2f);
        }
    }

    public void BestWordPopUp(string word)
    {
        Addressables.LoadAssetAsync<GameObject>("bestword").Completed += handle =>
        {
            var popUp = Instantiate(handle.Result);
            popUp.transform.SetParent(transform, false);
            popUp.GetComponent<BestWordAnimation>().SetProps(word, handle);
            popUp.SetActive(true);

            var sequence = DOTween.Sequence();
            sequence.AppendInterval(1.8f);
            sequence.Append(popUp.transform.DOScale(Vector3.zero, 0.5f));
            sequence.Play().OnComplete(() => popUp.GetComponent<BestWordAnimation>().CleanUp());
        };
    }
    #endregion
}