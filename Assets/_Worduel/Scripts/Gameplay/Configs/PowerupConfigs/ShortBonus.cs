using UnityEngine;

[CreateAssetMenu(fileName = "ShortBonus", menuName = "Powerups/ShortBonus")]
public class ShortBonus : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "ShortBonus";
        AI.Instance.PreferShort = true;
    }
}
