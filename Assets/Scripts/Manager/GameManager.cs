using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private bool _isPlayerTurn;
    public bool IsPlayerTurn => _isPlayerTurn;
    private int _turn;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameDictionary.Instance.Initialize();
        AudioManager.Instance.Initialize();
        PowerupsManager.Instance.InitializePowerUps();
        NameRegister.Instance.Initialize();
    }

    public void CheckForGameOver()
    {
        WordFinder.Instance.FindAllWords();
        if (Board.Instance.FoundWords.Count == 0)
        {
            UIManager.Instance.ToggleGameOverScreen();
        }
    }

    public void NextTurn()
    {
        _turn++;
        _isPlayerTurn = !_isPlayerTurn;

        UIController.Instance.ToggleHintAndConfirm();

        if (_turn > 2 && _isPlayerTurn && PowerupsManager.Instance.PowerUpCounts() > 0)
        {
            UIManager.Instance.TogglePowerupsPanel();
        }
    }
}
