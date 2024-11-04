using UnityEngine;

[CreateAssetMenu(fileName = "LetterReplace", menuName = "PowerUps/LetterReplace")]
public class LetterReplace : PowerUpBase
{
    public override void SetUpPowerUp()
    {
        UIManager.Instance.UpdateIndicatorText("Select a letter to replace.");
        Board.Instance.SelectTileToReplace();
    }
}