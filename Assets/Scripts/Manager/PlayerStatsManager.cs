using TMPro;
using UnityEngine;

public class PlayerStatsManager : Singleton<PlayerStatsManager>
{
    [SerializeField] private TextMeshProUGUI _playerName, _opponentName;
    [SerializeField] private TextMeshProUGUI _playerScore, _opponentScore;

    private string _playerBestWord, _opponentBestWord;
    private int _playerBestScore, _opponentBestScore, _playerCurrentScore, _opponentCurrentScore;

    public void LoadNames()
    {
        _playerName.text = PlayerPrefs.GetString("Username");
    }

    public void UpdatePlayerStats(string word, int score)
    {
        UpdatePlayerScore(score);
        if (score <= _playerBestScore) return;
        _playerBestScore = score;
        _playerBestWord = word;
    }

    public void UpdateOpponentStats(string word, int score)
    {
        UpdateOpponentScore(score);
        if (score <= _opponentBestScore) return;
        _opponentBestScore = score;
        _opponentBestWord = word;
    }

    public string GetPlayerBestWord()
    {
        return _playerBestWord + $" ({_playerBestScore})";
    }

    public string GetOpponentBestWord()
    {
        return _opponentBestWord + $" ({_opponentBestScore})";
    }

    private void UpdatePlayerScore(int score)
    {
        _playerCurrentScore += score;
        _playerScore.text = _playerCurrentScore.ToString();
    }

    private void UpdateOpponentScore(int score)
    {
        _opponentCurrentScore += score;
        _opponentScore.text = _opponentCurrentScore.ToString();
    }

    public void Reset()
    {
        _playerCurrentScore = _opponentCurrentScore = 0;
        _opponentScore.text = _playerScore.text = "0";
    }
}
