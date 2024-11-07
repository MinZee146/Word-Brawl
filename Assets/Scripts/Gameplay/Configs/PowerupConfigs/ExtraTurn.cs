using UnityEngine;

[CreateAssetMenu(fileName = "ExtraTurn", menuName = "Powerups/ExtraTurn")]
public class ExtraTurn : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "ExtraTurn";
    }
}

