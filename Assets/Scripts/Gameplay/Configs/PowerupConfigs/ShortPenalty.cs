using UnityEngine;

[CreateAssetMenu(fileName = "ShortPenalty", menuName = "Powerups/ShortPenalty")]
public class ShortPenalty : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "ShortPenalty";
    }
}