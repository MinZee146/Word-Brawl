using UnityEngine;

[CreateAssetMenu(fileName = "LetterReplace", menuName = "Powerups/LetterReplace")]
public class LetterReplace : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "ReplaceLetter";
        Board.Instance.HandleTileReplace += () =>
        {
            UIManager.Instance.ToggleTileReplacePopUp();
            UIManager.Instance.IsInteractable = false;
        };
    }
}