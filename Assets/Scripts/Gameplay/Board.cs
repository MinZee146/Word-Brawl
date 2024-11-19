using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MEC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Board : Singleton<Board>
{
    [SerializeField] private GameObject _tilePrefab, _linePrefab;

    [NonSerialized] public List<Tile> TileList = new();
    [NonSerialized] public Dictionary<string, FoundWordData> FoundWords = new();

    public bool IsDragging;
    public const int ColsEven = 6, ColsOdd = 5, Rows = 10;
    public TileConfig GetRandomLetter() => _configManager.GetRandomLetter();
    public TileConfig GetConfig(char letter) => _configManager.GetConfig(letter);
    public event Action HandleTileReplace;

    private RectTransform _board;
    private List<Tile> _selectingTiles = new(), _lastSelectedTiles;
    private List<GameObject> _lineList = new();
    private TileConfigManager _configManager = new();

    private string _currentWord, _selectedWord;
    private int _currentScore;

    private EventSystem _eventSystem;
    private GraphicRaycaster _graphicRaycaster;

    public void Initialize()
    {
        _configManager.LoadConfigs();
        _configManager.HandleConfigsLoaded += OnConfigsLoaded;

        _board = GetComponent<RectTransform>();
        _graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        _eventSystem = FindObjectOfType<EventSystem>();
    }

    private void Start()
    {
        Initialize();
    }

    public void NewGame()
    {
        GenerateBoard();
        Notifier.Instance.OnTurnChanged();
    }

    private void OnConfigsLoaded()
    {
        NewGame();
    }

    private void Update()
    {
        HandleInput();
    }

    #region GenerateBoard
    private void GenerateBoard()
    {
        TileList.Clear();

        foreach (RectTransform child in _board)
        {
            Destroy(child.gameObject);
        }

        var hexWidth = _tilePrefab.GetComponent<RectTransform>().rect.width;
        var hexHeight = _tilePrefab.GetComponent<RectTransform>().rect.height;
        var boardWidth = ColsEven * hexWidth;
        var boardHeight = Rows * hexHeight * 0.8f;

        var startX = -boardWidth / 2f + hexWidth * 0.5f;
        var startY = -boardHeight / 2f + hexHeight * 0.55f;

        for (var row = 0; row < Rows; row++)
        {
            var cols = (row % 2 != 0) ? ColsOdd : ColsEven;

            for (var col = 0; col < cols; col++)
            {
                var xPos = startX + col * hexWidth;

                if (row % 2 != 0)
                {
                    xPos += hexWidth / 2f;
                }

                var yPos = startY + row * hexHeight * 0.75f;
                var tile = Instantiate(_tilePrefab, _board);

                // Set the tile's RectTransform position relative to the board
                var rectTransform = tile.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(xPos, yPos);

                tile.name = $"({row},{col})";

                var component = tile.GetComponent<Tile>();
                component.Column = col;
                component.Row = row;
                component.IsRowEven = row % 2 == 0;

                component.Deselect();
                component.SetTileConfig(_configManager.GetRandomLetter());
                TileList.Add(component);
            }
        }

        WordFinder.Instance.FindAllWords();
    }
    #endregion

    #region InputHandle
    private void HandleInput()
    {
        if (GameFlowManager.Instance.IsPlayerTurn && UIManager.Instance.CheckCanInteractBoard())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (UIManager.Instance.IsInspectingBoard)
                {
                    if (UIManager.Instance.InspectPanel == "PowerUps")
                    {
                        UIManager.Instance.ToggleInspectPowerUps();
                    }

                    if (UIManager.Instance.InspectPanel == "Replace")
                    {
                        UIManager.Instance.ToggleInspectReplace();
                    }
                }
                else
                {
                    HandleTouching();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                IsDragging = false;
            }

            if (IsDragging)
            {
                HandleDragging();
            }
        }
    }

    private void HandleTouching()
    {
        var pos = Input.mousePosition;

        if (!RectTransformUtility.RectangleContainsScreenPoint(GameUIController.Instance.ConfirmButtonRect(), pos))
        {
            GameUIController.Instance.ToggleHintAndConfirm();
        }

        if (RectTransformUtility.RectangleContainsScreenPoint(GameUIController.Instance.BoardRectTransform(), pos))
        {
            IsDragging = true;
        }

        DeselectAll();
        DisconnectAll();

        _lastSelectedTiles = new List<Tile>(_selectingTiles);
        _selectingTiles.Clear();

        _selectedWord = _currentWord;
        _currentWord = null;

        WordDisplay.Instance.UndisplayWordAndScore();
    }

    private void HandleDragging()
    {
        var pointerEventData = new PointerEventData(_eventSystem)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        _graphicRaycaster.Raycast(pointerEventData, results);

        foreach (var tile in results.Select(result => result.gameObject.GetComponent<Tile>()).Where(tile => tile))
        {
            if (_selectingTiles.Count == 0)
            {
                SelectTile(tile);
            }
            else if (_selectingTiles[^1].IsAdjacent(tile))
            {
                HandleTileSelection(tile);
            }

            if (PowerUpsManager.Instance.CheckReplaceLetter)
            {
                HandleTileReplace?.Invoke();
            }

            break;
        }
    }

    private void HandleTileSelection(Tile tile)
    {
        if (_selectingTiles.Contains(tile))
        {
            if (tile != _selectingTiles[^2]) return;

            _selectingTiles[^1].Deselect();
            _selectingTiles.Remove(_selectingTiles[^1]);

            DisconnectLastLine();
            _currentWord = _currentWord?[..^1];
            WordDisplay.Instance.UpdateWordState(tile, _currentWord, ref _currentScore, _lineList, _selectingTiles);
        }
        else
        {
            tile.Select();
            Connect(tile);
            _selectingTiles.Add(tile);

            _currentWord += tile.Letter;
            WordDisplay.Instance.UpdateWordState(tile, _currentWord, ref _currentScore, _lineList, _selectingTiles);
        }
    }

    private void SelectTile(Tile tile)
    {
        tile.Select();
        _selectingTiles.Add(tile);
        _currentWord += tile.Letter;

        WordDisplay.Instance.UpdateWordState(tile, _currentWord, ref _currentScore, _lineList, _selectingTiles);
    }

    public void DeselectAll()
    {
        foreach (var tile in _selectingTiles)
        {
            tile.Deselect();
        }
    }

    private void Connect(Tile tile)
    {
        if (_selectingTiles.Contains(tile)) return;

        var line = Instantiate(_linePrefab, _board);
        line.transform.SetSiblingIndex(0);
        line.GetComponent<UILine>().CreateLine(_selectingTiles[^1].transform.position, tile.transform.position);

        _lineList.Add(line);
    }

    private void Connect(Tile firstTile, Tile secondTile)
    {
        var line = Instantiate(_linePrefab, _board);
        line.transform.SetSiblingIndex(0);
        line.GetComponent<UILine>().CreateLine(firstTile.transform.position, secondTile.transform.position);

        _lineList.Add(line);
    }

    private void DisconnectLastLine()
    {
        Destroy(_lineList[^1]);
        _lineList.Remove(_lineList[^1]);
    }

    public void DisconnectAll()
    {
        foreach (var line in _lineList)
        {
            Destroy(line.gameObject);
        }

        _lineList.Clear();
    }
    #endregion

    #region TilesPop
    public void ConfirmSelection()
    {
        if (!GameDictionary.Instance.CheckWord(_selectedWord)) return;
        Debug.Log($"Player Selected: {_selectedWord} ({_currentScore})");

        Timing.RunCoroutine(PopAndRefresh());
    }

    public IEnumerator<float> PopAndRefresh()
    {
        GameUIController.Instance.ToggleHintAndConfirm(display: false);
        UIManager.Instance.IsInteractable = false;

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(PopSelectedTiles()));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(UIManager.Instance.ShowPopUp(_selectedWord)));

        PlayerStatsManager.Instance.UpdateStats(_selectedWord, _currentWord, _currentScore);
        PowerUpsManager.Instance.CleanPowerUp();

        _selectedWord = null;
        _currentWord = null;
        _currentScore = 0;
        _selectingTiles.Clear();

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(RandomizeOneTile()));
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(GameManager.Instance.CheckForGameOver()));

        if (!GameManager.Instance.IsGameOver)
        {
            if (!PowerUpsManager.Instance.CheckExtraTurn)
            {
                yield return Timing.WaitForSeconds(0.5f);
                GameFlowManager.Instance.NextTurn();
            }
        }

        UIManager.Instance.IsInteractable = true;
        GameUIController.Instance.ToggleHintAndConfirm();
    }

    private IEnumerator<float> PopSelectedTiles()
    {
        foreach (var tile in _lastSelectedTiles)
        {
            TileList.Remove(tile);
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(tile.PopAndDestroy()));

            Pop(tile);
        }
    }

    private void Pop(Tile tile)
    {
        if (tile.IsRowEven)
        {
            if (!FallAndReplace(tile, tile.Column - 1, tile.Row + 1))
            {
                FallAndReplace(tile, tile.Column, tile.Row + 1);
            }
        }
        else
        {
            if (!FallAndReplace(tile, tile.Column, tile.Row + 1))
            {
                FallAndReplace(tile, tile.Column + 1, tile.Row + 1);
            }
        }
    }

    private bool FallAndReplace(Tile tile, int targetColumn, int targetRow)
    {
        var targetTile = TileList.FirstOrDefault(t => t.Column == targetColumn && t.Row == targetRow);

        if (!targetTile) return false;

        Pop(targetTile);

        targetTile.transform.DOMove(tile.transform.position, 0.1f, false);
        targetTile.Row = tile.Row;
        targetTile.Column = tile.Column;
        targetTile.IsRowEven = tile.IsRowEven;
        targetTile.name = $"({tile.Row},{tile.Column})";

        return true;
    }

    private IEnumerator<float> RandomizeOneTile()
    {
        if (TileList.Count != 0)
        {
            var random = TileList[UnityEngine.Random.Range(0, TileList.Count)];
            char randomChar = (char)UnityEngine.Random.Range('A', 'Z' + 1);

            random.transform.DOScale(Vector3.one * 0.1f, 0.3f).OnComplete(() =>
            {
                random.SetTileConfig(GetConfig(randomChar));
                random.transform.DOScale(Vector3.one, 0.3f);
            });

            yield return Timing.WaitForSeconds(0.3f);
        }
    }
    #endregion

    #region OpponentSelect
    public IEnumerator<float> OpponentSelect(string word)
    {
        var firstPos = FoundWords[word].Path.First();
        var previousTile = TileList.FirstOrDefault(t => t.Row == firstPos.x && t.Column == firstPos.y);

        previousTile.Select();
        _selectingTiles.Add(previousTile);
        _currentWord += previousTile.Letter;

        foreach (var tile in from pos in FoundWords[word].Path
                             where pos != firstPos
                             select TileList.FirstOrDefault(t => t.Row == pos.x && t.Column == pos.y))
        {
            yield return Timing.WaitForSeconds(0.3f);

            tile.Select();
            Connect(tile, previousTile);
            _selectingTiles.Add(tile);
            previousTile = tile;

            _currentWord += tile.Letter;
            WordDisplay.Instance.UpdateWordState(tile, _currentWord, ref _currentScore, _lineList, _selectingTiles);
        }

        _lastSelectedTiles = new List<Tile>(_selectingTiles);

        Debug.Log($"Opponent Selected: {word} ({_currentScore})");
    }
    #endregion

    #region PowerUpsHandle
    public void ReplaceSelectingTileWith(char letter)
    {
        var tile = _selectingTiles[^1];
        TileList.Remove(tile);
        tile.SetTileConfig(_configManager.Configs.FirstOrDefault(tileStat => tileStat.Letter == letter));
        TileList.Add(tile);
        tile.Deselect();

        Timing.RunCoroutine(GameManager.Instance.CheckForGameOver());
    }

    public void ClearHandleTileReplaceListeners()
    {
        HandleTileReplace = null;
    }

    public void ShuffleBoard()
    {
        foreach (var tile in TileList)
        {
            tile.SetTileConfig(GetRandomLetter());
            tile.Deselect();
        }

        Timing.RunCoroutine(GameManager.Instance.CheckForGameOver());
    }
    #endregion
}