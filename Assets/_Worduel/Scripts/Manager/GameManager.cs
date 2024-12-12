using UnityEngine;
using System.Collections.Generic;
using MEC;

public class GameManager : SingletonPersistent<GameManager>
{
    public bool IsGameOver { get; private set; }

    public enum GameMode
    {
        PvC,
        PvP,
    }
    public enum Location
    {
        Home,
        Gameplay,
    }

    public GameMode CurrentGameMode = GameMode.PvC;
    public Location CurrentLocation = Location.Home;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        FetchAdPref();

        GameDictionary.Instance.Initialize();
        AudioManager.Instance.Initialize();
        ThemeManager.Instance.Initialize();
        UIManager.Instance.Initialize();
        CurrencyManager.Instance.Initialize();
        PlayerDataTracker.Instance.Initialize();
        RewardManager.Instance.GrantDailySpin();
    }

    private void FetchAdPref()
    {
        if (!PlayerPrefs.HasKey(GameConstants.PLAYERPREFS_IS_FORCED_ADS_ENABLED))
        {
            PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_IS_FORCED_ADS_ENABLED, 1);
        }

        if (!PlayerPrefs.HasKey(GameConstants.PLAYERPREFS_IS_BANNER_ADS_ENABLED))
        {
            PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_IS_BANNER_ADS_ENABLED, 1);
        }
    }

    public void NewGame()
    {
        IsGameOver = false;

        AudioManager.Instance.PlaySFX("Bell");
        UIManager.Instance.SetButtonInSettingsActive(true);
        GameFlowManager.Instance.StartGame();
        PowerUpsManager.Instance.Initialize();

        PlayerStatsManager.Instance.ResetStats();
        HintCounter.Instance.Reset();
        Notifier.Instance.Reset();
    }

    public void Replay()
    {
        UIManager.Instance.ToggleGameOverPanel(false);
        RewardManager.Instance.TotalAdDuration = 0;

        LoadingAnimation.Instance.AnimationLoading(0.5f, () =>
        {
            NewGame();
            Board.Instance.NewGame();

            LoadingAnimation.Instance.AnimationLoaded(0.5f, 0);
        });
    }

    public IEnumerator<float> CheckForGameOver()
    {
        WordFinder.Instance.FindAllWords();

        while (WordFinder.Instance.IsFindingWords)
        {
            yield return Timing.WaitForOneFrame;
        }

        IsGameOver = Board.Instance.FoundWords.Keys.Count == 0;

        if (IsGameOver)
        {
            Notifier.Instance.OnRoundChanged();
            yield return Timing.WaitForSeconds(0.75f);
            GameFlowManager.Instance.HandleGameOver();
        }
    }
}
