public class GameFlowManager : SingletonPersistent<GameFlowManager>
{
    private bool _isGameOver;
    private bool _isPlayerTurn;
    private int _turn, _phase;

    public bool IsPlayerTurn => _isPlayerTurn;
    public bool IsGameOver => _isGameOver;
    public int Turn => _turn;
    public int Phase => _phase;

    public void StartGame()
    {
        _isPlayerTurn = true;
        _turn = 1;
        _phase = 1;
    }

    public void NextPhase()
    {
        _phase++;
        _turn = 1;

        PowerupsManager.Instance.Initialize();
    }

    public void NextTurn()
    {
        _turn++;
        _isPlayerTurn = !_isPlayerTurn;

        GameUIController.Instance.ToggleHintAndConfirm();

        if (_turn > 2 && _isPlayerTurn && PowerupsManager.Instance.PowerUpCounts() > 0)
        {
            UIManager.Instance.TogglePowerupsPanel();
        }
    }
}
