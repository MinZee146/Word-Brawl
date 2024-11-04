using UnityEngine;

[CreateAssetMenu(fileName = "DoubleScore", menuName = "PowerUps/DoubleScore")]
public class DoubleScore : PowerUpBase
{
    public override void SetUpPowerUp()
    {
        Board.Instance.DoubleScore();
    }
}
