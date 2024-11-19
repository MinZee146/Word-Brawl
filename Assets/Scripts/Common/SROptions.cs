using System.ComponentModel;
using MEC;
using UnityEngine;

public partial class SROptions
{
    [Category("Debug")]
    public void GameOver()
    {
        UIManager.Instance.LoadStats();
        UIManager.Instance.ToggleGameOverPanel();
    }

    [Category("Debug")]
    public void NewPhase()
    {
        GameFlowManager.Instance.NextPhase();
    }

    [Category("Debug")]
    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [Category("PopUps")]
    public void Great()
    {
        UIManager.Instance.InstantiatePopUps("hhh");
    }

    [Category("PopUps")]
    public void Amazing()
    {
        UIManager.Instance.InstantiatePopUps("hhhh");
    }

    [Category("PopUps")]
    public void Fabulous()
    {
        UIManager.Instance.InstantiatePopUps("hhhhh");
    }

    [Category("PopUps")]
    public void Spectacular()
    {
        UIManager.Instance.InstantiatePopUps("hhhhhhh");
    }

    [Category("PopUps")]
    public void BestWord()
    {
        UIManager.Instance.BestWordPopUp("HanCute");
    }

    [Category("Cheat")]
    public void AddMoreHints()
    {
        HintCounter.Instance.DebugUpdateCounter(10);
    }
}