using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GameUIController : Singleton<GameUIController>
{
    [SerializeField] private GameObject _hintButton, _confirmButton;
    [SerializeField] private RectTransform _boardRectTransform;

    private Coroutine _shakeHintCoroutine;

    private void OnEnable()
    {
        StartHintShakeRoutine();
    }

    private void OnDisable()
    {
        StopHintShakeRoutine();
    }

    public RectTransform ConfirmButtonRect()
    {
        return _confirmButton.GetComponent<RectTransform>();
    }

    public RectTransform BoardRectTransform()
    {
        return _boardRectTransform;
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

    public void ToggleSettings()
    {
        UIManager.Instance.ToggleSettingsPanel();
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
