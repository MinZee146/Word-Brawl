using TMPro;
using UnityEngine;

public class Notifier : Singleton<Notifier>
{
    [SerializeField] private TextMeshProUGUI _notifyText, _playerName, _opponentName;

    public void OnTurnChanged()
    {
        var isPlayerTurn = GameFlowManager.Instance.IsPlayerTurn;
        _notifyText.text = isPlayerTurn ? $"{_playerName.text}'s turn" : $"{_opponentName.text}'s turn";
    }

    public void OnPhaseChanged()
    {
        _notifyText.text = "No words left!";
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
            var user = isPlayerTurn ? _playerName.text : _opponentName.text;

            var formattedPowerUpName = System.Text.RegularExpressions.Regex.Replace(powerUpName, "(?<!^)([A-Z])", " $1");
            _notifyText.text = $"{user} used <color=#003366>{formattedPowerUpName}</color>";

        }
    }
}
