using UnityEngine;

[CreateAssetMenu(fileName = "Shuffle", menuName = "Powerups/Shuffle")]
public class Shuffle : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "Shuffle";

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            Board.Instance.ShuffleBoard();
        }
    }
}