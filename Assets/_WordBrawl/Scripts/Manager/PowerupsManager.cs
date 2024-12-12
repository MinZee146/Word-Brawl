using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using MEC;

public class PowerUpsManager : SingletonPersistent<PowerUpsManager>
{
    [SerializeField] private Button[] _powerUpsButtons;

    public bool CheckExtraTurn => _isExtraTurn;
    public bool CheckReplaceLetter => _isReplaceLetter;
    public bool CheckShuffle => _isShuffle;
    public bool CheckRevealWord => _isRevealWord;

    private bool _isBeingGrief, _isPenalty, _isCleansing, _isExtraTurn, _isReplaceLetter, _isShuffle, _isRevealWord;
    private PowerUpBase _currentPowerUp;
    private PowerUpBase[] _powerUpsList = new PowerUpBase[6];
    private AsyncOperationHandle<IList<PowerUpBase>> _loadedPowerUpHandle;

    public int PowerUpCounts()
    {
        return _powerUpsButtons.Count(button => button.interactable);
    }

    public void Initialize()
    {
        UnloadPowerUps();
        CleanPowerUp();

        _loadedPowerUpHandle = Addressables.LoadAssetsAsync<PowerUpBase>("PowerUp", null);
        _loadedPowerUpHandle.Completed += OnPowerUpsLoaded;
    }

    private void OnPowerUpsLoaded(AsyncOperationHandle<IList<PowerUpBase>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var powerUps = handle.Result.OrderBy(x => Random.value).ToList();
            for (var i = 0; i < 6; i++)
            {
                int index = i;
                _powerUpsList[index] = powerUps[index];

                _powerUpsButtons[index].interactable = true;
                _powerUpsButtons[index].onClick.AddListener(() => Timing.RunCoroutine(UsePowerUp(index)));
                _powerUpsButtons[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = powerUps[index].Description;
                _powerUpsButtons[index].transform.GetChild(1).GetComponent<Image>().sprite = powerUps[index].Sprite;
            }
        }
        else
        {
            Debug.LogError("Failed to load power-ups from Addressables.");
        }
    }

    private IEnumerator<float> UsePowerUp(int index)
    {
        AudioManager.Instance.PlaySFX("PowerupSelect");
        _powerUpsButtons[index].interactable = false;

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            PowerUpCountdown.Instance.Reset();
            UIManager.Instance.TogglePowerUpsPanel(false);

            UIManager.Instance.IsInteractable = false;
            yield return Timing.WaitForSeconds(0.5f);
            UIManager.Instance.IsInteractable = true;
        }

        _currentPowerUp = _powerUpsList[index];
        _powerUpsList[index].ApplyPowerUp();

        Notifier.Instance.OnUsePowerUp(_currentPowerUp.GetName());

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            PlayerDataTracker.Instance.LogPowerUpUsage(_currentPowerUp.GetName());
        }

        CheckForPowerUpAction();

        Debug.Log("Selected PowerUp: " + _powerUpsList[index].name);
    }

    public void PlayerUseRandomPowerUp()
    {
        var availableButtons = _powerUpsButtons.Where(button => button.interactable).ToList();
        var randomButton = availableButtons[Random.Range(0, availableButtons.Count)];
        var selectedPowerUpIndex = System.Array.IndexOf(_powerUpsButtons, randomButton);

        Timing.RunCoroutine(UsePowerUp(selectedPowerUpIndex));
    }

    public void AIUseRandomPowerUp()
    {
        var availableButtons = _powerUpsButtons.Where(button => button.interactable).ToList();

        if (availableButtons.Count == 0)
        {
            return;
        }

        if (_isBeingGrief || _isPenalty)
        {
            for (var i = 0; i < _powerUpsList.Length; i++)
            {
                if (_powerUpsList[i].GetName() == "Cleanse" && _powerUpsButtons[i].interactable)
                {
                    Timing.RunCoroutine(UsePowerUp(i));
                    UIManager.Instance.UpdateOpponentPowerUp(_powerUpsList[i].Description, _powerUpsList[i].Sprite);
                    return;
                }
            }
        }

        var randomIndex = Random.Range(0, availableButtons.Count);
        var selectedPowerUpIndex = System.Array.IndexOf(_powerUpsButtons, availableButtons[randomIndex]);

        Timing.RunCoroutine(UsePowerUp(selectedPowerUpIndex));
        UIManager.Instance.UpdateOpponentPowerUp(_powerUpsList[selectedPowerUpIndex].Description, _powerUpsList[selectedPowerUpIndex].Sprite);
    }

    public void CheckForPowerUpAction()
    {
        if (_currentPowerUp == null) return;

        switch (_currentPowerUp.GetName())
        {
            case "RevealWord":
                _isRevealWord = true;
                break;
            case "Shuffle":
                _isShuffle = true;
                break;
            case "ReplaceLetter":
                _isReplaceLetter = true;
                break;
            case "ExtraTurn":
                _isExtraTurn = true;
                break;
        }
    }

    public void CheckForPowerUpScoring(ref int currentScore, int currentLength)
    {
        if (_isBeingGrief && _currentPowerUp == null)
        {
            currentScore /= 2;
        }

        if (_isPenalty && _currentPowerUp == null && currentLength < 5)
        {
            currentScore /= 2;
        }

        if (_currentPowerUp == null) return;

        switch (_currentPowerUp.GetName())
        {
            case "DoubleScore":
                currentScore *= 2;
                break;
            case "LongBonus":
                currentScore = currentLength >= 5 ? currentScore * 2 : currentScore;
                break;
            case "ShortBonus":
                currentScore = currentLength < 5 ? currentScore * 2 : currentScore;
                break;
            case "Grief":
                _isBeingGrief = true;
                break;
            case "ShortPenalty":
                _isPenalty = true;
                break;
            case "Cleanse":
                _isCleansing = true;
                break;
        }

        if (_isCleansing)
        {
            if (!_isBeingGrief && !_isPenalty)
            {
                currentScore = (int)(currentScore * 1.5f);
                return;
            }
        }
        else
        {
            if (_isBeingGrief && _currentPowerUp.GetName() != "Grief")
            {
                currentScore /= 2;
                return;
            }

            if (_isPenalty && _currentPowerUp.GetName() != "ShortPenalty" && currentLength < 5)
            {
                currentScore /= 2;
            }
        }
    }

    public void CleanPowerUp()
    {
        _isReplaceLetter = _isShuffle = _isRevealWord = _isCleansing = false;

        if (_currentPowerUp == null)
        {
            _isExtraTurn = _isBeingGrief = _isPenalty = false;
            return;
        }

        if (_currentPowerUp.GetName() != "ExtraTurn")
        {
            _isExtraTurn = false;
        }

        if (_currentPowerUp.GetName() != "Grief")
        {
            _isBeingGrief = false;
        }

        if (_currentPowerUp.GetName() != "ShortPenalty")
        {
            _isPenalty = false;
        }

        _currentPowerUp = null;
        AI.Instance.PreferLong = AI.Instance.PreferShort = false;
    }

    public void UnloadPowerUps()
    {
        if (_loadedPowerUpHandle.IsValid())
        {
            Addressables.Release(_loadedPowerUpHandle);

            foreach (var button in _powerUpsButtons)
            {
                button.interactable = false;
                button.onClick.RemoveAllListeners();
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Empty;
                button.transform.GetChild(1).GetComponent<Image>().sprite = null;
            }
        }
    }
}
