using UnityEngine;

[CreateAssetMenu(fileName = "ShortBonus", menuName = "PowerUps/ShortBonus")]
public class ShortBonus : PowerUpBase
{
    public override void SetUpPowerUp()
    {
        Board.Instance.ShortBonus();
    }
}
