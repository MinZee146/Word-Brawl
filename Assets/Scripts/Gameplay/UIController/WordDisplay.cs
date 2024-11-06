using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordDisplay : Singleton<WordDisplay>
{
    [SerializeField] private TextMeshProUGUI _wordDisplayText;
    [SerializeField] private RectTransform _wordDisplayRect;

    public void UndisplayWordAndScore()
    {
        _wordDisplayText.text = string.Empty;
        _wordDisplayRect.sizeDelta = new Vector2(0, 0);
    }

    private void Validate(List<GameObject> lineList, List<Tile> selectingTiles)
    {
        foreach (var tile in selectingTiles)
        {
            tile.ValidateWord();
        }

        foreach (var line in lineList)
        {
            line.GetComponent<Image>().color = new Color(78 / 255f, 200 / 255f, 75 / 255f);
        }
    }

    private void Invalidate(List<GameObject> lineList, List<Tile> selectingTiles)
    {
        foreach (var tile in selectingTiles)
        {
            tile.InvalidateWord();
        }

        foreach (var line in lineList)
        {
            line.GetComponent<Image>().color = new Color(110 / 255f, 110 / 255f, 110 / 255f);
        }
    }

    public void UpdateWordState(Tile tile, string currentWord, ref int currentScore, List<GameObject> lineList, List<Tile> selectingTiles)
    {
        if (currentWord.Length > 1)
        {
            if (GameDictionary.Instance.CheckWord(currentWord))
            {
                _wordDisplayText.text = $"{currentWord} <color={UpdateScore(ref currentScore, selectingTiles)}>({currentScore})</color>";
                Validate(lineList, selectingTiles);
                GameUIController.Instance.ToggleHintAndConfirm(false);
            }
            else
            {
                _wordDisplayText.text = currentWord;
                Invalidate(lineList, selectingTiles);
                GameUIController.Instance.ToggleHintAndConfirm(true);
            }

            UpdateWordDisplayPosition(tile);
        }
        else
        {
            GameUIController.Instance.ToggleHintAndConfirm(true);
            UndisplayWordAndScore();
            Invalidate(lineList, selectingTiles);
        }
    }

    private string UpdateScore(ref int currentScore, List<Tile> selectingTiles)
    {
        currentScore = selectingTiles.Count == 0 ? 0 : selectingTiles.Sum(tile => tile.Score) * selectingTiles.Count;
        // _currentTrueScore = _currentScore;

        var scoreColor = currentScore switch
        {
            <= 10 => "#EEEEEE",
            <= 20 => "#FFFF99",
            <= 30 => "#FFFF33",
            <= 40 => "#FFCC00",
            <= 50 => "#FF9900",
            <= 65 => "#FF5555",
            <= 80 => "#66FF00",
            _ => "#00FFFF",
        };

        return scoreColor;
    }

    private void UpdateWordDisplayPosition(Tile currentTile)
    {
        var tileRectTransform = currentTile.GetComponent<RectTransform>();
        var anchoredPosition = tileRectTransform.anchoredPosition;

        _wordDisplayRect.anchoredPosition = anchoredPosition + new Vector2(0, 175);
        var panelWidth = _wordDisplayText.preferredWidth;
        _wordDisplayRect.sizeDelta = new Vector2(panelWidth + 75, 100);

        var canvasRect = _wordDisplayRect.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        var canvasWidth = canvasRect.rect.width;
        var canvasHeight = canvasRect.rect.height;
        var canvas = _wordDisplayRect.GetComponentInParent<Canvas>();
        var canvasScaleFactor = canvas.scaleFactor;

        var padding = 100f;
        var clampedPosition = _wordDisplayRect.anchoredPosition;

        var maxX = (canvasWidth / 2) - _wordDisplayRect.sizeDelta.x / 2 / canvasScaleFactor - padding;
        var minX = -(canvasWidth / 2) + _wordDisplayRect.sizeDelta.x / 2 / canvasScaleFactor + padding;
        var maxY = (canvasHeight / 2) - _wordDisplayRect.sizeDelta.y / 2 / canvasScaleFactor - padding;
        var minY = -(canvasHeight / 2) + _wordDisplayRect.sizeDelta.y / 2 / canvasScaleFactor + padding;

        if (panelWidth + padding > canvasWidth)
        {
            while (panelWidth + padding > canvasWidth && _wordDisplayText.fontSize > 10)
            {
                _wordDisplayText.fontSize -= 1;
                panelWidth = _wordDisplayText.preferredWidth;
                _wordDisplayRect.sizeDelta = new Vector2(panelWidth + 75, 100);
            }

            clampedPosition = new Vector2(0, clampedPosition.y);
        }
        else
        {
            while (panelWidth + padding <= canvasWidth && _wordDisplayText.fontSize < 55)
            {
                _wordDisplayText.fontSize += 1;
                panelWidth = _wordDisplayText.preferredWidth;
                _wordDisplayRect.sizeDelta = new Vector2(panelWidth + 75, 100);
            }

            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        }

        _wordDisplayRect.anchoredPosition = clampedPosition;
    }
}
