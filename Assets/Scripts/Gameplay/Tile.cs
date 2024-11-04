using System.Collections.Generic;
using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField] private Image _innerCircle, _background;
    [SerializeField] private Sprite _idleSprite, _selectingSprite, _validSprite;
    [SerializeField] private TextMeshProUGUI _scoreText, _letterText;
    [SerializeField] private Gradient _gradient;

    public int Row { get; set; }
    public int Column { get; set; }
    public bool IsRowEven { get; set; }
    public char Letter { get; private set; }
    public int Score { get; private set; }
    public Color Color { get; private set; }

    public bool IsAdjacent(Tile tile)
    {
        if (IsRowEven)
        {
            return (tile.Column == Column - 1 && tile.Row >= Row - 1 && tile.Row <= Row + 1) ||
                   (tile.Column == Column && (tile.Row == Row - 1 || tile.Row == Row + 1)) ||
                   (tile.Column == Column + 1 && tile.Row == Row);
        }
        return (tile.Column == Column - 1 && tile.Row == Row) ||
               (tile.Column == Column && (tile.Row == Row - 1 || tile.Row == Row + 1)) ||
               (tile.Column == Column + 1 && tile.Row >= Row - 1 && tile.Row <= Row + 1);
    }

    public void SetTileConfig(TileConfig config)
    {
        //Get config
        Letter = config.Letter;
        Score = config.Score;
        Color = config.Color;

        //Apply config
        _scoreText.text = Score.ToString();
        _letterText.text = Letter.ToString();
        _innerCircle.color = Color;
    }

    public void Select()
    {
        _innerCircle.sprite = _selectingSprite;
        _innerCircle.color = Color.white;
        _letterText.color = Color.white;
        _scoreText.color = Color.white;
        _background.color = new Color(1f, 1f, 1f, 0f);

        _innerCircle.transform.DOComplete();
        _innerCircle.transform.DOShakePosition(0.5f, 10f, 10, 75, false, true, ShakeRandomnessMode.Harmonic);
        AudioManager.Instance.PlaySFX("TileSelect");
    }

    public void ValidateWord()
    {
        _innerCircle.sprite = _validSprite;
        _letterText.color = Color.black;
        _scoreText.color = Color.black;
    }

    public void InvalidateWord()
    {
        _innerCircle.sprite = _selectingSprite;
        _letterText.color = Color.white;
        _scoreText.color = Color.white;
    }

    public Tween Hint()
    {
        return _innerCircle.DOGradientColor(_gradient, 0.4f).OnComplete(() =>
        {
            Deselect();
            AudioManager.Instance.PlaySFX("Hint");
        });
    }

    public void Deselect()
    {
        _innerCircle.sprite = _idleSprite;
        _innerCircle.color = Color;
        _letterText.color = Color.black;
        _scoreText.color = Color.black;
        _background.color = Color.white;

        _innerCircle.transform.DOComplete();
        _innerCircle.transform.DOShakePosition(0.5f, 10f, 10, 75, false, true, ShakeRandomnessMode.Harmonic);
    }

    public IEnumerator<float> PopAndDestroy()
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(PopAnimation()));

        var random = Random.Range(0f, 1f);

        if (random > 0.5f)
        {
            AudioManager.Instance.PlaySFX("Pop1");
        }
        else
        {
            AudioManager.Instance.PlaySFX("Pop2");
        }

        Destroy(gameObject);
    }

    IEnumerator<float> PopAnimation()
    {
        transform.DOScale(0f, 0.5f);

        yield return Timing.WaitForSeconds(0.2f);
    }
}

