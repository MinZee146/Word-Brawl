using TMPro;
using UnityEngine;

public class PlayerStatsManager : Singleton<PlayerStatsManager>
{
    [SerializeField] private TextMeshProUGUI _playerName, _opponentName;

    public void LoadNames()
    {
        _playerName.text = PlayerPrefs.GetString("Username");
    }
}
