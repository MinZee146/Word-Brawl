using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Board : Singleton<Board>
{
    [SerializeField] private GameObject _tilePrefab;

    private RectTransform _board;
    private List<Tile> _tileList = new(), _selectingTiles = new(), _lastSelectedTiles;
    private TileConfigManager _configManager = new();
    private bool _isDragging;
    private const int ColsEven = 6, ColsOdd = 5, Rows = 10;

    public void Initialize()
    {
        _configManager.LoadConfigs();
        _board = GetComponent<RectTransform>();
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
                _lastSelectedTiles = new List<Tile>(_selectingTiles);
                _selectingTiles.Clear();
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
        var mousePosition = Input.mousePosition;

        foreach (var tile in _tileList)
        {
            var tileRectTransform = tile.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(tileRectTransform, mousePosition))
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
    }

    private void HandleTileSelection(Tile tile)
    {
        if (_selectingTiles.Contains(tile))
        {
            if (tile != _selectingTiles[^2]) return;

            _selectingTiles[^1].Deselect();
            _selectingTiles.Remove(_selectingTiles[^1]);
        }
        else
        {
            SelectTile(tile);
        }
    }

    private void SelectTile(Tile tile)
    {
        tile.Select();
        _selectingTiles.Add(tile);
    }

    private void DeselectAll()
    {
        foreach (var tile in _selectingTiles)
        {
            tile.Deselect();
        }
    }
    #endregion
}
