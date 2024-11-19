using MEC;
using UnityEngine;

public class GameFlowManager : SingletonPersistent<GameFlowManager>
{
    private bool _isPlayerTurn;
    private int _turn, _phase;

    public bool IsPlayerTurn => _isPlayerTurn;
    public int Turn => _turn;
    public int Phase => _phase;

    public void StartGame()
    {
        _phase = 1;
        _turn = 1;
        _isPlayerTurn = true;
        AudioManager.Instance.PlaySFX("Bell");
    }

    public void NextPhase()
    {
        _phase++;
        _turn = 0;
        _isPlayerTurn = _phase != 1;

        PowerUpsManager.Instance.Initialize();
        UIManager.Instance.TogglePhaseChangePanel();
        AudioManager.Instance.PlaySFX("Bell");
    }

    public void HandleGameOver()
    {
        if (Phase == 1)
        {
            NextPhase();
        }
        else
        {
            UIManager.Instance.LoadStats();
            UIManager.Instance.ToggleGameOverPanel();
            AudioManager.Instance.PlaySFX("Bell");
        }
    }

    public void NextTurn()
    {
        _turn++;
        _isPlayerTurn = !_isPlayerTurn;

        GameUIController.Instance.ToggleHintAndConfirm();
        Notifier.Instance.OnTurnChanged();

        if (_isPlayerTurn)
        {
            if (_turn > 2 && PowerUpsManager.Instance.PowerUpCounts() > 0)
            {
                UIManager.Instance.TogglePowerUpsPanel();
            }
        }
        else
        {
            Timing.RunCoroutine(AI.Instance.AITurn());
        }
    }
}
