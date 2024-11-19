using UnityEngine;
using System.Collections.Generic;
using MEC;

public class GameManager : SingletonPersistent<GameManager>
{
    private bool _isGameOver;
    public bool IsGameOver => _isGameOver;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameDictionary.Instance.Initialize();
        AudioManager.Instance.Initialize();
        UIManager.Instance.Initialize();
    }

    public void NewGame()
    {
        GameFlowManager.Instance.StartGame();
        PowerUpsManager.Instance.Initialize();
    }

    public void Replay()
    {
        AudioManager.Instance.PlaySFX("ButtonClick");
        UIManager.Instance.ToggleGameOverPanel();

        LoadingAnimation.Instance.AnimationLoading(0.5f, () =>
        {
            UIManager.Instance.LoadNames();
            PlayerStatsManager.Instance.LoadNames();
            PlayerStatsManager.Instance.Reset();

            Board.Instance.NewGame();
            NewGame();

            LoadingAnimation.Instance.AnimationLoaded(0.5f, 0);
        });
    }

    public IEnumerator<float> CheckForGameOver()
    {
        WordFinder.Instance.FindAllWords();
        _isGameOver = Board.Instance.FoundWords.Keys.Count == 0;

        if (_isGameOver)
        {
            Notifier.Instance.OnPhaseChanged();
            yield return Timing.WaitForSeconds(0.75f);
            GameFlowManager.Instance.HandleGameOver();
        }
    }
}
