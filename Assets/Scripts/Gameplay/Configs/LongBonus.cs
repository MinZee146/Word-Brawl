using UnityEngine;

[CreateAssetMenu(fileName = "LongBonus", menuName = "PowerUps/LongBonus")]
public class LongBonus : PowerUpBase
{
    public override void SetUpPowerUp()
    {
        Board.Instance.LongBonus();
    }
}
