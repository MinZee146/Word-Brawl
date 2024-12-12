using System.Collections.Generic;
using MEC;

public class GameFlowManager : SingletonPersistent<GameFlowManager>
{
    public bool IsPlayerTurn { get; private set; }
    public int Turn { get; private set; }
    public int Round { get; private set; }

    public void StartGame()
    {
        Round = 1;
        Turn = 1;
        IsPlayerTurn = true;

        GameUIController.Instance.UpdateRoundIndicator();
    }

    public void NextRound()
    {
        Round++;
        Turn = 0;
        IsPlayerTurn = Round != 1;

        PowerUpsManager.Instance.Initialize();
        UIManager.Instance.ToggleRoundChangePanel(true);
        GameUIController.Instance.UpdateRoundIndicator();
    }

    public void HandleGameOver()
    {
        HintCounter.Instance.SetStatsAtRound();
        Notifier.Instance.SetStatsAtRound();

        if (Round == 1)
        {
            NextRound();
        }
        else
        {
            UIManager.Instance.ToggleGameOverPanel(true);
            PlayerStatsManager.Instance.LogStats();
            AudioManager.Instance.PlaySFX("Bell");
        }
    }

    public void NextTurn()
    {
        Turn++;
        IsPlayerTurn = !IsPlayerTurn;

        GameUIController.Instance.ToggleHintAndConfirm();
        UIManager.Instance.SetButtonInSettingsActive(IsPlayerTurn);
        UIManager.Instance.DisableAllPanel();
        Notifier.Instance.OnTurnChanged();

        if (IsPlayerTurn)
        {
            if (Turn > 2 && PowerUpsManager.Instance.PowerUpCounts() > 0)
            {
                UIManager.Instance.TogglePowerUpsPanel(true);
            }
        }
        else
        {
            Timing.RunCoroutine(AI.Instance.AITurn());
        }
    }
}
