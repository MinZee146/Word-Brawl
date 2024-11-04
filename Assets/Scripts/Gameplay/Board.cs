using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board>
{
    [SerializeField] private GameObject _tilePrefab;
    private RectTransform _board;
    private const int ColsEven = 6, ColsOdd = 5, Rows = 10;
    private List<Tile> _tileList = new();
    private TileConfigManager _configManager = new();

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
}
