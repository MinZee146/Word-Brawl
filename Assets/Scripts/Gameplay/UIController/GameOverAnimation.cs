using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameOverAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _playerStats, _opponentStats;
    [SerializeField] private GameObject _playerTrophy, _opponentTrophy;
    [SerializeField] private TextMeshProUGUI _playerBestWord, _opponentBestWord;

    private void OnEnable()
    {
        Reset();
        var sequence = DOTween.Sequence();
        var showPlayerStats = _playerStats.GetComponent<RectTransform>().DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        var showOpponentStats = _opponentStats.GetComponent<RectTransform>().DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        sequence.Append(showPlayerStats);
        sequence.Append(showOpponentStats);

        if (PlayerStatsManager.Instance.IsPlayerWon())
        {
            var playerTrophyAnim = _playerTrophy.GetComponent<RectTransform>().DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBounce);
            sequence.Append(playerTrophyAnim);
        }
        else
        {
            var opponentTrophyAnim = _opponentTrophy.GetComponent<RectTransform>().DOScale(Vector3.one, 0.8f).SetEase(Ease.OutBounce);
            sequence.Append(opponentTrophyAnim);
        }

        if (PlayerStatsManager.Instance.IsPlayerHavingBestWord())
        {
            WobbleAndRainbow(_playerBestWord);
        }
        else
        {
            WobbleAndRainbow(_opponentBestWord);
        }
    }

    private void Reset()
    {
        _playerStats.transform.localScale = Vector3.zero;
        _opponentStats.transform.localScale = Vector3.zero;
        _playerTrophy.transform.localScale = Vector3.zero;
        _opponentTrophy.transform.localScale = Vector3.zero;
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
