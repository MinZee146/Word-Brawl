using System.ComponentModel;
using MEC;
using UnityEngine;

public partial class SROptions
{
    [Category("Debug")]
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [Category("Debug")]
    public void AIPlay()
    {
        Timing.RunCoroutine(AI.Instance.AITurn());
    }

    [Category("Debug")]
    public void GameOver()
    {
        UIManager.Instance.ToggleGameOverPanel();
    }

    [Category("Debug")]
    public void NewPhase()
    {
        GameFlowManager.Instance.NextPhase();
    }
}