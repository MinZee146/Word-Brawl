using UnityEngine;

[CreateAssetMenu(fileName = "DoubleScore", menuName = "Powerups/DoubleScore")]
public class DoubleScore : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "DoubleScore";
    }
}
