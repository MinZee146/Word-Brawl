using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardManager : SingletonPersistent<RewardManager>
{
    [NonSerialized] public int TotalAdDuration;

    [SerializeField] private Button _spinButton;
    [SerializeField] private GameObject _dailySpinNotifier, _noMoneyWarning;
    [SerializeField] private TextMeshProUGUI _spinText;

    public void GrantDailySpin()
    {
        var lastRewardTimestamp = PlayerPrefs.GetString(GameConstants.PLAYERPREFS_LAST_DAILY_REWARD, "");

        if (!DateTime.TryParse(lastRewardTimestamp, out DateTime lastRewardDate))
        {
            lastRewardDate = DateTime.MinValue;
        }

        var now = DateTime.Now;
        var timeSinceLastReward = now - lastRewardDate;

        if (timeSinceLastReward.TotalHours > 24 || !PlayerPrefs.HasKey(GameConstants.PLAYERPREFS_HAS_SPUN_TODAY))
        {
            PlayerPrefs.SetString(GameConstants.PLAYERPREFS_LAST_DAILY_REWARD, now.ToString());
            PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HAS_SPUN_TODAY, 0);
        }

        var hasSpunToday = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_HAS_SPUN_TODAY, 0) == 1;
        _dailySpinNotifier.SetActive(!hasSpunToday);
        _spinButton.interactable = !hasSpunToday;

        if (hasSpunToday)
        {
            _spinText.text = "Claimed";
        }
        else
        {
            _spinText.text = "Spin";
        }
    }

    public void DisableSpin()
    {
        _dailySpinNotifier.SetActive(false);
        _spinButton.interactable = false;
        _spinText.text = "Claimed";

        PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HAS_SPUN_TODAY, 1);
    }

    public void GrantHints()
    {
        if (PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_COINS) >= 100)
        {
            CurrencyManager.Instance.UpdateCoins(-100);
            PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, 5);
            HintCounter.Instance.FetchHintPref();
            UIManager.Instance.ToggleMoreHintsPanel(false);
        }
        else
        {
            Error();
        }
    }

    private void Error()
    {
        _noMoneyWarning.GetComponent<TextMeshProUGUI>()?.DOKill();
        _noMoneyWarning.GetComponent<TextMeshProUGUI>().DOFade(1f, 0f);
        _noMoneyWarning.SetActive(true);
        _noMoneyWarning.GetComponent<TextMeshProUGUI>().DOFade(0f, 1f).SetEase(Ease.InOutQuad).SetDelay(2f).OnComplete(() =>
        {
            _noMoneyWarning.SetActive(false);
        });
    }
}
