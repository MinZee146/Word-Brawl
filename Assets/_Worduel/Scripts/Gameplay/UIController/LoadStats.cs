using TMPro;
using UnityEngine;

public class LoadStats : Singleton<LoadStats>
{
    [SerializeField] private TextMeshProUGUI _name, _battleHistory, _bestWord, _mostUsedPowerUp;

    public void Load()
    {
        _name.text = "Name: " + $"<color=orange>{PlayerPrefs.GetString(GameConstants.PLAYERPREFS_USERNAME, "")}</color>";
        _battleHistory.text = $"<color=green>{PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_WINS, 0)}</color> wins\n<color=red>{PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_LOSSES, 0)}</color> losses";
        _bestWord.text = $"Best word of all time:\n\n<color=blue><size=60>{PlayerPrefs.GetString(GameConstants.PLAYERPREFS_BEST_WORD_OF_ALL_TIME, "")}</size></color>";
        _mostUsedPowerUp.text = $"Most used powerup:\n\n<color=purple><size=60>{PlayerDataTracker.Instance.GetMostUsedPowerUp()}</size></color>";
    }

    public void UpdateName()
    {
        _name.text = "Name: " + $"<color=orange>{PlayerPrefs.GetString(GameConstants.PLAYERPREFS_USERNAME, "")}</color>";
    }
}
