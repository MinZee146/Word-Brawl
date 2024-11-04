using UnityEngine;

[CreateAssetMenu(fileName = "Grief", menuName = "PowerUps/Grief")]
public class Grief : PowerUpBase
{
    public override void SetUpPowerUp()
    {
        Board.Instance.Grief();
    }
}

