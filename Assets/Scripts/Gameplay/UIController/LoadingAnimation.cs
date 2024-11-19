using UnityEngine;
using DG.Tweening;
using System;

public class LoadingAnimation : SingletonPersistent<LoadingAnimation>
{
    [SerializeField] private GameObject _loadingPanel;
    [SerializeField] private RectTransform _leftPanel, _rightPanel, _topPanel;

    private Vector2 leftOriginalPos;
    private Vector2 rightOriginalPos;
    private Vector2 topOriginalPos;
    float halfScreenWidth = Screen.width / 2f;
    float screenHeight = Screen.height;

    public void Initialize()
    {
        _loadingPanel.SetActive(false);

        _leftPanel.sizeDelta = new Vector2(halfScreenWidth, screenHeight);
        _rightPanel.sizeDelta = new Vector2(halfScreenWidth, screenHeight);
        _topPanel.sizeDelta = new Vector2(Screen.width, screenHeight / 5);

        leftOriginalPos = new Vector2(-halfScreenWidth, 0);
        rightOriginalPos = new Vector2(halfScreenWidth, 0);
        topOriginalPos = new Vector2(0, screenHeight / 5);

        _leftPanel.anchoredPosition = leftOriginalPos;
        _rightPanel.anchoredPosition = rightOriginalPos;
        _topPanel.anchoredPosition = topOriginalPos;
    }

    public void AnimationLoading(float transitionDuration, Action onComplete = null)
    {
        _loadingPanel.SetActive(true);

        Sequence closeSequence = DOTween.Sequence();
        closeSequence.Join(_topPanel.DOAnchorPos(new Vector2(0, -2 * screenHeight / 25), transitionDuration));
        closeSequence.Join(_leftPanel.DOAnchorPos(Vector2.zero, transitionDuration));
        closeSequence.Join(_rightPanel.DOAnchorPos(Vector2.zero, transitionDuration));
        closeSequence.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void AnimationLoaded(float transitionDuration, float delay)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            Sequence openSequence = DOTween.Sequence();
            openSequence.Join(_topPanel.DOAnchorPos(topOriginalPos, transitionDuration));
            openSequence.Join(_leftPanel.DOAnchorPos(leftOriginalPos, transitionDuration));
            openSequence.Join(_rightPanel.DOAnchorPos(rightOriginalPos, transitionDuration));
            openSequence.OnComplete(() =>
            {
                _loadingPanel.SetActive(false);
            });
        });
    }
}