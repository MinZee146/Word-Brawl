using UnityEngine;

[CreateAssetMenu(fileName = "LetterReplace", menuName = "Powerups/LetterReplace")]
public class LetterReplace : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "ReplaceLetter";

        if (GameFlowManager.Instance.IsPlayerTurn)
        {
            Board.Instance.HandleTileReplace += () =>
            {
                UIManager.Instance.ToggleTileReplacePopUp(true);
            };
        }
    }
}