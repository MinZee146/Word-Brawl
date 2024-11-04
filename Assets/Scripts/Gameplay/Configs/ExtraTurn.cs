using UnityEngine;

[CreateAssetMenu(fileName = "ExtraTurn", menuName = "PowerUps/ExtraTurn")]
public class ExtraTurn : PowerUpBase
{
    public override void SetUpPowerUp()
    {
        Board.Instance.ExtraTurn();
    }
}

