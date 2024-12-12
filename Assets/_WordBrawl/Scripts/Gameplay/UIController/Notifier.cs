using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Notifier : Singleton<Notifier>
{
    public int TimesUpAtRound { get; private set; }
    public int TotalTimesUp { get; private set; }
    public float AverageTimePercentUsedAtRound { get; private set; }
    public float AverageTimePercentUsed { get; private set; }

    [SerializeField] private float _time;
    [SerializeField] private GameObject _progressBar;
    [SerializeField] private TextMeshProUGUI _notifyText;
    [SerializeField] private UIEffectTweener _tweener;
    [SerializeField] private Sprite _red, _green;

    private Tween _currentTween;
    private float _averagePlaytimeAtRound;
    private float _averagePlaytime;
    private int _turnsAtRound;
    private int _totalTurns;

    private IEnumerator<float> NotifierAnimation()
    {
        _tweener.enabled = true;
        yield return Timing.WaitForSeconds(1f);
        _tweener.enabled = false;
    }

    public void OnTurnChanged()
    {
        _notifyText.text = GameFlowManager.Instance.IsPlayerTurn ? $"<color=#2B3467>Your turn</color>" : $"<color=#EB455F>{PlayerStatsManager.Instance.OpponentName}'s turn</color>";
        Timing.RunCoroutine(NotifierAnimation());

        if (!PowerUpsManager.Instance.CheckReplaceLetter)
        {
            BeginCountdown();
        }
    }

    public void OnRoundChanged()
    {
        _notifyText.text = "No words left!";
        Timing.RunCoroutine(NotifierAnimation());
    }

    public void OnUsePowerUp(string powerUpName)
    {
        var isPlayerTurn = GameFlowManager.Instance.IsPlayerTurn;

        if (powerUpName == "ReplaceLetter" && isPlayerTurn)
        {
            _notifyText.text = "Select a letter to replace";
        }
        else
        {
            var user = isPlayerTurn ? PlayerStatsManager.Instance.PlayerName : PlayerStatsManager.Instance.OpponentName;
            var formattedPowerUpName = System.Text.RegularExpressions.Regex.Replace(powerUpName, "(?<!^)([A-Z])", " $1");

            if (isPlayerTurn)
            {
                _notifyText.text = $"<color=#2B3467>{user}</color> used <color=#78A1BB>{formattedPowerUpName}</color>";
            }
            else
            {
                _notifyText.text = $"<color=#EB455F>{user}</color> used <color=#78A1BB>{formattedPowerUpName}</color>";
            }
        }

        Timing.RunCoroutine(NotifierAnimation());
    }

    public void BeginCountdown()
    {
        StopCountdown();
        _progressBar.SetActive(true);

        var isColorChanged = false;
        var image = _progressBar.GetComponent<Image>();
        image.sprite = _green;
        image.fillAmount = 1;
        image.DOKill();
        image.DOFade(1, 0);

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            image.fillOrigin = (int)Image.OriginHorizontal.Right;
        }

        _currentTween = DOTween.To(() => image.fillAmount, x => image.fillAmount = x, 0, _time)
        .SetEase(Ease.Linear)
        .OnUpdate(() =>
        {
            if (!isColorChanged && image.fillAmount <= 0.3)
            {
                isColorChanged = true;
                image.sprite = _red;

                image.DOFade(0, 0.2f).OnComplete(() =>
                {
                    image.DOFade(1, 0.2f).SetLoops(-1, LoopType.Yoyo);
                });
            }
        })
        .OnComplete(() =>
        {
            if (GameFlowManager.Instance.IsPlayerTurn)
            {
                TotalTimesUp++;
                CaculateAverageTimePercentUsed();
            }

            Board.Instance.ResetUI();
            Board.Instance.ResetData();
            WordDisplay.Instance.UndisplayWordAndScore();

            PowerUpsManager.Instance.CleanPowerUp();
            GameFlowManager.Instance.NextTurn();
        });
    }

    public void PauseCountdown()
    {
        _currentTween?.Pause();
    }

    public void ResumeCountdown()
    {
        _currentTween?.Play();
    }

    public void StopCountdown()
    {
        _currentTween?.Kill();
        _progressBar.SetActive(false);
    }

    public void Reset()
    {
        _averagePlaytimeAtRound = _turnsAtRound = 0;

        TotalTimesUp = TimesUpAtRound = 0;
        AverageTimePercentUsedAtRound = AverageTimePercentUsed = 0;
    }

    public void SetStatsAtRound()
    {
        _averagePlaytimeAtRound = _averagePlaytime - _averagePlaytimeAtRound;
        _turnsAtRound = _totalTurns - _turnsAtRound;

        AverageTimePercentUsedAtRound = _averagePlaytimeAtRound / _turnsAtRound;
        AverageTimePercentUsed = _averagePlaytime / _totalTurns;
        TimesUpAtRound = TotalTimesUp - TimesUpAtRound;
    }

    public void CaculateAverageTimePercentUsed()
    {
        _totalTurns++;
        _averagePlaytime += 1 - _progressBar.GetComponent<Image>().fillAmount;
    }
}
