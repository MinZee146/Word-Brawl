using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using TMPro;
using UnityEngine;

public class GameOverAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _coinsRewardRect, _playerStatsRect, _opponentStatsRect, _playerTrophyRect, _opponentTrophyRect;
    [SerializeField] private TextMeshProUGUI _playerBestWord, _opponentBestWord, _coinsEarnedText;
    [SerializeField] private GameObject _coins, _coinsStart, _coinsEnd;

    private void OnEnable()
    {
        Reset();

        var sequence = DOTween.Sequence();
        var showPlayerStats = _playerStatsRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        var showOpponentStats = _opponentStatsRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        sequence.Append(showPlayerStats);
        sequence.AppendInterval(0.25f);
        sequence.Append(showOpponentStats);
        sequence.AppendInterval(0.25f);

        var trophyAnim = PlayerStatsManager.Instance.IsPlayerWon() ?
        _playerTrophyRect.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBounce) :
        _opponentTrophyRect.DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBounce);
        sequence.Append(trophyAnim);
        sequence.AppendInterval(0.25f);

        var bestWord = PlayerStatsManager.Instance.IsPlayerHavingBestWord() ? _playerBestWord : _opponentBestWord;
        sequence.onComplete += () =>
        {
            WobbleAndRainbow(bestWord);
            CoinsEarnedAnimation();
        };
    }

    private void Reset()
    {
        _playerStatsRect.localScale = Vector3.zero;
        _opponentStatsRect.localScale = Vector3.zero;
        _playerTrophyRect.localScale = Vector3.zero;
        _opponentTrophyRect.localScale = Vector3.zero;

        _coinsRewardRect.localScale = Vector3.zero;
        _coinsEarnedText.text = "";

        UIManager.Instance.SetButtonInGameOverActive(false);
    }

    private void CoinsEarnedAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_coinsRewardRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce));
        sequence.AppendInterval(0.25f);
        sequence.onComplete += () =>
        {
            Timing.RunCoroutine(AnimateCoinIncrease(50));
        };
    }

    private void AnimateCoinMove()
    {
        _coins.transform.localPosition = _coinsStart.transform.localPosition;
        _coins.gameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.25f);
        sequence.Append(_coins.transform.DOScale(2f, 0.3f).SetEase(Ease.OutBack))
                .Join(_coins.transform.DORotate(new Vector3(0, 0, 360), 0.3f, RotateMode.FastBeyond360));
        sequence.AppendInterval(0.25f);
        sequence.Append(_coins.transform.DOScale(1.5f, 0.5f).SetEase(Ease.InOutQuad));
        sequence.Append(_coins.transform.DOMove(_coinsEnd.transform.position, 1f).SetEase(Ease.InOutQuad))
                .Join(_coins.transform.DOScale(1f, 1f).SetEase(Ease.InOutQuad));
        sequence.AppendInterval(0.25f);
        sequence.OnComplete(() =>
        {
            UIManager.Instance.SetButtonInGameOverActive(true);
            CurrencyManager.Instance.UpdateCoins(Convert.ToInt32(_coinsEarnedText.text));
            _coins.gameObject.SetActive(false);

            UIManager.Instance.ToggleDoubleRewardPanel(true);
            // DoubleRewardPopUp.Instance.FetchCurrentRewards(Convert.ToInt32(_coinsEarnedText.text), RewardType.Coin);
        });
    }

    private IEnumerator<float> AnimateCoinIncrease(int increment)
    {
        int startCoins = 0;
        int targetCoins = increment;

        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Timing.DeltaTime;
            float progress = elapsedTime / duration;
            int newCoins = Mathf.RoundToInt(Mathf.Lerp(startCoins, targetCoins, progress));
            _coinsEarnedText.text = newCoins.ToString();

            yield return Timing.WaitForOneFrame;
        }

        _coinsEarnedText.text = targetCoins.ToString();
        AnimateCoinMove();
    }

    private void WobbleAndRainbow(TextMeshProUGUI text)
    {
        var wobbleDuration = 0.6f;
        var changeColorDuration = 0.5f;
        var defaultTextColor = new Color(61 / 255f, 61 / 255f, 62 / 255f);
        var textInfo = text.textInfo;

        text.ForceMeshUpdate(true, true);

        var fullText = text.text;
        var colonIndex = fullText.IndexOf(":");
        if (colonIndex == -1) return;

        var startCharIndex = colonIndex + 1;

        for (int i = startCharIndex; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            var vertexIndex = charInfo.vertexIndex;
            var vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            var originalVertice = new Vector3[4];
            originalVertice[0] = vertices[vertexIndex];
            originalVertice[1] = vertices[vertexIndex + 1];
            originalVertice[2] = vertices[vertexIndex + 2];
            originalVertice[3] = vertices[vertexIndex + 3];

            var materialIndex = charInfo.materialReferenceIndex;
            var vertexColors = textInfo.meshInfo[materialIndex].colors32;
            var delay = (i - startCharIndex) * 0.1f; // Offset delay based on index

            DOTween.To(() => 0f, value =>
            {
                var wobbleAmount = Mathf.Sin(value * Mathf.PI * 2f) * 10f;

                for (int j = 0; j < 4; j++)
                {
                    vertices[vertexIndex + j] = originalVertice[j] + new Vector3(0, wobbleAmount, 0);
                }

                text.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                var color = Color.HSVToRGB((value + (i - startCharIndex) * 0.1f) % 1f, 1f, 1f);

                for (int j = 0; j < 4; j++)
                {
                    vertexColors[vertexIndex + j] = color;
                }

                text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            }, 1f, wobbleDuration)
            .SetEase(Ease.InOutSine)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                DOTween.To(() => vertexColors[vertexIndex], color =>
                {
                    for (int j = 0; j < 4; j++)
                    {
                        vertexColors[vertexIndex + j] = color;
                    }

                    text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                }, defaultTextColor, changeColorDuration).SetEase(Ease.InOutSine);
            });
        }
    }
}
