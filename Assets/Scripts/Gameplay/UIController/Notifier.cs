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
            _notifyText.text = $"{user} used <color=#4C1F7A>{powerUpName}</color>";
        }
    }
}
