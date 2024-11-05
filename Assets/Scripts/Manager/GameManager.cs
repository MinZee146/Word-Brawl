using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
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

    public bool CheckForGameOver()
    {
        WordFinder.Instance.FindAllWords();
        return Board.Instance.FoundWords.Count > 0;
    }
}
