using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;

public class CurrencyManager : SingletonPersistent<CurrencyManager>
{
    [SerializeField] private TextMeshProUGUI _coinsText;

    private bool _isPurchasing;
    private int _currentAmount;

    public void Initialize()
    {
        FetchPrefs();
    }

    public void FetchPrefs()
    {
        _currentAmount = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_COINS, RemoteConfig.Instance.InitialCoins);
        PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_COINS, _currentAmount);

        UpdateText();
    }

    private void UpdateText()
    {
        var formattedNumber = string.Format("{0:N0}", _currentAmount);
        _coinsText.text = formattedNumber.ToString();
    }

    public void UpdateCoins(int amount)
    {
        if (_isPurchasing) return;

        Timing.RunCoroutine(Animate(amount));
        UpdateText();
    }

    private IEnumerator<float> Animate(int amount)
    {
        _isPurchasing = true;

        var startCoins = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_COINS);
        var targetCoins = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_COINS) + amount;
        PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_COINS, targetCoins);

        var duration = 0.5f;
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Timing.DeltaTime;
            var progress = elapsedTime / duration;
            var newCoins = Mathf.RoundToInt(Mathf.Lerp(startCoins, targetCoins, progress));

            _currentAmount = newCoins;
            UpdateText();

            yield return Timing.WaitForOneFrame;
        }

        _currentAmount = targetCoins;
        UpdateText();

        _isPurchasing = false;
    }
}
