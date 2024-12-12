using UnityEngine;

public class Roulette : MonoBehaviour
{
    [SerializeField] private float _rotatePower;
    [SerializeField] private float _stopPower;

    private Rigidbody2D _rbody;
    private int _inRotate;
    private float _time;

    private void Start()
    {
        _rbody = GetComponent<Rigidbody2D>();
        GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, Random.Range(0, 9) * 36);
    }

    private void Update()
    {
        if (_rbody.angularVelocity > 0)
        {
            _rbody.angularVelocity -= _stopPower * Time.deltaTime;

            _rbody.angularVelocity = Mathf.Clamp(_rbody.angularVelocity, 0, 1440);
        }

        if (_rbody.angularVelocity == 0 && _inRotate == 1)
        {
            _time += 1 * Time.deltaTime;
            if (_time >= 0.5f)
            {
                GetReward();

                _inRotate = 0;
                _time = 0;
            }
        }
    }

    public void Rotate()
    {
        if (_inRotate == 0)
        {
            _rbody.AddTorque(_rotatePower);
            _inRotate = 1;
        }
        RewardManager.Instance.DisableSpin();
        AudioManager.Instance.PlaySFX("Spin");
    }

    public void GetReward()
    {
        float rotationZ = transform.eulerAngles.z;
        rotationZ = (rotationZ + 360) % 360;

        int section = Mathf.RoundToInt(rotationZ / 36);
        Win(section);

        AudioManager.Instance.PlaySFX("Bell");
    }

    public void Win(int section)
    {
        GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, section * 36);
        UIManager.Instance.ToggleDoubleRewardPanel(true);

        switch (section)
        {
            case 0:
                PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_HINT_COUNTER) + 1);
                RewardPopUp.Instance.FetchCurrentRewards(1, RewardType.Hint);
                break;
            case 2:
                PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_HINT_COUNTER) + 2);
                RewardPopUp.Instance.FetchCurrentRewards(2, RewardType.Hint);
                break;
            case 4:
                PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_HINT_COUNTER) + 3);
                RewardPopUp.Instance.FetchCurrentRewards(3, RewardType.Hint); ;
                break;
            case 6:
                PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_HINT_COUNTER) + 4);
                RewardPopUp.Instance.FetchCurrentRewards(4, RewardType.Hint);
                break;
            case 8:
                PlayerPrefs.SetInt(GameConstants.PLAYERPREFS_HINT_COUNTER, PlayerPrefs.GetInt(GameConstants.PLAYERPREFS_HINT_COUNTER) + 5);
                RewardPopUp.Instance.FetchCurrentRewards(5, RewardType.Hint);
                break;
            case 1:
                CurrencyManager.Instance.UpdateCoins(10);
                RewardPopUp.Instance.FetchCurrentRewards(10, RewardType.Coin);
                break;
            case 3:
                CurrencyManager.Instance.UpdateCoins(20);
                RewardPopUp.Instance.FetchCurrentRewards(20, RewardType.Coin);
                break;
            case 7:
                CurrencyManager.Instance.UpdateCoins(30);
                RewardPopUp.Instance.FetchCurrentRewards(30, RewardType.Coin);
                break;
            case 9:
                CurrencyManager.Instance.UpdateCoins(40);
                RewardPopUp.Instance.FetchCurrentRewards(40, RewardType.Coin);
                break;
            case 5:
                Debug.Log("No reward this time!");
                break;
        }
    }
}
