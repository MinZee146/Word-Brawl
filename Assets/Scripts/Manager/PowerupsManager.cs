using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PowerUpsManager : SingletonPersistent<PowerUpsManager>
{
    [SerializeField] private Button[] _powerUpsButtons;

    public bool CheckExtraTurn => _isExtraTurn;

    private bool _isBeingGrief, _isPenalty, _isExtraTurn;
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

        _loadedPowerUpHandle = Addressables.LoadAssetsAsync<PowerUpBase>("PowerupConfigs", null);
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
                _powerUpsButtons[index].onClick.AddListener(() => UsePowerUp(index));
                _powerUpsButtons[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = powerUps[index].Description;
                _powerUpsButtons[index].transform.GetChild(1).GetComponent<Image>().sprite = powerUps[index].Sprite;
            }
        }
        else
        {
            Debug.LogError("Failed to load power-ups from Addressables.");
        }
    }

    private void UsePowerUp(int index)
    {
        AudioManager.Instance.PlaySFX("PowerupSelect");
        _powerUpsButtons[index].interactable = false;
        _currentPowerUp = _powerUpsList[index];
        _powerUpsList[index].ApplyPowerUp();
        Notifier.Instance.OnUsePowerUp(_currentPowerUp.GetName());

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            UIManager.Instance.TogglePowerUpsPanel();
        }

        CheckForPowerUpAction();

        Debug.Log("Selected PowerUp: " + _powerUpsList[index].name);
    }

    public void SelectRandomPowerUp()
    {
        var availableButtons = _powerUpsButtons.Where(button => button.interactable).ToList();

        if (availableButtons.Count == 0)
        {
            return;
        }

        var randomIndex = Random.Range(0, availableButtons.Count);
        var selectedPowerUpIndex = System.Array.IndexOf(_powerUpsButtons, availableButtons[randomIndex]);

        UsePowerUp(selectedPowerUpIndex);
        UIManager.Instance.UpdateOpponentPowerUp(_powerUpsList[selectedPowerUpIndex].Description, _powerUpsList[selectedPowerUpIndex].Sprite);
    }

    public void CheckForPowerUpAction()
    {
        if (_currentPowerUp == null) return;

        switch (_currentPowerUp.GetName())
        {
            case "RevealWord":
                break;
            case "ReplaceLetter":
                break;
            case "Shuffle":
                break;
            case "ExtraTurn":
                _isExtraTurn = true;
                break;
        }
    }

    public void CheckForPowerUpScoring(ref int currentScore, int currentLength)
    {
        if (_isBeingGrief)
        {
            currentScore /= 2;
        }

        if (_isPenalty)
        {
            if (currentLength < 5)
            {
                currentScore /= 2;
            }
        }

        if (_currentPowerUp == null) return;

        switch (_currentPowerUp.GetName())
        {
            case "DoubleScore":
                currentScore *= 2;
                break;
            case "Grief":
                _isBeingGrief = true;
                break;
            case "LongBonus":
                currentScore = currentLength >= 5 ? currentScore * 2 : currentScore;
                break;
            case "ShortBonus":
                currentScore = currentLength < 5 ? currentScore * 2 : currentScore;
                break;
            case "ShortPenalty":
                _isPenalty = true;
                break;
        }
    }

    public void CleanPowerUp()
    {
        if (_currentPowerUp == null)
        {
            _isExtraTurn = false;
            _isBeingGrief = false;
            _isPenalty = false;
            return;
        }

        if (_currentPowerUp.GetName() != "Grief")
        {
            _isBeingGrief = false;
        }

        if (_currentPowerUp.GetName() != "ShortPenalty")
        {
            _isPenalty = false;
        }

        if (_currentPowerUp.GetName() != "ExtraTurn")
        {
            _isExtraTurn = false;
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
                button.onClick.RemoveAllListeners();
                button.interactable = false;
                button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Empty;
                button.transform.GetChild(1).GetComponent<Image>().sprite = null;
            }
        }
    }
}
