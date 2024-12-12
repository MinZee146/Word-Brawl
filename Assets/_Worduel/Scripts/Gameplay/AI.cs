using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI : Singleton<AI>
{
    [NonSerialized] public string ForcedWord;
    [NonSerialized] public bool PreferLong, PreferShort;

    public IEnumerator<float> AITurn()
    {
        if (!PowerUpsManager.Instance.CheckExtraTurn)
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(ChoosePowerUp()));

        var longWords = Board.Instance.FoundWords.Keys.Where(word => word.Length >= 5).ToList();
        var shortWords = Board.Instance.FoundWords.Keys.Where(word => word.Length < 5).ToList();

        var randomValue = Random.Range(0f, 1f);
        List<string> selectedList;

        if (PreferLong && longWords.Count > 0)
        {
            selectedList = longWords;
        }
        else if (PreferShort && shortWords.Count > 0)
        {
            selectedList = shortWords;
        }
        else if (randomValue <= 0.4f && longWords.Count > 0)
        {
            selectedList = longWords;
        }
        else
        {
            selectedList = shortWords;
        }

        var randomWord = string.IsNullOrEmpty(ForcedWord) ? selectedList[Random.Range(0, selectedList.Count)] : ForcedWord;

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(Board.Instance.OpponentSelect(randomWord)));
        yield return Timing.WaitForSeconds(0.75f);
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(Board.Instance.PopAndRefresh()));

        ForcedWord = null;

        if (!GameManager.Instance.IsGameOver && PowerUpsManager.Instance.CheckExtraTurn)
        {
            yield return Timing.WaitForSeconds(Random.Range(1f, 2f));
            Timing.RunCoroutine(AITurn());
        }
    }

    private IEnumerator<float> ChoosePowerUp()
    {
        if (PowerUpsManager.Instance.PowerUpCounts() == 0 || GameFlowManager.Instance.Turn <= 2)
        {
            yield return Timing.WaitForSeconds(Random.Range(1f, 2f));
        }
        else
        {
            yield return Timing.WaitForSeconds(Random.Range(0.5f, 1.5f));

            PowerUpsManager.Instance.AIUseRandomPowerUp();
            UIManager.Instance.ToggleOpponentPowerUpPanel(true);
            yield return Timing.WaitForSeconds(2f);

            UIManager.Instance.ToggleOpponentPowerUpPanel(false);
            yield return Timing.WaitForSeconds(0.5f);

            {
                if (PowerUpsManager.Instance.CheckRevealWord)
                {
                    yield return Timing.WaitForSeconds(0.25f);
                    UIManager.Instance.ToggleRevealWordPopUp(true);
                    yield return Timing.WaitForSeconds(2f);
                    UIManager.Instance.ToggleRevealWordPopUp(false);
                    yield return Timing.WaitForSeconds(0.25f);
                }

                if (PowerUpsManager.Instance.CheckShuffle)
                {
                    Board.Instance.ShuffleBoard();
                }

                if (PowerUpsManager.Instance.CheckReplaceLetter)
                {
                    yield return Timing.WaitUntilDone(Timing.RunCoroutine(AIReplaceTile()));
                }
            }

            yield return Timing.WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    private IEnumerator<float> AIReplaceTile()
    {
        var incompleteWord = WordFinder.Instance.FindIncompleteWord();
        var lastChar = incompleteWord[^1];
        var currentWord = incompleteWord[..^1];

        var lastLetterPos = Board.Instance.FoundWords[currentWord].Path.Last();
        var lastLetterTile = Board.Instance.TileList.FirstOrDefault(t => t.Row == lastLetterPos.x && t.Column == lastLetterPos.y);
        var neighbors = Board.Instance.TileList.Where(lastLetterTile.IsAdjacent).ToList();

        var randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
        while (Board.Instance.FoundWords[currentWord].Path.Contains(new Vector2Int(randomNeighbor.Row, randomNeighbor.Column)))
        {
            randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
        }

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(AIReplaceTile(lastChar, randomNeighbor)));

        Board.Instance.FoundWords[incompleteWord] = new FoundWordData(new List<Vector2Int>(Board.Instance.FoundWords[currentWord].Path) { new(randomNeighbor.Row, randomNeighbor.Column) }, 0);
        ForcedWord = incompleteWord;
    }

    private IEnumerator<float> AIReplaceTile(char lastChar, Tile randomNeighbor)
    {
        yield return Timing.WaitForSeconds(0.25f);
        randomNeighbor.Select();
        yield return Timing.WaitForSeconds(0.5f);
        randomNeighbor.Deselect();
        yield return Timing.WaitForSeconds(0.5f);

        UIManager.Instance.ToggleTileReplacePopUp(true);
        yield return Timing.WaitForSeconds(1f);
        UIManager.Instance.SetReplaceText(lastChar);
        yield return Timing.WaitForSeconds(1f);
        UIManager.Instance.ToggleTileReplacePopUp(false);
        yield return Timing.WaitForSeconds(1f);

        randomNeighbor.transform.DOScale(Vector3.one * 0.1f, 0.3f).OnComplete(() =>
         {
             randomNeighbor.SetTileConfig(Board.Instance.GetConfig(lastChar));
             randomNeighbor.transform.DOScale(Vector3.one, 0.3f);
         });
    }
}
