using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        PowerupsManager.Instance.InitializePowerUps();
    }
    
    public bool CheckForGameOver()
    {
        WordFinder.Instance.FindAllWords();
        return Board.Instance.FoundWords.Count > 0;
    }
}
