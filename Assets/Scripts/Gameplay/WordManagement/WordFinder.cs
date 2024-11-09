using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class WordFinder : Singleton<WordFinder>
{
    private bool[,] _visited;
    private int _hintIndex;
    private List<Vector2Int> _currentHint;

    #region WordManagement
    public void FindAllWords()
    {
        Board.Instance.FoundWords.Clear();
        _visited = new bool[Board.Rows, Board.ColsEven];

        foreach (var tile in Board.Instance.TileList)
        {
            var currentWord = "";
            var currentPath = new List<Vector2Int>();
            DFS(tile, currentWord, currentPath);
        }
    }

    private void DFS(Tile tile, string currentWord, List<Vector2Int> currentPath)
    {
        currentWord += tile.Letter;
        currentPath.Add(new Vector2Int(tile.Row, tile.Column));
        if (!GameDictionary.Instance.IsPrefix(currentWord))
        {
            currentPath.RemoveAt(currentPath.Count - 1);
            return;
        }

        if (GameDictionary.Instance.CheckWord(currentWord))
        {
            Board.Instance.FoundWords[currentWord] = new List<Vector2Int>(currentPath);
        }

        _visited[tile.Row, tile.Column] = true;

        foreach (var neighbor in GetNeighbors(tile).Where(neighbor => !_visited[neighbor.Row, neighbor.Column]))
        {
            DFS(neighbor, currentWord, currentPath);
        }

        _visited[tile.Row, tile.Column] = false;
        currentPath.RemoveAt(currentPath.Count - 1);
    }

    private List<Tile> GetNeighbors(Tile tile)
    {
        return Board.Instance.TileList.Where(tile.IsAdjacent).ToList();
    }
    #endregion

    #region Hint
    public void GetHint()
    {
        UIManager.Instance.IsInteractable = false;
        GameUIController.Instance.ToggleHintAndConfirm(display: false);

        if (_currentHint == null || !CheckIfHintIsLost())
        {
            _hintIndex = 0;
            _currentHint = Board.Instance.FoundWords.Values.FirstOrDefault(word => word.Count >= 5)
                           ?? Board.Instance.FoundWords.Values.FirstOrDefault();
            Board.Instance.FoundWords.Keys.FirstOrDefault(word => Board.Instance.FoundWords[word] == _currentHint);
        }

        if (_hintIndex < _currentHint.Count)
        {
            _hintIndex++;
        }

        var subList = _currentHint.GetRange(0, _hintIndex);
        var sequence = DOTween.Sequence();

        foreach (var tile in from pos in subList
                             select Board.Instance.TileList.FirstOrDefault(t => t.Row == pos.x && t.Column == pos.y))
        {
            sequence.Append(tile.Hint());
        }

        if (_hintIndex == _currentHint.Count)
        {
            var subSequence = DOTween.Sequence();

            foreach (var tile in from pos in subList
                                 select Board.Instance.TileList.FirstOrDefault(t => t.Row == pos.x && t.Column == pos.y))
            {
                subSequence.Join(tile.Hint());
            }

            sequence.Append(subSequence);

            sequence.OnComplete(() =>
            {
                AudioManager.Instance.PlaySFX("HintCompleted");

                UIManager.Instance.IsInteractable = true;
                GameUIController.Instance.ToggleHintAndConfirm();
            });
        }
        else
        {
            sequence.OnComplete(() =>
            {
                UIManager.Instance.IsInteractable = true;
                GameUIController.Instance.ToggleHintAndConfirm();
            });
        }

        sequence.Play();
    }

    private bool CheckIfHintIsLost()
    {
        return Board.Instance.FoundWords.Values.Any(array => array == _currentHint);
    }
    #endregion

    #region LetterReplace
    public string FindIncompleteWord()
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (var targetLength = 5; targetLength > 0; targetLength--)
        {
            foreach (var potentialWord in from word in Board.Instance.FoundWords.Keys
                                          where word.Length == targetLength - 1
                                          from letter in alphabet
                                          let potentialWord = word + letter
                                          where GameDictionary.Instance.CheckWord(potentialWord) && !IsLetterNear(word, letter)
                                          select potentialWord)
            {
                return potentialWord;
            }
        }
        return null;
    }

    private bool IsLetterNear(string word, char letter)
    {
        var positions = Board.Instance.FoundWords[word];
        return positions.Select(pos => GetNeighbors(
            Board.Instance.TileList.First(t => t.Row == pos.x && t.Column == pos.y))).
            Any(neighbors => neighbors.
                Any(t => t.Letter == letter));
    }
    #endregion
}
