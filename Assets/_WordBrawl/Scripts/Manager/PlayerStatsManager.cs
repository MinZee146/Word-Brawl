using System;
using System.Collections.Generic;
using MEC;
using TMPro;
using UnityEngine;

public class PlayerStatsManager : Singleton<PlayerStatsManager>
{
    [SerializeField] private TextMeshProUGUI _playerName, _opponentName;
    [SerializeField] private TextMeshProUGUI _playerScore, _opponentScore;

    private string _playerBestWord, _opponentBestWord;
    private int _playerBestScore, _opponentBestScore, _playerCurrentScore, _opponentCurrentScore;

    public string PlayerName => _playerName.text;
    public string OpponentName => _opponentName.text;
    public int PlayerScore => _playerCurrentScore;
    public int OpponentScore => _opponentCurrentScore;
    public int PlayerScoreAtRound { get; private set; }
    public int OpponentScoreAtRound { get; private set; }

    public void ResetStats()
    {
        _playerName.text = "You";
        _opponentName.text = "Computer";
        _playerScore.text = _opponentScore.text = "0";

        PlayerScoreAtRound = OpponentScoreAtRound = 0;
        _playerCurrentScore = _opponentCurrentScore = 0;
        _playerBestScore = _opponentBestScore = 0;
        _playerBestWord = _opponentBestWord = "";
    }

    public void UpdateStats(string playerWord, string opponentWord, int score)
    {
        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            UpdatePlayerScore(score);
            if (score <= _playerBestScore) return;
            _playerBestScore = score;
            _playerBestWord = playerWord;
        }
        else
        {
            UpdateOpponentScore(score);
            if (score <= _opponentBestScore) return;
            _opponentBestScore = score;
            _opponentBestWord = opponentWord;
        }
    }

    public string GetPlayerBestWord()
    {
        return _playerBestWord + $" ({_playerBestScore})";
    }

    public string GetOpponentBestWord()
    {
        return _opponentBestWord + $" ({_opponentBestScore})";
    }

    public bool IsPlayerWon()
    {
        if (_playerCurrentScore == _opponentCurrentScore)
            return IsPlayerHavingBestWord();
        else
            return _playerCurrentScore > _opponentCurrentScore;
    }

    public bool IsPlayerHavingBestWord()
    {
        return _playerBestScore > _opponentBestScore;
    }

    public int ScoreGap()
    {
        return Math.Abs(_playerCurrentScore - _opponentCurrentScore);
    }

    private void UpdatePlayerScore(int increment)
    {
        Timing.RunCoroutine(AnimateScoreIncrease(_playerScore, _playerCurrentScore, increment, value => _playerCurrentScore = value));
    }

    private void UpdateOpponentScore(int increment)
    {
        Timing.RunCoroutine(AnimateScoreIncrease(_opponentScore, _opponentCurrentScore, increment, value => _opponentCurrentScore = value));
    }

    private IEnumerator<float> AnimateScoreIncrease(TextMeshProUGUI scoreText, int currentScore, int increment, System.Action<int> setScore)
    {
        int startScore = currentScore;
        int targetScore = currentScore + increment;

        float durationPerPoint = 0.015f;
        float duration = Mathf.Max(0.2f, increment * durationPerPoint);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Timing.DeltaTime;
            float progress = elapsedTime / duration;
            int newScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, progress));
            setScore(newScore);
            scoreText.text = newScore.ToString();

            yield return Timing.WaitForOneFrame;
        }

        setScore(targetScore);
        scoreText.text = targetScore.ToString();
    }

    public void LogStats()
    {
        PlayerDataTracker.Instance.LogBattleResult(IsPlayerWon(), _playerBestWord, _playerBestScore);
    }

    public bool HasWonRound()
    {
        PlayerScoreAtRound = PlayerScore - PlayerScoreAtRound;
        OpponentScoreAtRound = OpponentScore - OpponentScoreAtRound;

        return PlayerScoreAtRound > OpponentScoreAtRound;
    }
}
