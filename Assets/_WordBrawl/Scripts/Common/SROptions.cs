using System.ComponentModel;
using UnityEngine;

public partial class SROptions
{
    [Category("Debug")]
    public void GameOver()
    {
        UIManager.Instance.ToggleGameOverPanel(true);
    }

    [Category("Debug")]
    public void NewRound()
    {
        GameFlowManager.Instance.NextRound();
    }

    [Category("Debug")]
    public void ClearAllPrefabs()
    {
        PlayerPrefs.DeleteAll();
    }

    [Category("Debug")]
    public void Instruction()
    {
        UIManager.Instance.ToggleInstructionPanel(true);
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

    [Category("Cheats")]
    public void AddCoins()
    {
        CurrencyManager.Instance.UpdateCoins(1000);
    }

    [Category("Cheats")]
    public void GrantHints()
    {
        var currentHint = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, 5);
        PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, currentHint + 5);
        HintCounter.Instance.FetchHintPref();
    }
}
