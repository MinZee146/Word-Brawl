using UnityEngine;

[CreateAssetMenu(fileName = "TimeFreeze", menuName = "Powerups/TimeFreeze")]
public class TimeFreeze : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "TimeFreeze";

        Notifier.Instance.PauseCountdown();
    }
}
