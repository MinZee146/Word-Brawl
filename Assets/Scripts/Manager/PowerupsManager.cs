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

    private PowerUpBase[] _powerUpsList = new PowerUpBase[6];
    private AsyncOperationHandle<IList<PowerUpBase>> _loadedPowerUpHandle;

    public int PowerUpCounts()
    {
        return _powerUpsButtons.Count(button => button.interactable);
    }

    public void Initialize()
    {
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
                _powerUpsButtons[index].onClick.AddListener(() => ApplyPowerUp(index));
                _powerUpsButtons[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = powerUps[index].Description;
                _powerUpsButtons[index].transform.GetChild(1).GetComponent<Image>().sprite = powerUps[index].Sprite;
            }
        }
        else
        {
            Debug.LogError("Failed to load power-ups from Addressables.");
        }
    }

    private void ApplyPowerUp(int index)
    {
        AudioManager.Instance.PlaySFX("PowerupSelect");
        _powerUpsButtons[index].interactable = false;

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            UIManager.Instance.TogglePowerUpsPanel();
        }
        // TODO: Add Powerup logic here
    }

    public void UnloadPowerUps()
    {
        // Release all loaded power-ups with a single call
        Addressables.Release(_loadedPowerUpHandle);

        // Clear button states and listeners
        foreach (var button in _powerUpsButtons)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = false;
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = string.Empty;
            button.transform.GetChild(1).GetComponent<Image>().sprite = null;
        }
    }
}
