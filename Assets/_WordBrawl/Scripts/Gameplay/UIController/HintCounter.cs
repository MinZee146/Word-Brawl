using TMPro;
using UnityEngine;

public class HintCounter : Singleton<HintCounter>
{
    public int CurrentHintCounter => _currentHintCounter;
    public int TotalHintUsed { get; private set; }
    public int HintUsedAtRound { get; private set; }

    [SerializeField] private TextMeshProUGUI _hintText;

    private int _currentHintCounter;

    public void FetchHintPref()
    {
        _currentHintCounter = PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, 5);
        _hintText.text = _currentHintCounter != 0 ? _currentHintCounter.ToString() : "+";
    }

    public void UpdateCounter()
    {
        TotalHintUsed++;
        _currentHintCounter--;
        _hintText.text = _currentHintCounter != 0 ? _currentHintCounter.ToString() : "+";

        PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, _currentHintCounter);
    }

    public void Reset()
    {
        TotalHintUsed = HintUsedAtRound = 0;
    }

    public void SetStatsAtRound()
    {
        HintUsedAtRound = TotalHintUsed - HintUsedAtRound;
    }
}
