using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "LetterReplace", menuName = "Powerups/LetterReplace")]
public class LetterReplace : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "ReplaceLetter";

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            Board.Instance.HandleTileReplace += () =>
            {
                UIManager.Instance.ToggleTileReplacePopUp();
            };
        }
        else
        {
            var incompleteWord = WordFinder.Instance.FindIncompleteWord();
            var lastChar = incompleteWord[^1];
            var currentWord = incompleteWord[..^1];
            var lastLetterPos = Board.Instance.FoundWords[currentWord].Last();
            var lastLetterTile = Board.Instance.TileList.FirstOrDefault(t => t.Row == lastLetterPos.x && t.Column == lastLetterPos.y);
            var neighbors = Board.Instance.TileList.Where(lastLetterTile.IsAdjacent).ToList();
            var randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];

            while (Board.Instance.FoundWords[currentWord].Contains(new Vector2Int(randomNeighbor.Row, randomNeighbor.Column)))
            {
                randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
            }

            Debug.Log($"AI selected to replace {randomNeighbor.Letter} with {lastChar}");

            Timing.RunCoroutine(AIReplaceTile(lastChar, randomNeighbor));

            //Add this to found words temporarily to be immediately selected
            Board.Instance.FoundWords[incompleteWord] = new List<Vector2Int>(Board.Instance.FoundWords[currentWord]) { new(randomNeighbor.Row, randomNeighbor.Column) };
            AI.Instance.ForcedWord = incompleteWord;
        }
    }

    private IEnumerator<float> AIReplaceTile(char lastChar, Tile randomNeighbor)
    {
        UIManager.Instance.ToggleTileReplacePopUp();
        yield return Timing.WaitForSeconds(0.5f);
        UIManager.Instance.SetReplaceText(lastChar);
        yield return Timing.WaitForSeconds(1f);
        UIManager.Instance.ToggleTileReplacePopUp();

        randomNeighbor.transform.DOScale(Vector3.one * 0.1f, 0.3f).OnComplete(() =>
         {
             randomNeighbor.SetTileConfig(Board.Instance.GetConfig(lastChar));
             randomNeighbor.transform.DOScale(Vector3.one, 0.3f);
         });
    }
}