using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Random = UnityEngine.Random;

public class AI : Singleton<AI>
{
    public string ForcedWord;
    public bool PreferLong, PreferShort;

    public IEnumerator<float> AITurn()
    {
        if (!PowerUpsManager.Instance.CheckExtraTurn)
            yield return Timing.WaitUntilDone(Timing.RunCoroutine(ChoosePowerUp()));

        var longWords = Board.Instance.FoundWords.Keys.Where(word => word.Length >= 5).ToList();
        var shortWords = Board.Instance.FoundWords.Keys.Where(word => word.Length < 5).ToList();

        var randomValue = Random.Range(0f, 1f);
        List<string> selectedList;

        if (PreferLong && longWords.Count > 0)
        {
            selectedList = longWords;
        }
        else if (PreferShort && shortWords.Count > 0)
        {
            selectedList = shortWords;
        }
        else if (randomValue <= 0.35f && longWords.Count > 0)
        {
            selectedList = longWords;
        }
        else
        {
            selectedList = shortWords;
        }

        var randomWord = String.IsNullOrEmpty(ForcedWord) ? selectedList[Random.Range(0, selectedList.Count)] : ForcedWord;

        yield return Timing.WaitUntilDone(Timing.RunCoroutine(Board.Instance.OpponentSelect(randomWord)));
        yield return Timing.WaitForSeconds(0.6f);

        Board.Instance.DisconnectAll();
        Board.Instance.DeselectAll();
        WordDisplay.Instance.UndisplayWordAndScore();

        Timing.RunCoroutine(Board.Instance.PopAndRefresh());

        yield return Timing.WaitForSeconds(1.5f);
        if (PowerUpsManager.Instance.CheckExtraTurn)
        {
            Timing.RunCoroutine(AITurn());
        }

        ForcedWord = null;
    }

    private IEnumerator<float> ChoosePowerUp()
    {
        if (PowerUpsManager.Instance.PowerUpCounts() == 0 || GameFlowManager.Instance.Turn <= 2)
        {
            yield return Timing.WaitForSeconds(1.5f);
        }
        else
        {
            yield return Timing.WaitForSeconds(0.75f);
            PowerUpsManager.Instance.SelectRandomPowerUp();
            UIManager.Instance.ToggleOpponentPowerUpPanel();

            yield return Timing.WaitForSeconds(1.5f);
            UIManager.Instance.ToggleOpponentPowerUpPanel();

            yield return Timing.WaitForSeconds(1f);
        }
    }
}
