using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DemoTiles : Singleton<DemoTiles>
{
    [SerializeField] private GameObject _linePrefab, _cursor;
    [SerializeField] private List<Tile> _tileList;

    private bool IsDragging;
    private bool IsClosing;
    private Vector3 _originalPos, _targetPos;
    private EventSystem _eventSystem;
    private GraphicRaycaster _graphicRaycaster;
    private List<Tile> _selectingTiles = new();
    private List<GameObject> _lineList = new();
    private Tween _cursorTween;

    private void OnEnable()
    {
        _graphicRaycaster = GetComponentInParent<GraphicRaycaster>();
        _eventSystem = FindObjectOfType<EventSystem>();
        SetTemporaryCoordinates();
    }

    private void Update()
    {
        if (!IsClosing)
        {
            HandleInput();
        }
    }

    public void CursorAnimation()
    {
        _originalPos = _tileList[0].transform.position;
        _targetPos = _tileList[4].transform.position;

        AnimateCursor();
    }

    private void AnimateCursor()
    {
        if (_cursorTween != null && _cursorTween.IsActive())
        {
            _cursorTween.Kill();
            _cursorTween = null;
        }

        _cursor.SetActive(true);
        _cursorTween = _cursor.transform.DOMove(_targetPos, 1.5f).OnComplete(() =>
        {
            _cursor.transform.position = _originalPos;
            AnimateCursor();
        });
    }

    private void StopAnimation()
    {
        if (_cursorTween != null && _cursorTween.IsActive())
        {
            _cursorTween.Kill();
        }

        _cursor.transform.DOKill();
        _cursor.SetActive(false);
        _cursor.transform.position = _originalPos;
    }

    private void SetTemporaryCoordinates()
    {
        for (var i = 0; i < _tileList.Count; i++)
        {
            _tileList[i].Column = i;
            _tileList[i].Color = Color.white;
            _tileList[i].Deselect();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DeselectAll();
            DisconnectAll();
            StopAnimation();

            _selectingTiles.Clear();
            IsDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsDragging = false;

            if (_selectingTiles.Count == 0)
            {
                AnimateCursor();
            }
        }

        if (IsDragging)
        {
            HandleDragging();
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
        }
        else
        {
            tile.Select();
            Connect(tile);
            _selectingTiles.Add(tile);
        }

        if (_selectingTiles.Count == 5 && _selectingTiles[0] == _tileList[0])
        {
            foreach (var t in _selectingTiles)
            {
                t.ValidateWord();
            }

            foreach (var line in _lineList)
            {
                line.GetComponent<UILine>().Validate();
            }

            UIManager.Instance.UICanvasGroup.blocksRaycasts = false;
            IsClosing = true;

            transform.DOScale(Vector3.zero, 0.5f).SetDelay(0.5f).OnComplete(() =>
            {
                IsClosing = false;
                DeselectAll();
                UIManager.Instance.ToggleInstructionPanel(false);
            });
        }
        else
        {
            foreach (var t in _selectingTiles)
            {
                t.InvalidateWord();
            }

            foreach (var line in _lineList)
            {
                line.GetComponent<UILine>().Invalidate();
            }
        }
    }

    private void SelectTile(Tile tile)
    {
        tile.Select();
        _selectingTiles.Add(tile);
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

        var line = Instantiate(_linePrefab, transform);
        line.transform.SetSiblingIndex(0);
        line.GetComponent<UILine>().CreateLine(_selectingTiles[^1].transform.position, tile.transform.position);

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
}