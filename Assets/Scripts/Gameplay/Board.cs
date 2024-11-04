using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Board : Singleton<Board>
{
    [SerializeField] private GameObject _tilePrefab, _linePrefab;

    private GameDictionary _dictionary;
    private RectTransform _board;
    private EventSystem _eventSystem;
    private GraphicRaycaster _graphicRaycaster;
    private List<Tile> _tileList = new(), _selectingTiles = new(), _lastSelectedTiles;
    private List<GameObject> _lineList = new();
    private TileConfigManager _configManager = new();


    private bool _isDragging;
    private string _currentWord, _selectedWord;

    private const int ColsEven = 6, ColsOdd = 5, Rows = 10;

    public void Initialize()
    {
        _dictionary = new GameDictionary();
        _configManager.LoadConfigs();
        _board = GetComponent<RectTransform>();
        _graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        _eventSystem = FindObjectOfType<EventSystem>();
    }

    private void Start()
    {
        Initialize();
        GenerateBoard();
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void GenerateBoard()
    {
        _tileList.Clear();

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
                _tileList.Add(component);
            }
        }
    }

    #region InputHandle
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _isDragging = true;

                DeselectAll();
                DisconnectAll();

                _lastSelectedTiles = new List<Tile>(_selectingTiles);
                _selectingTiles.Clear();

                _selectedWord = _currentWord;
                _currentWord = null;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                _isDragging = false;
            }
            else if (touch.phase == TouchPhase.Moved && _isDragging)
            {
                HandleDragging();
            }
        }
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
            UpdateWordState(tile);
        }
        else
        {
            tile.Select();
            Connect(tile);
            _selectingTiles.Add(tile);

            _currentWord += tile.Letter;
            UpdateWordState(tile);
        }
    }

    private void SelectTile(Tile tile)
    {
        tile.Select();
        _selectingTiles.Add(tile);
        _currentWord += tile.Letter;

        UpdateWordState(tile);
    }

    private void DeselectAll()
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

    private void DisconnectLastLine()
    {
        Destroy(_lineList[^1]);
        _lineList.Remove(_lineList[^1]);
    }

    private void DisconnectAll()
    {
        foreach (var line in _lineList)
        {
            Destroy(line.gameObject);
        }

        _lineList.Clear();
    }
    #endregion

    private void Validate()
    {
        foreach (var tile in _selectingTiles)
        {
            tile.ValidateWord();
        }

        foreach (var line in _lineList)
        {
            line.GetComponent<Image>().color = new Color(78 / 255f, 200 / 255f, 75 / 255f);
        }
    }

    private void Invalidate()
    {
        foreach (var tile in _selectingTiles)
        {
            tile.InvalidateWord();
        }

        foreach (var line in _lineList)
        {
            line.GetComponent<Image>().color = new Color(110 / 255f, 110 / 255f, 110 / 255f);
        }
    }

    private void UpdateWordState(Tile tile)
    {
        if (_currentWord.Length > 1)
        {
            if (_dictionary.CheckWord(_currentWord))
            {
                Validate();
            }
            else
            {
                Invalidate();
            }
        }
        else
        {
            Invalidate();
        }
    }
}
