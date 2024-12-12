using UnityEngine;

[CreateAssetMenu(fileName = "Cleanse", menuName = "Powerups/Cleanse")]
public class Cleanse : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "Cleanse";
    }
}
