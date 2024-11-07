using UnityEngine;

[CreateAssetMenu(fileName = "LongBonus", menuName = "Powerups/LongBonus")]
public class LongBonus : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "LongBonus";
    }
}
