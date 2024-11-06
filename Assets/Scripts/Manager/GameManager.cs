public class GameManager : Singleton<GameManager>
{
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GameDictionary.Instance.Initialize();
        AudioManager.Instance.Initialize();
        UIManager.Instance.Initialize();
        PowerupsManager.Instance.InitializePowerUps();
        NameRegister.Instance.Initialize();
    }

    public void NewGame()
    {
        GameFlowManager.Instance.StartGame();
    }

    public void CheckForGameOver()
    {
        WordFinder.Instance.FindAllWords();

        if (Board.Instance.FoundWords.Count == 0)
        {
            UIManager.Instance.ToggleGameOverScreen();
        }
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
