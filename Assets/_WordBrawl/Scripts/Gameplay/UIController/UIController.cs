using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : Singleton<GameUIController>
{
    [SerializeField] private GameObject _hintButton, _confirmButton, _scorePopUp;
    [SerializeField] private RectTransform _boardRectTransform;
    [SerializeField] private TextMeshProUGUI _roundText;
    [SerializeField] private Image _playerScoreboard, _opponentScoreboard, _background;

    private Coroutine _shakeHintCoroutine;
    public CanvasGroup GameplayCanvasGroup;

    private void OnEnable()
    {
        StartHintShakeRoutine();
        ApplyTheme();
    }

    private void OnDisable()
    {
        StopHintShakeRoutine();
    }

    private void ApplyTheme()
    {
        _playerScoreboard.color = ThemeManager.Instance.CurrentTheme.PlayerScoreBoard;
        _opponentScoreboard.color = ThemeManager.Instance.CurrentTheme.OpponentScoreBoard;
        _background.sprite = ThemeManager.Instance.CurrentTheme.Background;
    }

    public void UpdateRoundIndicator()
    {
        _roundText.text = _roundText.text == "1/2" ? "2/2" : "1/2";
    }

    public RectTransform ConfirmButtonRect()
    {
        return _confirmButton.GetComponent<RectTransform>();
    }

    public RectTransform BoardRectTransform()
    {
        return _boardRectTransform;
    }

    public void ScorePopUp()
    {
        _scorePopUp.SetActive(true);
    }

    public void ToggleHintAndConfirm(bool hintState = true, bool display = true)
    {
        _hintButton.transform.DOKill();
        _confirmButton.transform.DOKill();

        if (!display || !GameFlowManager.Instance.IsPlayerTurn)
        {
            _hintButton.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => _hintButton.SetActive(false));
            _confirmButton.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => _confirmButton.SetActive(false));

            return;
        }

        if (hintState)
        {
            _hintButton.transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.InOutSine);
            _hintButton.SetActive(true);
            _hintButton.transform.DOScale(Vector3.one, 0.15f);
            _confirmButton.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => _confirmButton.SetActive(false));
        }
        else
        {
            _confirmButton.SetActive(true);
            _confirmButton.transform.DOScale(Vector3.one, 0.15f);
            _hintButton.transform.DOScale(Vector3.zero, 0.15f).OnComplete(() => _hintButton.SetActive(false));
        }
    }

    public void OpenSettings()
    {
        UIManager.Instance.ToggleSettingsPanel(true);
    }

    private void StartHintShakeRoutine()
    {
        StopHintShakeRoutine();
        _shakeHintCoroutine = StartCoroutine(HintShakeAfterDelay());
    }

    private void StopHintShakeRoutine()
    {
        if (_shakeHintCoroutine != null)
        {
            StopCoroutine(_shakeHintCoroutine);
            _shakeHintCoroutine = null;
        }
    }

    private IEnumerator HintShakeAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        _hintButton.transform.DORotate(new Vector3(0, 0, 5), 0.25f)
        .From(new Vector3(0, 0, -5))
        .SetLoops(2, LoopType.Yoyo)
        .SetEase(Ease.InOutSine)
        .OnComplete(() =>
        {
            _hintButton.transform.DORotate(Vector3.zero, 0.1f).SetEase(Ease.InOutSine);
        });

        StartHintShakeRoutine();
    }
}
