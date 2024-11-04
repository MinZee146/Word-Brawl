using UnityEngine;

[CreateAssetMenu(fileName = "Shuffle", menuName = "PowerUps/Shuffle")]
public class Shuffle : PowerUpBase
{
    public override void SetUpPowerUp()
    {
        Board.Instance.Shuffle();

        if (GameManager.Instance.TurnManager.IsPLayerTurn)
        {
            Board.Instance.ShuffleTiles();
            Board.Instance.Shuffle();
        }
    }
}