using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PowerupsManager : SingletonPersistent<PowerupsManager>
{
    [SerializeField] private Button[] _powerUpsButtons;

    private PowerupBase[] _powerUpsList = new PowerupBase[6];
    private AsyncOperationHandle<IList<PowerupBase>> _loadedPowerupHandle;

    public int PowerUpCounts()
    {
        return _powerUpsButtons.Count(button => button.interactable);
    }

    public void InitializePowerUps()
    {
        _loadedPowerupHandle = Addressables.LoadAssetsAsync<PowerupBase>("PowerupConfigs", null);
        _loadedPowerupHandle.Completed += OnPowerUpsLoaded;
    }

    private void OnPowerUpsLoaded(AsyncOperationHandle<IList<PowerupBase>> handle)
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

        // TODO: Add Powerup logic here
    }

    public void UnloadPowerUps()
    {
        // Release all loaded power-ups with a single call
        Addressables.Release(_loadedPowerupHandle);

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
