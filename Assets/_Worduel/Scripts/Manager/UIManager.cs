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
using UnityEngine.SceneManagement;

public class UIManager : SingletonPersistent<UIManager>
{
    [SerializeField] private GameObject _gameOverPanel, _gameOverBG, _powerUpsPanel, _powerUpsBG, _roundChangePanel, _roundChangeBG, _moreHintsPanel, _moreHintsBG, _purchaseFailedPanel, _purchaseFailedBG, _purchaseCompletedMenu, _purchaseCompletedBG;
    [SerializeField] private GameObject _opponentPowerUpPanel, _opponentPowerUpBG, _revealWordPanel, _revealWordBG, _replaceTilePanel, _replaceTileBG, _instructionPanel, _instructionBG, _doubleRewardBG, _doubleRewardMenu;
    [SerializeField] private GameObject _homeScreen, _loadingBG, _navigationBar, _currency, _themeSelectPanel, _themeSelectBG, _settingsPanel, _settingsBG, _statsPanel, _statsBG, _dailyPanel, _dailyBG;
    [SerializeField] private GameObject _toggleSFXButton, _toggleMusicButton, _inspectButton, _okReplaceButton, _okRevealButton, _homeSettingsButton, _homeGameOverButton, _replayButton;
    [SerializeField] private Sprite _soundOn, _soundOff;
    [SerializeField] private TMP_InputField _replaceLetter;
    [SerializeField] private TextMeshProUGUI _playerName, _opponentName, _playerBestWord, _opponentBestWord;
    [SerializeField] private TextMeshProUGUI _revealText, _descriptionPowerUp;
    [SerializeField] private Image _imagePowerUp;

    private AsyncOperationHandle<SceneInstance> _sceneMenuHandle;
    private AsyncOperationHandle<SceneInstance> _sceneGameplayHandle;
    private bool _isInspectingBoard, _isInteractable = true;

    public CanvasGroup UICanvasGroup;
    public string InspectPanel { get; private set; }
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
        LoadingAnimation.Instance.Initialize();
        UpdateSettingsUI();
    }

    public bool CheckCanInteractBoard()
    {
        if (_gameOverBG.activeSelf || _settingsBG.activeSelf || _powerUpsBG.activeSelf || _roundChangeBG.activeSelf || _moreHintsBG.activeSelf
        || _opponentPowerUpBG.activeSelf || _revealWordBG.activeSelf || _replaceTileBG.activeSelf || _instructionBG.activeSelf || _loadingBG.activeSelf
        || !_isInteractable)
        {
            return false;
        }

        return true;
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

                GameManager.Instance.NewGame();
                HintCounter.Instance.FetchHintPref();
                PopUpsPool.Instance.Instantiate();

                ToggleHomeScreen(false);
                ToggleNavigationBar(false);
                ToggleCurrency(false);

                LoadingAnimation.Instance.AnimationLoaded(0.5f, 0.25f);
                GameManager.Instance.CurrentLocation = GameManager.Location.Gameplay;
            };
        });
    }

    public void LoadMenuScene()
    {
        DisableAllPanel();

        if (!GameManager.Instance.IsGameOver)
        {
            PlayerStatsManager.Instance.HasWonRound();
            HintCounter.Instance.SetStatsAtRound();
            Notifier.Instance.SetStatsAtRound();
        }

        LoadingAnimation.Instance.AnimationLoading(0.5f, () =>
        {
            _sceneMenuHandle = Addressables.LoadSceneAsync("Assets/Scenes/Main Menu.unity");
            _sceneMenuHandle.Completed += handle =>
            {
                if (_sceneGameplayHandle.IsValid())
                {
                    Addressables.UnloadSceneAsync(_sceneGameplayHandle, true);
                }

                SetButtonInSettingsActive(false);
                ToggleHomeScreen(true);
                ToggleNavigationBar(true);
                ToggleCurrency(true);

                LoadingAnimation.Instance.AnimationLoaded(0.5f, 0.25f);
                GameManager.Instance.CurrentLocation = GameManager.Location.Home;
            };
        });
    }
    #endregion

    #region TogglePanel
    public void PanelAnimation(GameObject panel, GameObject background, bool setActive, Action onOpen = null, Action onClose = null)
    {
        if (SceneManager.GetActiveScene().name == "Gameplay")
        {
            Board.Instance.IsDragging = false;
            GameUIController.Instance.GameplayCanvasGroup.blocksRaycasts = false;
        }

        UICanvasGroup.blocksRaycasts = false;

        panel.transform.DOKill();
        if (setActive)
        {
            onOpen?.Invoke();
            background.SetActive(true);
            panel.SetActive(true);

            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                UICanvasGroup.blocksRaycasts = true;
            });
        }
        else
        {
            panel.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
            {
                onClose?.Invoke();
                panel.SetActive(false);
                background.SetActive(false);

                if (SceneManager.GetActiveScene().name == "Gameplay")
                {
                    GameUIController.Instance.GameplayCanvasGroup.blocksRaycasts = true;
                }

                UICanvasGroup.blocksRaycasts = true;
            });
        }
    }

    public void DisableAllPanel()
    {
        _isInspectingBoard = false;

        if (_gameOverBG.activeSelf)
        {
            ToggleGameOverPanel(false);
        }

        if (_powerUpsBG.activeSelf)
        {
            TogglePowerUpsPanel(false);
        }

        if (_settingsBG.activeSelf)
        {
            ToggleSettingsPanel(false);
        }

        if (_roundChangeBG.activeSelf)
        {
            ToggleRoundChangePanel(false);
        }

        if (_moreHintsBG.activeSelf)
        {
            ToggleMoreHintsPanel(false);
        }

        if (_opponentPowerUpBG.activeSelf)
        {
            ToggleOpponentPowerUpPanel(false);
        }

        if (_revealWordBG.activeSelf)
        {
            ToggleRevealWordPopUp(false);
        }

        if (_replaceTileBG.activeSelf)
        {
            ToggleTileReplacePopUp(false);
        }

        if (_instructionBG.activeSelf)
        {
            ToggleInstructionPanel(false);
        }
    }

    public void ToggleDailyRewardPanel(bool setActive)
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        PanelAnimation(_dailyPanel, _dailyBG, setActive);
    }

    public void ToggleStatsPanel(bool setActive)
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        PanelAnimation(_statsPanel, _statsBG, setActive, onOpen: () =>
        {
            LoadStats.Instance.Load();
        });
    }

    public void ToggleInstructionPanel(bool setActive)
    {
        PanelAnimation(_instructionPanel, _instructionBG, setActive);
    }

    public void ToggleDoubleRewardPanel(bool setActive)
    {
        PanelAnimation(_doubleRewardMenu, _doubleRewardBG, setActive);
    }

    public void TogglePurchaseCompletedPanel(bool setActive)
    {
        PanelAnimation(_purchaseCompletedMenu, _purchaseCompletedBG, setActive);
    }

    public void TogglePurchasedFailedPanel(bool setActive)
    {
        PanelAnimation(_purchaseFailedPanel, _purchaseFailedBG, setActive);
    }

    public void ToggleSettingsPanel(bool setActive)
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        PanelAnimation(_settingsPanel, _settingsBG, setActive);
    }

    public void ToggleThemePanel(bool setActive)
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        PanelAnimation(_themeSelectPanel, _themeSelectBG, setActive);
    }

    public void ToggleGameOverPanel(bool setActive)
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        ToggleCurrency(setActive);
        PanelAnimation(_gameOverPanel, _gameOverBG, setActive,
        onOpen: () =>
        {
            _playerName.text = PlayerStatsManager.Instance.PlayerName;
            _opponentName.text = PlayerStatsManager.Instance.OpponentName;
            _playerBestWord.text = $"Best word: {PlayerStatsManager.Instance.GetPlayerBestWord()}";
            _opponentBestWord.text = $"Best word: {PlayerStatsManager.Instance.GetOpponentBestWord()}";

        });
    }

    public void TogglePowerUpsPanel(bool setActive)
    {
        PanelAnimation(_powerUpsPanel, _powerUpsBG, setActive,
        onOpen: () => Notifier.Instance.PauseCountdown(),
        onClose: () => Notifier.Instance.ResumeCountdown());
    }

    public void ToggleRoundChangePanel(bool setActive)
    {
        PanelAnimation(_roundChangePanel, _roundChangeBG, setActive, onClose: () =>
        {
            LoadingAnimation.Instance.AnimationLoading(0.5f, () =>
            {
                AudioManager.Instance.PlaySFX("Bell");
                Board.Instance.NewGame();
                GameFlowManager.Instance.NextTurn();
                LoadingAnimation.Instance.AnimationLoaded(0.5f, 0f);
            });
        });
    }

    public void ToggleMoreHintsPanel(bool setActive)
    {
        PanelAnimation(_moreHintsPanel, _moreHintsBG, setActive,
        onOpen: () =>
        {
            Notifier.Instance.PauseCountdown();
            ToggleCurrency(true);
        },
        onClose: () =>
        {
            Notifier.Instance.ResumeCountdown();
            ToggleCurrency(false);
        });
    }

    public void ToggleOpponentPowerUpPanel(bool setActive)
    {
        PanelAnimation(_opponentPowerUpPanel, _opponentPowerUpBG, setActive);
    }

    public void ToggleRevealWordPopUp(bool setActive)
    {
        PanelAnimation(_revealWordPanel, _revealWordBG, setActive,
        onOpen: () =>
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

    public void ToggleTileReplacePopUp(bool setActive)
    {
        PanelAnimation(_replaceTilePanel, _replaceTileBG, setActive,
        onOpen: () =>
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
    #endregion

    #region ToggleUI
    public void SetButtonInSettingsActive(bool setInteractable)
    {
        _homeSettingsButton.GetComponent<Button>().interactable = setInteractable;
    }

    public void SetButtonInGameOverActive(bool setActive)
    {
        var homeImage = _homeGameOverButton.GetComponent<Image>();
        var replayImage = _replayButton.GetComponent<Image>();

        _homeGameOverButton.SetActive(setActive);
        _replayButton.SetActive(setActive);

        if (setActive)
        {
            homeImage.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
            replayImage.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
        }
        else
        {
            homeImage.DOFade(0, 0f).SetEase(Ease.OutQuad);
            replayImage.DOFade(0, 0f).SetEase(Ease.OutQuad);
        }
    }

    public void ToggleNavigationBar(bool setActive)
    {
        _navigationBar.SetActive(setActive);
    }

    public void ToggleHomeScreen(bool setActive)
    {
        _homeScreen.SetActive(setActive);
    }

    public void ToggleCurrency(bool setActive)
    {
        _currency.SetActive(setActive);
    }

    public void ToggleInspectPowerUps()
    {
        _isInspectingBoard = !_isInspectingBoard;
        InspectPanel = "PowerUps";

        TogglePowerUpsPanel(!_isInspectingBoard);
    }

    public void ToggleInspectReplace()
    {
        _isInspectingBoard = !_isInspectingBoard;
        InspectPanel = "Replace";

        ToggleTileReplacePopUp(!IsInspectingBoard);
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

    #region SettingsUI
    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
        var isSfxOn = _toggleSFXButton.GetComponent<Image>().sprite == _soundOn;
        _toggleSFXButton.GetComponent<Image>().sprite = isSfxOn ? _soundOff : _soundOn;

        PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_IS_SFX_ON, isSfxOn ? 0 : 1);
        PlayerPrefs.Save();
    }

    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
        var isMusicOn = _toggleMusicButton.GetComponent<Image>().sprite == _soundOn;
        _toggleMusicButton.GetComponent<Image>().sprite = isMusicOn ? _soundOff : _soundOn;

        PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_IS_MUSIC_ON, isMusicOn ? 0 : 1);
        PlayerPrefs.Save();
    }

    private void UpdateSettingsUI()
    {
        var sfxState = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_IS_SFX_ON, 1);
        _toggleSFXButton.GetComponent<Image>().sprite = sfxState == 1 ? _soundOn : _soundOff;

        var musicState = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_IS_MUSIC_ON, 1);
        _toggleMusicButton.GetComponent<Image>().sprite = musicState == 1 ? _soundOn : _soundOff;
    }
    #endregion

    #region PowerUpUI
    public void UpdateOpponentPowerUp(string description, Sprite sprite)
    {
        _descriptionPowerUp.text = description;
        _imagePowerUp.sprite = sprite;
    }

    public void SetRevealedText(string text)
    {
        _revealText.text = $"The longest word available is\n<color=#FF2222>{text.ToUpper()}</color>";
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

    public void SetReplaceText(char letter)
    {
        _replaceLetter.text = letter.ToString();
    }

    public void ConfirmReplace()
    {
        Board.Instance.ReplaceSelectingTileWith(_replaceLetter.text[0]);
        Board.Instance.ClearHandleTileReplaceListeners();
        Notifier.Instance.OnTurnChanged();
        ToggleTileReplacePopUp(false);
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

    public IEnumerator<float> ShowPopUp(string selectedWord, int score)
    {
        if (GameFlowManager.Instance.IsPlayerTurn && selectedWord.Length > 2)
        {
            if (selectedWord == WordFinder.Instance.BestWord)
            {
                BestWordPopUp($"{selectedWord} ({score})");
            }

            InstantiatePopUps(selectedWord);
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
