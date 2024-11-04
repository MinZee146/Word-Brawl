using UnityEngine;

[CreateAssetMenu(fileName = "RevealWord", menuName = "PowerUps/RevealWord")]
public class RevealWord : PowerUpBase
{
    public override void SetUpPowerUp()
    {
        Board.Instance.RevealWord();

        if (GameManager.Instance.TurnManager.IsPLayerTurn)
        {
            UIManager.Instance.UpdateHintText(Board.Instance.LongestWord());
            UIManager.Instance.ToggleHintPanel();
            Board.Instance.RevealWord();
        }
    }
}
