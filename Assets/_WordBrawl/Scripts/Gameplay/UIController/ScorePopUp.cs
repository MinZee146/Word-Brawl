using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScorePopUp : Singleton<ScorePopUp>
{
    [SerializeField] private Sprite _blue, _red;
    [SerializeField] private GameObject _playerScoreBoard, _opponentScoreBoard;
    [SerializeField] private TextMeshProUGUI _score;

    private void OnEnable()
    {
        ShrinkAndMove();
    }

    public void SetScore(int score)
    {
        _score.text = score.ToString();
    }

    private void ShrinkAndMove()
    {
        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            GetComponent<Image>().sprite = _blue;
        }
        else
        {
            GetComponent<Image>().sprite = _red;
        }

        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.zero;

        Vector3 targetPos = GameFlowManager.Instance.IsPlayerTurn
            ? _playerScoreBoard.transform.position
            : _opponentScoreBoard.transform.position;

        var sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(1f, 0.5f).SetEase(Ease.InOutQuad));
        sequence.Append(
            DOTween.Sequence()
                .Join(transform.DOScale(0.5f, 0.5f).SetEase(Ease.InOutQuad))
                .Join(transform.DOMove(targetPos, 0.5f).SetEase(Ease.InOutQuad))
        );

        sequence.OnComplete(() =>
        {
            AudioManager.Instance.PlaySFX("Score");
            gameObject.SetActive(false);
        });
    }
}
