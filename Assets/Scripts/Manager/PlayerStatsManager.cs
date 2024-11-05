using TMPro;
using UnityEngine;

public class PlayerStatsManager : Singleton<PlayerStatsManager>
{
    [SerializeField] private TextMeshProUGUI _playerName, _opponentName;

    private string _playerBestWord, _opponentBestWord;
    private int _playerScore, _opponentScore;

    public void LoadNames()
    {
        _playerName.text = PlayerPrefs.GetString("Username");
    }

    public void UpdatePlayerStats(string word, int score)
    {
        if (score <= _playerScore) return;
        _playerScore = score;
        _playerBestWord = word;
    }

    public void UpdateOpponentStats(string word, int score)
    {
        if (score <= _opponentScore) return;
        _opponentScore = score;
        _opponentBestWord = word;
    }

    public string GetPlayerBestWord()
    {
        return _playerBestWord + $" ({_playerScore})";
    }

    public string GetOpponentBestWord()
    {
        return _opponentBestWord + $" ({_opponentScore})";
    }

}
