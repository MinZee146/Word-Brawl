using UnityEngine;

[CreateAssetMenu(fileName = "Shuffle", menuName = "Powerups/Shuffle")]
public class Shuffle : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "Shuffle";
        foreach (var tile in Board.Instance.TileList)
        {
            tile.SetTileConfig(Board.Instance.GetRandomLetter());
            tile.Deselect();
        }

        WordFinder.Instance.FindAllWords();
        GameManager.Instance.CheckForGameOver();
    }
}