using System.Collections.Generic;
using System.Linq;
using MEC;
using UnityEngine;

public class AI : Singleton<AI>
{
    public IEnumerator<float> AITurn()
    {
        yield return Timing.WaitUntilDone(Timing.RunCoroutine(ChoosePowerUp()));

        var longWords = Board.Instance.FoundWords.Keys.Where(word => word.Length >= 5).ToList();
        var shortWords = Board.Instance.FoundWords.Keys.Where(word => word.Length < 5).ToList();

        var randomValue = Random.Range(0f, 1f);
        List<string> selectedList;

        if (randomValue <= 0.35f && longWords.Count > 0)
        {
            selectedList = longWords;
        }
        else
        {
            selectedList = shortWords.Count > 0 ? shortWords : longWords;
        }

        var randomWord = selectedList[Random.Range(0, selectedList.Count)];

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(Board.Instance.OpponentSelect(randomWord)));
        yield return Timing.WaitForSeconds(0.6f);

        Board.Instance.DisconnectAll();
        Board.Instance.DeselectAll();
        WordDisplay.Instance.UndisplayWordAndScore();

        Timing.RunCoroutine(Board.Instance.PopAndRefresh());
    }

    private IEnumerator<float> ChoosePowerUp()
    {
        if (PowerUpsManager.Instance.PowerUpCounts() == 0) yield break;

        if (GameFlowManager.Instance.Turn > 2)
        {
            yield return Timing.WaitForSeconds(0.5f);
            PowerUpsManager.Instance.SelectRandomPowerUp();
            UIManager.Instance.ToggleOpponentPowerUpPanel();

            yield return Timing.WaitForSeconds(2.5f);
            UIManager.Instance.ToggleOpponentPowerUpPanel();
        }
    }
}
