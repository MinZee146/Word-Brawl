using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerupsManager : SingletonPersistent<PowerupsManager>
{
    [SerializeField] private Button[] _powerUpsButtons;

    private PowerupBase[] _powerUpsList = new PowerupBase[6];

    public int PowerUpCounts()
    {
        var availablePowerUps = _powerUpsButtons.Where(button => button.interactable).ToList();
        return availablePowerUps.Count;
    }

    public void InitializePowerUps()
    {
        // Randomly select 6 unique powerUps
        var powerUps = Resources.LoadAll<PowerupBase>("PowerupConfigs").ToList();
        powerUps = powerUps.OrderBy(x => Random.value).ToList();

        for (var i = 0; i < 6; i++)
        {
            int index = i;
            _powerUpsList[index] = powerUps[index];

            _powerUpsButtons[index].interactable = true;
            _powerUpsButtons[index].onClick.AddListener(() => ApplyPowerUp(index));
            _powerUpsButtons[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = powerUps[index].Description;
            _powerUpsButtons[index].transform.GetChild(1).GetComponent<Image>().sprite = powerUps[index].Sprite;
        }
    }

    private void ApplyPowerUp(int index)
    {
        AudioManager.Instance.PlaySFX("PowerupSelect");

        _powerUpsButtons[index].interactable = false;

        //TODO: Powerup logic here
    }
}
