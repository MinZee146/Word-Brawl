using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class FoundWordData
{
    public List<Vector2Int> Path { get; set; }
    public int Score { get; set; }

    public FoundWordData(List<Vector2Int> path, int score)
    {
        Path = path;
        Score = score;
    }
}

public class WordFinder : Singleton<WordFinder>
{
    private bool[,] _visited;
    private int _hintIndex;
    private List<Vector2Int> _currentHint;

    private string _bestWord;
    private int _bestScore;

    public string BestWord => _bestWord;
    public int BestScore => _bestScore;

    #region WordManagement
    public void FindAllWords()
    {
        _bestWord = "";
        _bestScore = 0;

        Board.Instance.FoundWords.Clear();
        _visited = new bool[Board.Rows, Board.ColsEven];

        foreach (var tile in Board.Instance.TileList)
        {
            var currentWord = "";
            var currentScore = 0;
            var currentPath = new List<Vector2Int>();

            DFS(tile, currentWord, currentScore, currentPath);
        }

        FoundWords();
    }

    private void FoundWords()
    {
        foreach (var entry in Board.Instance.FoundWords)
        {
            string word = entry.Key;
            int score = entry.Value.Score;

            // Debug.Log($"Word: {word}, Score: {score}");
        }

        Debug.Log($"BestWord: {_bestWord}, BestScore: {_bestScore}");
    }

    private void DFS(Tile tile, string currentWord, int currentScore, List<Vector2Int> currentPath)
    {
        currentWord += tile.Letter;
        currentScore += tile.Score;
        currentPath.Add(new Vector2Int(tile.Row, tile.Column));

        if (!GameDictionary.Instance.IsPrefix(currentWord))
        {
            currentPath.RemoveAt(currentPath.Count - 1);
            return;
        }

        if (GameDictionary.Instance.CheckWord(currentWord))
        {
            var wordScore = currentScore * currentWord.Length;

            if (wordScore > _bestScore)
            {
                _bestScore = wordScore;
                _bestWord = currentWord;
            }

            Board.Instance.FoundWords[currentWord] = new FoundWordData(new List<Vector2Int>(currentPath), wordScore);
        }

        _visited[tile.Row, tile.Column] = true;

        foreach (var neighbor in GetNeighbors(tile).Where(neighbor => !_visited[neighbor.Row, neighbor.Column]))
        {
            DFS(neighbor, currentWord, currentScore, currentPath);
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
        if (HintCounter.Instance.CurrentHintCounter == 0)
        {
            UIManager.Instance.ToggleHintsRunOutPanel();
            return;
        }

        GameUIController.Instance.ToggleHintAndConfirm(display: false);
        UIManager.Instance.IsInteractable = false;

        if (_currentHint == null || !CheckIfHintIsLost())
        {
            _hintIndex = 0;

            var wordsWithMinLength = Board.Instance.FoundWords.Values
            .Where(data => data.Path.Count >= 5)
            .ToList();

            _currentHint = wordsWithMinLength.Any()
                ? wordsWithMinLength[Random.Range(0, wordsWithMinLength.Count)].Path
                : Board.Instance.FoundWords.Values.ElementAt(Random.Range(0, Board.Instance.FoundWords.Values.Count)).Path;

            Board.Instance.FoundWords.Keys.FirstOrDefault(word => Board.Instance.FoundWords[word].Path == _currentHint);
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

        if (_hintIndex < _currentHint.Count)
        {
            HintCounter.Instance.UpdateCounter();
        }
    }

    private bool CheckIfHintIsLost()
    {
        return Board.Instance.FoundWords.Values.Any(data => data.Path == _currentHint);
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
        var positions = Board.Instance.FoundWords[word].Path;
        return positions.Select(pos => GetNeighbors(
            Board.Instance.TileList.First(t => t.Row == pos.x && t.Column == pos.y))).
            Any(neighbors => neighbors.
                Any(t => t.Letter == letter));
    }
    #endregion
}
