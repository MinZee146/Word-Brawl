using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopUp : Singleton<RewardPopUp>
{
    [SerializeField] private Sprite _hintIcon;
    [SerializeField] private Sprite _coinIcon;
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private Image _rewardImage;

    public void FetchCurrentRewards(int amount, RewardType rewardType)
    {
        _amountText.text = amount.ToString();

        switch (rewardType)
        {
            case RewardType.Hint:
                _rewardImage.sprite = _hintIcon;
                break;
            case RewardType.Coin:
                _rewardImage.sprite = _coinIcon;
                break;
        }
    }
}

public enum RewardType
{
    Hint,
    Coin
}

