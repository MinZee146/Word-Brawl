using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RevealWord", menuName = "Powerups/RevealWord")]
public class RevealWord : PowerUpBase
{
    public override void ApplyPowerUp()
    {
        Name = "RevealWord";

        var word = Board.Instance.FoundWords.Keys.OrderByDescending(word => word.Length).FirstOrDefault();
        UIManager.Instance.SetRevealedText(word);

        if (!GameFlowManager.Instance.IsPlayerTurn)
        {
            AI.Instance.ForcedWord = word;
        }
        else
        {
            UIManager.Instance.ToggleRevealWordPopUp();
        }
    }
}
