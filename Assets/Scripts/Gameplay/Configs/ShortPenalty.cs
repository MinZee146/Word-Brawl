using UnityEngine;

[CreateAssetMenu(fileName = "ShortPenalty", menuName = "PowerUps/ShortPenalty")]
public class ShortPenalty : PowerUpBase
{
    public override void SetUpPowerUp()
    {
        Board.Instance.ShortPenalty();
    }
}