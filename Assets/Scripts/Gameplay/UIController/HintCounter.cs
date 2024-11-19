using TMPro;
using UnityEngine;
using System;

public class HintCounter : Singleton<HintCounter>
{
    public int CurrentHintCounter => _currentHintCounter;

    [SerializeField] private TextMeshProUGUI _hintText;

    private int _currentHintCounter;

    private const int DailyHintIncrease = 5;
    private const string HintCounterKey = "HintCounter";
    private const string LastHintUpdateKey = "LastHintUpdate";

    public void FetchHintPref()
    {
        _currentHintCounter = PlayerPrefs.GetInt(HintCounterKey, 5);

        var lastUpdate = PlayerPrefs.GetString(LastHintUpdateKey, "");
        var today = DateTime.Now.ToString("yyyy-MM-dd");

        if (lastUpdate != today)
        {
            _currentHintCounter += DailyHintIncrease;
            PlayerPrefs.SetString(LastHintUpdateKey, today);
            PlayerPrefs.SetInt(HintCounterKey, _currentHintCounter);
        }

        _hintText.text = _currentHintCounter.ToString();
    }

    public void UpdateCounter()
    {
        _currentHintCounter--;
        PlayerPrefs.SetInt(HintCounterKey, _currentHintCounter);
        _hintText.text = _currentHintCounter.ToString();
    }

    public void DebugUpdateCounter(int counter)
    {
        _currentHintCounter = counter;
        PlayerPrefs.SetInt(HintCounterKey, _currentHintCounter);
        _hintText.text = _currentHintCounter.ToString();
    }
}
