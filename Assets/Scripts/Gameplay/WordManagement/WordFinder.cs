using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class WordFinder : MonoBehaviour
{
    private GameDictionary _dictionary;
    private bool[,] _visited;
    private int _hintIndex;
    private List<Vector2Int> _currentHint;
    private readonly Dictionary<string, List<Vector2Int>> _foundWords = new();

    #region WordManagement
    private void FindAllWords()
    {
        _foundWords.Clear();
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

        if (!_dictionary.IsPrefix(currentWord))
        {
            currentPath.RemoveAt(currentPath.Count - 1);
            return;
        }

        if (_dictionary.CheckWord(currentWord))
        {
            _foundWords[currentWord] = new List<Vector2Int>(currentPath);
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
        if (_currentHint == null || !CheckIfHintIsLost())
        {
            _hintIndex = 0;
            _currentHint = _foundWords.Values.FirstOrDefault(word => word.Count >= 5) ?? _foundWords.Values.FirstOrDefault();
            _foundWords.Keys.FirstOrDefault(word => _foundWords[word] == _currentHint);
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
                AudioManager.Instance.PlaySFX("HintCompleted")
            );
        }

        sequence.Play();
    }

    private bool CheckIfHintIsLost()
    {
        return _foundWords.Values.Any(array => array == _currentHint);
    }
    #endregion
}
